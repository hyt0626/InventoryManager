using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;

public class DataTableManager
{
    /// <summary>
    /// 从 CSV 文件读取数据到 DataTable。
    /// </summary>
    private DataTable OpenCSV(string filePath)
    {
        DataTable dt = new DataTable();

        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
        {
            string line;
            string[] headers = null;
            bool isFirst = true;

            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] values = line.Split(',');

                if (isFirst)
                {
                    headers = values;
                    foreach (var h in headers)
                        dt.Columns.Add(new DataColumn(h.Trim()));

                    isFirst = false;
                }
                else
                {
                    if (values.Length != headers.Length)
                        continue;

                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                        dr[i] = values[i].Trim();
                    dt.Rows.Add(dr);
                }
            }
        }

        return dt;
    }

    /// <summary>
    /// 从 CSV 文件读取并转换为 List<T>。
    /// 表头列名需与 T 的字段或属性名一致（忽略大小写）。
    /// </summary>
    public List<T> MyLoadCSV<T>(string filePath) where T : new()
    {
        DataTable dt = OpenCSV(filePath);
        List<T> list = new List<T>();

        if (dt == null || dt.Rows.Count == 0)
            return list;

        Type type = typeof(T);
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (DataRow row in dt.Rows)
        {
            T obj = new T();

            // 匹配字段
            foreach (var f in fields)
            {
                if (!dt.Columns.Contains(f.Name)) continue;
                string value = row[f.Name].ToString();
                f.SetValue(obj, ConvertValue(value, f.FieldType));
            }

            // 匹配属性
            foreach (var p in props)
            {
                if (!p.CanWrite || !dt.Columns.Contains(p.Name)) continue;
                string value = row[p.Name].ToString();
                p.SetValue(obj, ConvertValue(value, p.PropertyType));
            }

            list.Add(obj);
        }

        return list;
    }

    /// <summary>
    /// 写入 CSV 文件（覆盖模式）
    /// 无论是否有数据，都会生成有效 CSV（至少包含表头）
    /// </summary>
    public void MyWriteCSV<T>(string filePath, List<T> dataList)
    {
        Type type = typeof(T);
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            // 始终写入表头
            List<string> headers = new List<string>();
            foreach (var f in fields) headers.Add(f.Name);
            foreach (var p in props) headers.Add(p.Name);
            sw.WriteLine(string.Join(",", headers));

            // 如果有数据，则写入内容
            if (dataList != null && dataList.Count > 0)
            {
                foreach (var item in dataList)
                {
                    List<string> values = new List<string>();

                    foreach (var f in fields)
                    {
                        object val = f.GetValue(item);
                        values.Add(EscapeCSV(val?.ToString() ?? ""));
                    }

                    foreach (var p in props)
                    {
                        object val = p.GetValue(item, null);
                        values.Add(EscapeCSV(val?.ToString() ?? ""));
                    }

                    sw.WriteLine(string.Join(",", values));
                }
            }
            else
            {
                // 仅打印提示，不中断写入
                Console.WriteLine($"数据为空，仅生成表头：{filePath}");
            }
        }

        Console.WriteLine($"已写入 CSV 文件：{filePath}");
    }


    /// <summary>
    /// 自动类型转换
    /// </summary>
    private object ConvertValue(string value, Type type)
    {
        try
        {
            if (type == typeof(string))
                return value;
            if (type == typeof(int))
                return int.Parse(value);
            if (type == typeof(float))
                return float.Parse(value);
            if (type == typeof(double))
                return double.Parse(value);
            if (type == typeof(bool))
                return value == "1" || value.ToLower() == "true";
            if (type.IsEnum)
                return Enum.Parse(type, value);
            return Convert.ChangeType(value, type);
        }
        catch
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }

    /// <summary>
    /// 处理CSV中的特殊字符（逗号、引号等）
    /// </summary>
    private string EscapeCSV(string value)
    {
        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
        {
            value = value.Replace("\"", "\"\"");
            return $"\"{value}\"";
        }
        return value;
    }
}
