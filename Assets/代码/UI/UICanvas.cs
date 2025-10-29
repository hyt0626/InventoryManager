using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour
{
    [Tooltip("增加商品按钮")]
    public Button AddProductBtn;

    [Tooltip("所有商品的父物体 UI（ScrollView 内容节点）")]
    public Transform ContentProduct;


    [Tooltip("用于管理 CSV 表格数据的脚本")]
    private DataTableManager dataTableManager = new();

    [Tooltip("商品数据保存目录（相对项目根目录）")]
    private string productsDir = "ProductsData";
    private List<Goods> products = new();
    void Start()
    {
        Init();
    }

    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        SaveProductsData();
    }

    void Init()
    {
        //确保商品数据目录存在
        if (!Directory.Exists(productsDir))
        {
            Directory.CreateDirectory(productsDir);
        }

        //启动时清理旧文件，只保留最新200个
        ClearOldProductFiles(200);

        //加载最新数据
        LoadProductsData();

        //新增商品按钮监听
        AddProductBtn.onClick.AddListener(() =>
        {
            ConfirmPanel.Instance.MyShowConfirmPanel(PanelMode.AddProduct, null, (name, quantity) =>
            {
                // 1️⃣ 检查是否重名（忽略大小写）
                bool exists = false;
                for (int i = 1; i < ContentProduct.childCount; i++)
                {
                    var existing = ContentProduct.GetChild(i).GetComponent<Product>();
                    if (existing != null && existing.NameText.text.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        exists = true;
                        break;
                    }
                }
                if (exists)
                {
                    Debug.Log($"已存在同名商品：{name}，请使用其他名称！");
                    TipPanel.Instance.MyShowMessage($"已存在同名商品：“{name}”，请使用其他名称！", "添加商品失败");
                    return;
                }

                Debug.Log($"新增商品：{name} 数量：{quantity}");
                var product = LoadPrefabWithScript<Product>("商品", ContentProduct);
                product.transform.SetSiblingIndex(ContentProduct.childCount - 2);
                if (product != null)
                {
                    product.MySetProductInfo(name, quantity);
                }
            });
        });
    }

    /// <summary>
    /// 添加预制体并获得其组件
    /// </summary>
    private T LoadPrefabWithScript<T>(string prefab, Transform parent = null) where T : Component
    {
        var pre = Resources.Load<GameObject>(prefab);
        if (pre == null)
        {
            Debug.LogError($"未找到预制体：{prefab}");
            return null;
        }
        var temp = Instantiate(pre);
        if (parent != null)
        {
            temp.transform.SetParent(parent, false);
        }
        T script = temp.GetComponent<T>();
        if (script == null)
        {
            Debug.LogError($"预制体上未找到脚本组件：{typeof(T).Name}");
        }
        return script;
    }

    /// <summary>
    /// 保存商品数据到 CSV 文件
    /// </summary>
    void SaveProductsData()
    {
        Debug.Log("程序即将退出，保存商品数据...");

        products.Clear();

        // 1️⃣ 遍历父物体下所有商品预制体 （跳过第0个）
        for (int i = 1; i < ContentProduct.childCount; i++)
        {
            var product = ContentProduct.GetChild(i).GetComponent<Product>();
            if (product != null)
            {
                // 2️⃣ 转换成 Goods 数据结构
                Goods goods = new Goods
                {
                    Name = product.NameText.text,
                    Stock = product.Stock,
                    InStock = 0,
                    OutStock = 0
                };
                products.Add(goods);
            }
        }

        // 生成带日期时间的文件名
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"Products_{timestamp}.csv";
        string fullPath = Path.Combine(productsDir, fileName);

        // 写入 CSV
        dataTableManager.MyWriteCSV(fullPath, products);

        Debug.Log($"商品数据已写入：{fullPath}  {products.Count}");
    }

    //加载商品数据显示到UI
    void LoadProductsData()
    {
        // 1️⃣ 获取所有 CSV 文件
        string[] csvFiles = Directory.GetFiles(productsDir, "Products_*.csv");

        if (csvFiles.Length == 0)
        {
            Debug.Log("未找到任何商品数据文件");
            return;
        }

        //按修改时间排序，选择最新的文件
        Array.Sort(csvFiles, (a, b) => File.GetLastWriteTime(b).CompareTo(File.GetLastWriteTime(a)));
        string latestFile = csvFiles[0];
        Debug.Log($"加载最新商品数据：{Path.GetFileName(latestFile)}");

        products = dataTableManager.MyLoadCSV<Goods>(latestFile);
        // 根据数据生成商品 UI
        foreach (var g in products)
        {
            var product = LoadPrefabWithScript<Product>("商品", ContentProduct);
            product.transform.SetSiblingIndex(ContentProduct.childCount - 2);
            product.MySetProductInfo(g.Name, g.Stock);
        }
    }

    //清理旧的商品数据文件，只保留最新N个
    void ClearOldProductFiles(int keepCount)
    {
        try
        {
            string[] files = Directory.GetFiles(productsDir, "Products_*.csv");
            if (files.Length <= keepCount)
            {
                Debug.Log($"当前共有 {files.Length} 个文件，无需清理。");
                return;
            }

            // 按最后修改时间升序排序（最旧在前）
            Array.Sort(files, (a, b) => File.GetLastWriteTime(a).CompareTo(File.GetLastWriteTime(b)));

            int deleteCount = files.Length - keepCount;
            for (int i = 0; i < deleteCount; i++)
            {
                File.Delete(files[i]);
                Debug.Log($"已删除旧数据文件：{Path.GetFileName(files[i])}");
            }

            Debug.Log($"已清理旧文件，仅保留最新 {keepCount} 个。");
        }
        catch (Exception ex)
        {
            Debug.LogError($"清理旧文件失败：{ex.Message}");
        }
    }
}
public class Goods
{
    public string Name;
    public int Stock;
    public int InStock;
    public int OutStock;
}