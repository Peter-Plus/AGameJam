using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPots : InteractItem
{
    #region fields
    [Header("对话内容")]
    public string[] dialogues = new string[]
    
    {
        "咦，这个红瓶子是什么东西？",
        "你好啊，我是血瓶，嘻嘻，快来尝尝我！",
        "我可以恢复你的生命值哦~~~~",
        "你得走过来才能喝到我哟~~~~",
        "快点啊，不然我会凉掉的~~~~",
        "快点啊，不然我会凉掉的~~~~",
        "快点啊，不然我会凉掉的~~~~"
    };
    public string speakName;
    private Sprite speakIcon;
    #endregion

    #region 生命周期
    protected override void Awake()
    {
        base.Awake();
        //加载资源，玩家、speak信息可能不便在场景中指定，如果用预设体的话
        speakIcon = Resources.Load<Sprite>("Texture/UI/AIChara/girl1");
    }
    #endregion

    #region 碰撞检测
    //当玩家碰到血瓶时，增加玩家的生命值
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerCore playerCore = collision.GetComponent<PlayerCore>();
            if (playerCore != null)
            {
                playerCore.PickupHealthPotion();
                Destroy(gameObject);
            }
        }
    }
    #endregion

    #region 重写逻辑
    protected override IEnumerator OnInteract()
    {
        bool done = false;
        switch (currentInteractCount)
        {
            case 1:
                done = false;
                UIManager.Instance.ShowChat(dialogues[0], speakName, speakIcon, false, true, () => done = true);
                yield return new WaitUntil(() => done);
                UIManager.Instance.ShowChat(dialogues[1], itemName, itemIcon.sprite, false, false);
                break;
            case 2:
                UIManager.Instance.ShowChat(dialogues[2], itemName, itemIcon.sprite, false, false);
                break;
            case 3:
                UIManager.Instance.ShowChat(dialogues[3], itemName, itemIcon.sprite, false, false);
                break;
            case 4:
                UIManager.Instance.ShowChat(dialogues[4], itemName, itemIcon.sprite, false, false);
                break;
            case 5:
                UIManager.Instance.ShowChat(dialogues[5], itemName, itemIcon.sprite, false, false);
                break;
            case 6:
                UIManager.Instance.ShowChat(dialogues[6], itemName, itemIcon.sprite, false, false);
                break;
            default:
                break;
        }
    }

    protected override void OnMaxInteractReached()
    {
        UIManager.Instance.ShowTextTip("血瓶已经凉了！",60);
    }

    protected override void OnMaxDistenceReached()
    {
        UIManager.Instance.ShowTextTip("离血瓶太远了，走近点吧！",60);
    }
    #endregion
}
