using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Product : MonoBehaviour
{
    public Text NameText, StockText;
    public Button InStockBtn, OutStockBtn, DeleteBtn;
    public int Stock;
    void Start()
    {
        Init();
    }

    void Init()
    {
        InStockBtn.onClick.AddListener(OnInStock);
        OutStockBtn.onClick.AddListener(OnOutStock);
        DeleteBtn.onClick.AddListener(OnDelete);
    }



    //入库
    void OnInStock()
    {
        ConfirmPanel.Instance.MyShowConfirmPanel(PanelMode.InStock, NameText.text, (product, num) =>
        {
            Stock += num;
            StockText.text = Stock.ToString();
        });
    }

    //出库
    void OnOutStock()
    {
        ConfirmPanel.Instance.MyShowConfirmPanel(PanelMode.OutStock, NameText.text, (product, num) =>
        {
            Stock -= num;
            StockText.text = Stock.ToString();
        });
    }

    //删除
    void OnDelete()
    {
        ConfirmPanel.Instance.MyShowConfirmPanel(PanelMode.Delete, NameText.text, (product, num) =>
        {
            Destroy(gameObject);
        });
    }

    //设置商品信息UI
    public void MySetProductInfo(string name, int quantity)
    {
        transform.name = NameText.text = name;
        Stock = quantity;
        StockText.text = quantity.ToString();
    }
}
