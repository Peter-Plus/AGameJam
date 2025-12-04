using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Level2Manager : LevelManager
{
    [Header("关卡2配置")]
    public string levelName = "夜晚的河边";

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
    // 资源
    private Sprite blackScreen;
    private Sprite illustration1;
    private Sprite illustration2;
    private Sprite girl1;
    private Sprite girl2;

    private GameObject player;
    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player");
        // 加载资源
        blackScreen = Resources.Load<Sprite>("CG/BlackScreen");
        illustration1 = Resources.Load<Sprite>("CG/Illustration1");
        illustration2 = Resources.Load<Sprite>("CG/Illustration2");
        girl1 = Resources.Load<Sprite>("Texture/UI/AIChara/girl1");
        girl2 = Resources.Load<Sprite>("Texture/UI/AIChara/girl2");
    }
    private void Start()
    {
        //判断是不是第一次进入该关卡
        if (DataManager.Instance.IsFirstTimeReachLevel(2))
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
        //开始交互
        SetUIInteract(true);

        bool done = false;
        skipRequested = false; //标记判断用
        //黑幕1s
        UIManager.Instance.ShowCGPanelInstant(blackScreen, 1f);
        // 插图1
        UIManager.Instance.ShowGameCGPanelInstant(illustration1);
        // 等待2s开始对话
        yield return WaitForSecondsOrSkip(2f, () => skipRequested);
        // 对话1（旁白）
        done = false;
        UIManager.Instance.ShowChat(dialogue1, false, () => done = true);
        yield return new WaitUntil(() => done || skipRequested);
        //旁白结束1s后开始对话2
        yield return WaitForSecondsOrSkip(1f, () => skipRequested);
        // 对话2 - 菊 渚月
        done = false;
        UIManager.Instance.ShowChat(dialogue2, girl1Name, girl1, true, true, () => done = true);
        yield return new WaitUntil(() => done || skipRequested);
        // 对话3 - 浅仓 凛凛子
        done = false;
        UIManager.Instance.ShowChat(dialogue3, girl2Name, girl2, true, false, () => done = true);
        yield return new WaitUntil(() => done || skipRequested);
        done = false;
        // 黑幕1+0.5+1s
        UIManager.Instance.ShowCGPanel(blackScreen, 0.5f, () => done = true);
        // 等待1s
        yield return WaitForSecondsOrSkip(1f, () => skipRequested);
        // 隐藏插图1
        UIManager.Instance.HideGameCGPanelInstant();
        // 显示插图2
        UIManager.Instance.ShowGameCGPanelInstant(illustration2);
        // 黑幕消失后等待0.5s开始对话4
        yield return new WaitUntil(() => done || skipRequested);
        yield return WaitForSecondsOrSkip(0.5f, () => skipRequested);
        // 对话4
        done = false;
        UIManager.Instance.ShowChat(dialogue1, true, () => done = true);
        yield return new WaitUntil(() => done || skipRequested);
        // 黑幕1+0.5+1秒并隐藏插图2
        done = false;
        UIManager.Instance.ShowCGPanel(blackScreen, 0.5f, () => done = true);
        // 等待1秒，确保黑幕完全显示出来
        yield return WaitForSecondsOrSkip(1f, () => skipRequested);
        UIManager.Instance.HideGameCGPanelInstant();
        // 黑幕消失后等待0.5s开始最后气泡
        yield return new WaitUntil(() => done || skipRequested);
        yield return WaitForSecondsOrSkip(0.5f, () => skipRequested);
        // 最后气泡
        done = false;
        if (player != null)
        {
            Vector3 playerHead = player.transform.position + Vector3.up * 2f;
            UIManager.Instance.ShowSpeakPanel(finalSpeak, playerHead, () => done = true);
        }
        yield return new WaitUntil(() => done || skipRequested);
        if (skipRequested)
        {
            UIManager.Instance.HideAllUIInstant();
        }
        // 恢复游戏
        SetUIInteract(false);
        UIManager.Instance.ShowTextTip(levelName);
        UIManager.Instance.ShowGameUI(true);
    }
}
