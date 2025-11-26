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
        //先检查当前关卡是不是boss关卡
        //Q 怎么检查？用名字，LevelBoss
        if (hasKilledAll)
        {
            if (other.CompareTag("Player"))
            {
                //检查当前关卡是不是boss关卡
                int currentLevelIndex = GameLevelManager.Instance.GetCurrentLevelIndex();
                if (currentLevelIndex == -1)
                {
                    UIManager.Instance.ShowLoadingPanel(true);
                    //开始协程，等待2s后返回主菜单
                    StartCoroutine(ReturnToMainMenuAfterDelay(2f));
                    return;
                }
                //触发关卡完成
                levelManager.LevelComplete();
                UIManager.Instance.ShowLoadingPanel(true);
            }
        }
    }

    private IEnumerator ReturnToMainMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameLevelManager.Instance.FailAndReturnToMainMenu();
    }
}
