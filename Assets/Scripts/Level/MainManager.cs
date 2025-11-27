using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : LevelManager
{
    [Header("Background")]
    public SpriteRenderer MainBK;

    public void ScaleBackground(float scaleMultiplier, float duration)
    {
        if (MainBK != null)
        {
            Vector3 targetScale = MainBK.transform.localScale * scaleMultiplier;
            MainBK.transform.DOScale(targetScale, duration).SetEase(Ease.InQuad);
        }
    }
    void Start()
    {
        //显示菜单面板
        UIManager.Instance.beginPanel.HideInstant(); // 先瞬间隐藏
        UIManager.Instance.beginPanel.Show(); // 再显示，触发淡入动画
        //播放主界面音乐
        AudioManager.Instance.PlayerMainMusic();
    }

}
