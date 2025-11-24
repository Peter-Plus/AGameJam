using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// 对话框面板，支持打字机效果、角色名、角色立绘
/// </summary>
public class ChatPanel : BasePanel
{
    [Header("UI References")]
    [Tooltip("对话文本")]
    public Text dialogueText;

    [Tooltip("角色名背景框")]
    public RectTransform nameBox;
    public float nameBoxPadding = 40f;
    public float widthK = 1.2f;
    public float transK = 1.1f;
    public float transOffsetX = -705f;//初始位置X坐标

    [Tooltip("角色名文本（可选）")]
    public Text nameText;

    [Tooltip("角色立绘（可选）")]
    public Image charaImage;

    [Tooltip("继续按钮")]
    public Button continueButton;

    [Tooltip("遮罩（可选，防止误触）")]
    public Image maskImage;

    [Header("Typing Settings")]
    [Tooltip("打字机速度")]
    public float typingSpeed = 0.05f;

    private string currentDialogue;
    private Action onCompleteCallback;
    private Coroutine typingCoroutine;

    protected override void Awake()
    {
        base.Awake();

        // 绑定按钮事件
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClick);
        }
    }

    /// <summary>
    /// 显示对话（无角色名和立绘）
    /// </summary>
    public void ShowDialogue(string dialogue, Action onComplete = null)
    {
        ShowDialogue(dialogue, "", null, onComplete);
    }

    /// <summary>
    /// 显示对话（带角色名）
    /// </summary>
    public void ShowDialogue(string dialogue, string characterName, Action onComplete = null)
    {
        ShowDialogue(dialogue, characterName, null, onComplete);
    }

    /// <summary>
    /// 显示对话（完整版）
    /// </summary>
    public void ShowDialogue(string dialogue, string characterName, Sprite characterSprite, Action onComplete = null)
    {
        currentDialogue = dialogue;
        onCompleteCallback = onComplete;

        // 设置角色名
        if (nameText != null)
        {
            //有角色名则显示角色名框
            nameText.text = characterName;
            bool hasName = !string.IsNullOrEmpty(characterName);//还需要判断是否为空字符串
            nameText.gameObject.SetActive(!string.IsNullOrEmpty(characterName));

            if(nameBox != null)
            {
                nameBox.gameObject.SetActive(hasName);
                if (hasName)
                {
                    Canvas.ForceUpdateCanvases(); // 强制更新布局
                    float nameWidth = nameText.preferredWidth + nameBoxPadding; // 额外留点空间
                    nameBox.sizeDelta = new Vector2(nameWidth*widthK, nameBox.sizeDelta.y);
                    //文字左对齐故还需要NameBox右移一定距离
                    //nameBox.anchoredPosition = new Vector2(nameWidth / 2f*transK, nameBox.anchoredPosition.y);
                    //直接改变X坐标
                    Vector2 pos = nameBox.anchoredPosition;
                    pos.x = transOffsetX + (nameWidth / 2f) * transK;
                    nameBox.anchoredPosition = pos;
                }

            }
        }

        // 设置角色立绘
        if (charaImage != null)
        {
            charaImage.sprite = characterSprite;
            charaImage.gameObject.SetActive(characterSprite != null);
        }
        Show();
        // 开始打字机效果
        if (dialogueText != null)
        {
            dialogueText.text = "";
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeText());
        }

        // 禁用继续按钮（打字机结束后启用）
        if (continueButton != null)
        {
            continueButton.interactable = false;
        }

        
    }

    /// <summary>
    /// 打字机效果协程
    /// </summary>
    private IEnumerator TypeText()
    {
        foreach (char c in currentDialogue)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        // 打字完成，启用继续按钮
        if (continueButton != null)
        {
            continueButton.interactable = true;
        }
    }

    /// <summary>
    /// 点击继续按钮
    /// </summary>
    private void OnContinueClick()
    {
        // 如果还在打字，直接显示全部文本
        if (typingCoroutine != null && dialogueText.text.Length < currentDialogue.Length)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentDialogue;
            continueButton.interactable = true;
            return;
        }

        // 关闭对话框
        Hide();

        // 触发回调
        onCompleteCallback?.Invoke();
        onCompleteCallback = null;
    }

    protected override void OnShow()
    {
        // 启用遮罩（如果有）
        if (maskImage != null)
        {
            maskImage.gameObject.SetActive(true);
        }
    }

    protected override void OnHide()
    {
        // 禁用遮罩
        if (maskImage != null)
        {
            maskImage.gameObject.SetActive(false);
        }

        // 停止打字机
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }
}