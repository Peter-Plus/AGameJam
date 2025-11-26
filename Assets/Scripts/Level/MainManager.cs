using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : LevelManager
{
    // Start is called before the first frame update
    void Start()
    {
        //显示菜单面板
        UIManager.Instance.beginPanel.Show();
        //播放主界面音乐
        AudioManager.Instance.PlayerMainMusic();
    }

}
