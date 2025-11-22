using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// PlayerPrefs单例管理器
/// 支持自动序列化类、List、Dictionary等复杂数据结构
/// </summary>
public class PlayerPrefsMgr
{
    private static PlayerPrefsMgr instance = new PlayerPrefsMgr();
    public static PlayerPrefsMgr Instance => instance;

    private PlayerPrefsMgr() { }

    #region 基础方法 - 直接封装PlayerPrefs
    public void SaveInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public int GetInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public void SaveFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    public float GetFloat(string key, float defaultValue = 0f)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public void SaveString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public string GetString(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    public void SaveBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }

    public bool GetBool(string key, bool defaultValue = false)
    {
        return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
    }

    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    public void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    public void Save()
    {
        PlayerPrefs.Save();
    }
    #endregion

    #region 高级方法 - 自动序列化对象
    /// <summary>
    /// 保存任意对象
    /// </summary>
    public void SaveObject(object data, string keyName)
    {
        Type dataType = data.GetType();
        FieldInfo[] fields = dataType.GetFields();

        foreach (FieldInfo field in fields)
        {
            string saveKey = $"{keyName}_{dataType.Name}_{field.FieldType.Name}_{field.Name}";
            object value = field.GetValue(data);
            SaveValue(value, saveKey);
        }

        PlayerPrefs.Save();
    }

    /// <summary>
    /// 加载对象
    /// </summary>
    public T LoadObject<T>(string keyName) where T : new()
    {
        return (T)LoadObject(typeof(T), keyName);
    }

    public object LoadObject(Type type, string keyName)
    {
        object data = Activator.CreateInstance(type);
        FieldInfo[] fields = type.GetFields();

        foreach (FieldInfo field in fields)
        {
            string loadKey = $"{keyName}_{type.Name}_{field.FieldType.Name}_{field.Name}";
            object value = LoadValue(field.FieldType, loadKey);
            if (value != null)
            {
                field.SetValue(data, value);
            }
        }

        return data;
    }

    /// <summary>
    /// 判断是否存在该对象
    /// </summary>
    /// <param name="keyName"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool HasObject(string keyName, Type type)
    {
        FieldInfo[] fields = type.GetFields();
        if (fields.Length == 0) return false;

        // 检查第一个字段的key
        FieldInfo firstField = fields[0];
        string firstKey = $"{keyName}_{type.Name}_{firstField.FieldType.Name}_{firstField.Name}";
        return PlayerPrefs.HasKey(firstKey);
    }
    #endregion

    #region 内部递归方法
    private void SaveValue(object value, string key)
    {
        if (value == null) return;

        Type valueType = value.GetType();

        // 基础类型
        if (valueType == typeof(int))
            PlayerPrefs.SetInt(key, (int)value);
        else if (valueType == typeof(float))
            PlayerPrefs.SetFloat(key, (float)value);
        else if (valueType == typeof(string))
            PlayerPrefs.SetString(key, (string)value);
        else if (valueType == typeof(bool))
            PlayerPrefs.SetInt(key, (bool)value ? 1 : 0);
        // List
        else if (typeof(IList).IsAssignableFrom(valueType))
        {
            IList list = value as IList;
            PlayerPrefs.SetInt(key, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                SaveValue(list[i], $"{key}_{i}");
            }
        }
        // Dictionary
        else if (typeof(IDictionary).IsAssignableFrom(valueType))
        {
            IDictionary dict = value as IDictionary;
            PlayerPrefs.SetInt(key, dict.Count);
            int index = 0;
            foreach (object dictKey in dict.Keys)
            {
                SaveValue(dictKey, $"{key}_key_{index}");
                SaveValue(dict[dictKey], $"{key}_value_{index}");
                index++;
            }
        }
        // 自定义类
        else
        {
            SaveObject(value, key);
        }
    }

    private object LoadValue(Type fieldType, string key)
    {
        if (!PlayerPrefs.HasKey(key)) return GetDefaultValue(fieldType);

        // 基础类型
        if (fieldType == typeof(int))
            return PlayerPrefs.GetInt(key, 0);
        else if (fieldType == typeof(float))
            return PlayerPrefs.GetFloat(key, 0f);
        else if (fieldType == typeof(string))
            return PlayerPrefs.GetString(key, "");
        else if (fieldType == typeof(bool))
            return PlayerPrefs.GetInt(key, 0) == 1;
        // List
        else if (typeof(IList).IsAssignableFrom(fieldType))
        {
            int count = PlayerPrefs.GetInt(key, 0);
            IList list = Activator.CreateInstance(fieldType) as IList;
            Type elementType = fieldType.GetGenericArguments()[0];
            for (int i = 0; i < count; i++)
            {
                list.Add(LoadValue(elementType, $"{key}_{i}"));
            }
            return list;
        }
        // Dictionary
        else if (typeof(IDictionary).IsAssignableFrom(fieldType))
        {
            int count = PlayerPrefs.GetInt(key, 0);
            IDictionary dict = Activator.CreateInstance(fieldType) as IDictionary;
            Type[] kvTypes = fieldType.GetGenericArguments();
            for (int i = 0; i < count; i++)
            {
                object dictKey = LoadValue(kvTypes[0], $"{key}_key_{i}");
                object dictValue = LoadValue(kvTypes[1], $"{key}_value_{i}");
                dict.Add(dictKey, dictValue);
            }
            return dict;
        }
        // 自定义类，递归实现
        else
        {
            return LoadObject(fieldType, key);
        }
    }

    private object GetDefaultValue(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }
    #endregion
}