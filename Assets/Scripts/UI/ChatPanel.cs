using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using DG.Tweening;

/// <summary>
/// 对话面板，支持打字机效果、角色姓名框、角色立绘
/// </summary>
public class ChatPanel : BasePanel
{
    [Header("UI References")]
    public Text dialogueText;
    public RectTransform nameBox;
    public float nameBoxPadding = 40f;// 姓名框内边距,调试位置用
    public float widthK = 1.2f;//调试用
    public float transK = 1.1f;//调试用
    public float transOffsetX = -705f; // 姓名框初始 X 位置，调试用
    public Button continueButton;
    public Image maskImage;
    public Text nameText;
    public Image charaImage;

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;
    private string currentDialogue;
    private bool quickContinue = false;
    private Action onCompleteCallback;
    private Tween typingTween;// 打字机动画

    protected override void Awake()
    {
        base.Awake();
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClick);
        }
    }

    // 显示无角色名与立绘的对话(旁白)
    public void ShowDialogue(string dialogue,bool canSkip = true, Action onComplete = null)
    {
        ShowDialogue(dialogue, "", null, canSkip,false, onComplete);
    }

    // 显示完整对话（文本、姓名、立绘）
    public void ShowDialogue(string dialogue, string characterName, Sprite characterSprite, bool canSkip = true,bool quickNext=false, Action onComplete = null)
    {
        currentDialogue = dialogue;// 设置当前对话文本
        onCompleteCallback = onComplete;// 设置回调
        if(!quickNext) quickContinue = quickNext;
        // ----- 设置角色名字 -----
        nameText.text = characterName;
        bool hasName = !string.IsNullOrEmpty(characterName);
        nameText.gameObject.SetActive(hasName);// 根据是否有名字显示姓名name文本
        nameBox.gameObject.SetActive(hasName);// 根据是否有名字显示姓名框NameBox
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

        // ----- 设置立绘 -----
        if (charaImage != null)
        {
            charaImage.sprite = characterSprite;
            charaImage.gameObject.SetActive(characterSprite != null);
        }

        // 显示面板
        Show();

        // ----- 开始打字机 -----
        typingTween?.Kill();
        dialogueText.text = "";
        continueButton.interactable = canSkip;// 根据是否允许跳过设置继续按钮状态
        typingTween = dialogueText.DOText(currentDialogue, currentDialogue.Length * typingSpeed)
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                // 打字完成，允许点击继续
                if (continueButton != null)
                {
                    continueButton.interactable = true;
                }
            });
    }

    private void OnContinueClick()
    {
        // 如果打字未完成，立即完成
        if (typingTween != null && typingTween.IsActive() && !typingTween.IsComplete())
        {
            typingTween.Complete();
            return;
        }
        if(!quickContinue)
        {
            Hide();
        }
        else
        {
            currentTween?.Kill();
            isShowing = false;
            canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }
        onCompleteCallback?.Invoke();
        onCompleteCallback = null;
        quickContinue = true;
        Debug.Log("是否快速继续标志已重置！");
    }

    protected override void OnShow()
    {
        // 显示遮罩
        if (maskImage != null)
        {
            maskImage.gameObject.SetActive(true);
            CanvasGroup maskGP = maskImage.GetComponent<CanvasGroup>();
            if(maskGP==null) maskGP = maskImage.gameObject.AddComponent<CanvasGroup>();
            maskGP.alpha = 0;
            maskGP.DOFade(1f, fadeTime).SetUpdate(true);
        }
    }
    protected override void OnHide()
    {
        if(maskImage!=null)
        {
            CanvasGroup maskGP = maskImage.GetComponent<CanvasGroup>();
            if(maskGP!=null)
            {
                maskGP.DOFade(0f, fadeTime).SetUpdate(true)
                    .OnComplete(() => maskImage.gameObject.SetActive(false));
            }
            else
            {
                maskImage.gameObject.SetActive(false);
            }
        }
        typingTween?.Kill();
    }
}
