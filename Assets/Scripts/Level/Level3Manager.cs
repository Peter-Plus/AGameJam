using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Level3Manager : LevelManager
{
    [Header("关卡3配置")]
    public string levelName = "街道2";

    [Header("对话内容")]
    [TextArea(2, 4)]
    public string dialogue1 = "雪夜便利店的自动门开合间带进细碎的雪花,两个穿着校服的女生挤在暖柜旁。";
    [TextArea(2, 4)]
    public string dialogue2 = "所以这就是你翘掉补习班的理由?为了吃关东煮?";
    [TextArea(2, 4)]
    public string dialogue3 = "为了和你一起吃这个。";
    [TextArea(2, 4)]
    public string finalSpeak = "\"后来被老师骂了个狗血淋头,不知道她有没有后悔……\"";

    [Header("角色名称")]
    public string girl1Name = "菊 渚月";
    public string girl2Name = "浅仓 凛凛子";

    private GameObject player;
    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Start()
    {
        //判断是不是第一次进入该关卡
        if (DataManager.Instance.IsFirstTimeReachLevel(0))
        {
            StartInteract();
        }
        else
        {
            UIManager.Instance.ShowTextTip(levelName);
            UIManager.Instance.ShowGameUI(true);
        }
    }


    protected override void Update()
    {
        base.Update();
    }

    // 需要重写的交互内容
    public override void StartInteract()
    {

        StartCoroutine(InteractSequence());

    }

    private IEnumerator InteractSequence()
    {
        //暂停游戏
        Time.timeScale = 0f;
        // 加载资源
        Sprite blackScreen = Resources.Load<Sprite>("CG/BlackScreen");
        Sprite illustration1 = Resources.Load<Sprite>("CG/Illustration1");
        Sprite illustration2 = Resources.Load<Sprite>("CG/Illustration2");
        Sprite girl1 = Resources.Load<Sprite>("Texture/UI/AIChara/girl1");
        Sprite girl2 = Resources.Load<Sprite>("Texture/UI/AIChara/girl2");
        bool done = false;
        //黑幕1.5s
        UIManager.Instance.ShowCGPanelInstant(blackScreen, 1.5f, () => { done = true; });
        // 插图1
        UIManager.Instance.ShowGameCGPanel(illustration1);
        yield return new WaitUntil(() => done);
        // 等待2s开始对话
        yield return new WaitForSecondsRealtime(2f);
        // 对话1
        done = false;
        UIManager.Instance.ShowChat(dialogue1, () => done = true);
        yield return new WaitUntil(() => done);
        //等待1s开始对话2
        yield return new WaitForSecondsRealtime(1f);
        // 对话2 - 菊 渚月
        done = false;
        UIManager.Instance.ShowChat(dialogue2, girl1Name, girl1, () => done = true);
        yield return new WaitUntil(() => done);
        // 对话3 - 浅仓 凛凛子
        done = false;
        UIManager.Instance.ShowChat(dialogue3, girl2Name, girl2, () => done = true);
        yield return new WaitUntil(() => done);
        // 黑幕1秒并隐藏插图2
        done = false;
        UIManager.Instance.ShowCGPanel(blackScreen, 1f, () => done = true);
        yield return new WaitForSecondsRealtime(1f); // 等待1秒，确保黑幕显示出来
        UIManager.Instance.HideGameCGPanel();
        yield return new WaitUntil(() => done);
        done = false;
        // 玩家头顶气泡
        if (player != null)
        {
            Vector3 playerHead = player.transform.position + Vector3.up * 2f;
            UIManager.Instance.ShowSpeakPanel(finalSpeak, playerHead, () => done = true);
        }
        yield return new WaitUntil(() => done);
        //恢复
        Time.timeScale = 1f;

        UIManager.Instance.ShowTextTip(levelName);
        UIManager.Instance.ShowGameUI(true);
    }
}
