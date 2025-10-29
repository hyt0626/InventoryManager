using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPanel : MonoBehaviour
{
    public static ConfirmPanel Instance;

    [Header("UIԪ��")]
    [Tooltip("����")]
    public Text TitleText;
    public InputField InputField1, InputField2;
    Text PlaceholderText1, PlaceholderText2;
    [Tooltip("ȷ�ϰ�ť")]
    public Button ConfirmBtn;
    [Tooltip("ȡ����ť")]
    public Button CancelBtn;

    //�ص������ⲿ���룩
    private Action<string, int> onConfirmCallback;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    void Start()
    {
        Init();
        gameObject.SetActive(false);
    }

    void Update()
    {
        TabChange();
        EnterConfirm();
    }

    void TabChange()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (InputField1.isFocused && InputField2.interactable)
            {
                InputField2.Select();
            }
            else if (InputField2.isFocused && InputField1.interactable)
            {
                InputField1.Select();
            }
        }
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
        PlaceholderText1 = InputField1.placeholder.GetComponent<Text>();
        PlaceholderText2 = InputField2.placeholder.GetComponent<Text>();

        CancelBtn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        ConfirmBtn.onClick.AddListener(OnConfirm);
    }

    private void OnConfirm()
    {
        string product = InputField1.text;
        int.TryParse(InputField2.text, out int num);
        //ִ�лص�
        onConfirmCallback?.Invoke(product, num);
        gameObject.SetActive(false);
    }

    //��ȷ�����
    public void MyShowConfirmPanel(PanelMode mode, string product, Action<string, int> onConfirm)
    {
        onConfirmCallback = onConfirm;

        switch (mode)
        {
            case PanelMode.None:
                break;
            case PanelMode.AddProduct:
                SetPanelInfo(true, "������Ʒ", "��������Ʒ����", "��������Ʒ����", false);
                InputField1.text = string.Empty;
                InputField1.Select();
                break;
            case PanelMode.InStock:
                SetPanelInfo(false, "��Ʒ���", product, "�������������", false);
                InputField2.Select();
                break;
            case PanelMode.OutStock:
                SetPanelInfo(false, "��Ʒ����", product, "�������������", false);
                InputField2.Select();
                break;
            case PanelMode.Delete:
                SetPanelInfo(false, "ɾ����Ʒ", product, "ȷ��ɾ������Ʒ��", true);
                break;
            default:
                break;
        }

        //���������Ϣ
        void SetPanelInfo(bool isAddProduct, string title, string product, string placeholder2, bool isDelete)
        {
            InputField1.interactable = isAddProduct;
            InputField2.interactable = !isDelete;
            TitleText.text = title;
            PlaceholderText1.text = product;
            PlaceholderText2.text = placeholder2;
            InputField2.text = string.Empty;

            gameObject.SetActive(true);
        }
    }
}
public enum PanelMode
{
    [Tooltip("Ĭ�ϣ��޲�����")]
    None,
    [Tooltip("������Ʒ")]
    AddProduct,
    [Tooltip("���")]
    InStock,
    [Tooltip("����")]
    OutStock,
    [Tooltip("ɾ��")]
    Delete
}