using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPanel : MonoBehaviour
{
    public static ConfirmPanel Instance;

    [Header("UI元素")]
    [Tooltip("标题")]
    public Text TitleText;
    public InputField InputField1, InputField2;
    Text PlaceholderText1, PlaceholderText2;
    [Tooltip("确认按钮")]
    public Button ConfirmBtn;
    [Tooltip("取消按钮")]
    public Button CancelBtn;

    //回调（由外部传入）
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
        //执行回调
        onConfirmCallback?.Invoke(product, num);
        gameObject.SetActive(false);
    }

    //打开确认面板
    public void MyShowConfirmPanel(PanelMode mode, string product, Action<string, int> onConfirm)
    {
        onConfirmCallback = onConfirm;

        switch (mode)
        {
            case PanelMode.None:
                break;
            case PanelMode.AddProduct:
                SetPanelInfo(true, "新增商品", "请输入商品名称", "请输入商品数量", false);
                InputField1.text = string.Empty;
                InputField1.Select();
                break;
            case PanelMode.InStock:
                SetPanelInfo(false, "商品入库", product, "请输入入库数量", false);
                InputField2.Select();
                break;
            case PanelMode.OutStock:
                SetPanelInfo(false, "商品出库", product, "请输入出库数量", false);
                InputField2.Select();
                break;
            case PanelMode.Delete:
                SetPanelInfo(false, "删除商品", product, "确认删除该商品吗？", true);
                break;
            default:
                break;
        }

        //设置面板信息
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
    [Tooltip("默认（无操作）")]
    None,
    [Tooltip("新增商品")]
    AddProduct,
    [Tooltip("入库")]
    InStock,
    [Tooltip("出库")]
    OutStock,
    [Tooltip("删除")]
    Delete
}