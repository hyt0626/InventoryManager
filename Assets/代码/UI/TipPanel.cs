using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : MonoBehaviour
{
    public static TipPanel Instance;

    [Header("UI 元素")]
    public Text TitleText;
    public Text MessageText;
    public Button ConfirmBtn;

    [Tooltip("是否启用自动关闭（秒）")]
    public float autoCloseTime = 0f;

    private Coroutine autoCloseCoroutine;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        gameObject.SetActive(false);
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        EnterConfirm();
    }

    void EnterConfirm()
    {
        // 检测按下 Enter 键
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // 只有面板激活时才执行
            if (gameObject.activeSelf)
            {
                ConfirmBtn.onClick.Invoke();
            }
        }
    }

    void Init()
    {
        if (ConfirmBtn != null)
        {
            ConfirmBtn.onClick.AddListener(() =>
            {
                Hide();
            });
        }
    }
    /// <summary>
    /// 显示提示面板
    /// </summary>
    /// <param name="message">要显示的消息内容</param>
    /// <param name="title">标题（可选）</param>
    /// <param name="autoCloseSeconds">自动关闭时间（0表示不自动关闭）</param>
    public void MyShowMessage(string message, string title = "提示面板", float autoCloseSeconds = 0f)
    {
        if (TitleText != null)
            TitleText.text = title;

        if (MessageText != null)
            MessageText.text = message;

        gameObject.SetActive(true);

        // 如果之前有协程，先停止
        if (autoCloseCoroutine != null)
            StopCoroutine(autoCloseCoroutine);

        // 启动自动关闭
        if (autoCloseSeconds > 0)
        {
            autoCloseCoroutine = StartCoroutine(AutoClose(autoCloseSeconds));
        }
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    IEnumerator AutoClose(float time)
    {
        yield return new WaitForSeconds(time);
        Hide();
    }
}
