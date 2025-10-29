using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : MonoBehaviour
{
    public static TipPanel Instance;

    [Header("UI Ԫ��")]
    public Text TitleText;
    public Text MessageText;
    public Button ConfirmBtn;

    [Tooltip("�Ƿ������Զ��رգ��룩")]
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
        // ��ⰴ�� Enter ��
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // ֻ����弤��ʱ��ִ��
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
    /// ��ʾ��ʾ���
    /// </summary>
    /// <param name="message">Ҫ��ʾ����Ϣ����</param>
    /// <param name="title">���⣨��ѡ��</param>
    /// <param name="autoCloseSeconds">�Զ��ر�ʱ�䣨0��ʾ���Զ��رգ�</param>
    public void MyShowMessage(string message, string title = "��ʾ���", float autoCloseSeconds = 0f)
    {
        if (TitleText != null)
            TitleText.text = title;

        if (MessageText != null)
            MessageText.text = message;

        gameObject.SetActive(true);

        // ���֮ǰ��Э�̣���ֹͣ
        if (autoCloseCoroutine != null)
            StopCoroutine(autoCloseCoroutine);

        // �����Զ��ر�
        if (autoCloseSeconds > 0)
        {
            autoCloseCoroutine = StartCoroutine(AutoClose(autoCloseSeconds));
        }
    }

    /// <summary>
    /// �������
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
