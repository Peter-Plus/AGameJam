using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// 对话面板，支持打字机效果、角色姓名框、角色立绘
/// </summary>
public class ChatPanel : BasePanel
{
    [Header("UI References")]
    [Tooltip("对话文本")]
    public Text dialogueText;

    [Tooltip("角色名字的底框图")]
    public RectTransform nameBox;
    public float nameBoxPadding = 40f;
    public float widthK = 1.2f;
    public float transK = 1.1f;
    public float transOffsetX = -705f; // 姓名框初始 X 位置

    [Tooltip("角色名字文本（可选）")]
    public Text nameText;

    [Tooltip("角色立绘（可选）")]
    public Image charaImage;

    [Tooltip("继续按钮")]
    public Button continueButton;

    [Tooltip("遮罩，用于禁止点击下层UI（可选）")]
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

        // 绑定按钮点击事件
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClick);
        }
    }

    /// <summary>
    /// 显示无角色名与立绘的对话
    /// </summary>
    public void ShowDialogue(string dialogue, Action onComplete = null)
    {
        ShowDialogue(dialogue, "", null, onComplete);
    }

    /// <summary>
    /// 显示带角色名的对话
    /// </summary>
    public void ShowDialogue(string dialogue, string characterName, Action onComplete = null)
    {
        ShowDialogue(dialogue, characterName, null, onComplete);
    }

    /// <summary>
    /// 显示完整对话（文本、姓名、立绘）
    /// </summary>
    public void ShowDialogue(string dialogue, string characterName, Sprite characterSprite, Action onComplete = null)
    {
        currentDialogue = dialogue;
        onCompleteCallback = onComplete;

        // ----- 设置角色名字 -----
        if (nameText != null)
        {
            nameText.text = characterName;
            bool hasName = !string.IsNullOrEmpty(characterName);
            nameText.gameObject.SetActive(hasName);

            if (nameBox != null)
            {
                nameBox.gameObject.SetActive(hasName);

                if (hasName)
                {
                    Canvas.ForceUpdateCanvases(); // 强制更新布局

                    float nameWidth = nameText.preferredWidth + nameBoxPadding;
                    nameBox.sizeDelta = new Vector2(nameWidth * widthK, nameBox.sizeDelta.y);

                    // 修改姓名框X坐标
                    Vector2 pos = nameBox.anchoredPosition;
                    pos.x = transOffsetX + (nameWidth / 2f) * transK;
                    nameBox.anchoredPosition = pos;
                }
            }
        }

        // ----- 设置立绘 -----
        if (charaImage != null)
        {
            charaImage.sprite = characterSprite;
            charaImage.gameObject.SetActive(characterSprite != null);
        }

        // 显示面板
        Show();

        // ----- 开始打字机 -----
        if (dialogueText != null)
        {
            dialogueText.text = "";
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeText());
        }

        // 在打字期间不允许点击继续
        if (continueButton != null)
        {
            continueButton.interactable = false;
        }
    }

    /// <summary>
    /// 打字机效果
    /// </summary>
    private IEnumerator TypeText()
    {
        foreach (char c in currentDialogue)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        // 打字完成，允许点击继续
        if (continueButton != null)
        {
            continueButton.interactable = true;
        }
    }

    /// <summary>
    /// 继续按钮逻辑
    /// </summary>
    private void OnContinueClick()
    {
        // 如果打字未完成，则立即显示完整文本
        if (typingCoroutine != null && dialogueText.text.Length < currentDialogue.Length)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentDialogue;
            continueButton.interactable = true;
            return;
        }

        // 关闭面板
        Hide();

        // 回调
        onCompleteCallback?.Invoke();
        onCompleteCallback = null;
    }

    protected override void OnShow()
    {
        // 显示遮罩
        if (maskImage != null)
        {
            maskImage.gameObject.SetActive(true);
        }
    }

    protected override void OnHide()
    {
        // 隐藏遮罩
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
