using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    public LevelManager levelManager;
    //关卡结束点-杀死所有敌人后碰撞到3D墙体触发关卡结束
    private bool hasKilledAll = false;
    //杀死所有敌人后将墙体设为触发器
    private void Update()
    {
        if (hasKilledAll) return;
        if(levelManager.CheckWinCondition())
        {
            hasKilledAll = true;
            GetComponent<Collider>().isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        levelManager.LevelComplete();
        UIManager.Instance.ShowLoadingPanel(true);
    }
}
