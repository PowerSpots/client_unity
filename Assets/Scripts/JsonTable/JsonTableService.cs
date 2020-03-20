#if UNITY_EDITOR

using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class JsonTableService
{
    private static readonly string ConstDataFolder = Application.dataPath + "/../../Excel/Excel2Json_Python/output/json/";
    private static readonly Dictionary<Type, object> TableDataMap = new Dictionary<Type, object>();

    public static List<T> Load<T>(string filePath) where T : JsonItemData
    {
        var jsonStr = ReadJsonFile(filePath);
        if (string.IsNullOrEmpty(jsonStr))
        {
            return null;
        }

        var items = JsonMapper.ToObject<List<T>>(jsonStr);

        if (!TableDataMap.ContainsKey(typeof (T)))
        {
            TableDataMap.Add(typeof (T), items);
        }

        return TableDataMap[typeof (T)] as List<T>;
    }

    public static List<T> GetItems<T>() where T : JsonItemData
    {
        if (TableDataMap.ContainsKey(typeof(T))) return TableDataMap[typeof(T)] as List<T>;
        var fileName = GetFilePath(typeof (T));
        Load<T>(fileName);
        return TableDataMap[typeof (T)] as List<T>;
    }


    public static T GetItem<T>(string keyName, int value) where T : JsonItemData
    {
        return GetItem<T, int>(keyName, value);
    }

    public static T GetItem<T>(string keyName, string value) where T : JsonItemData
    {
        return GetItem<T, string>(keyName, value);
    }

    public static T GetItem<T, T2>(string keyName, T2 value) where T : JsonItemData
    {
        var items = GetItems<T>();
        if (items == null)
        {
            return null;
        }
        
        var propertyInfo = typeof(T).GetField(keyName);
        return propertyInfo == null? null : (from item in items let temp = (T2) propertyInfo.GetValue(item) where temp.Equals(value) select item).FirstOrDefault();
    }


    public static List<T> GetItems<T>(string keyName, int value) where T : JsonItemData
    {
        return GetItems<T, int>(keyName,value);
    }

    public static List<T> GetItems<T>(string keyName, string value) where T : JsonItemData
    {
        return GetItems<T, string>(keyName, value);
    }

    public static List<T2> GetItems<T, T2>(string keyName) where T : JsonItemData
    {
        var items = GetItems<T>();
        if (items == null)
        {
            return null;
        }

        var propertyInfo = typeof(T).GetField(keyName);
        return propertyInfo == null ? null : items.Select(item => (T2) propertyInfo.GetValue(item)).ToList();
    }


    public static List<T> GetItems<T , T2>(string keyName, T2 value) where T : JsonItemData 
    {
        var items = GetItems<T>();
        if (items == null)
        {
            return null;
        }

        var propertyInfo = typeof(T).GetField(keyName);
        return propertyInfo == null ? null : (from item in items let temp = (T2) propertyInfo.GetValue(item) where temp.Equals(value) select item).ToList();
    }


    public delegate bool JsonItemFilter(object item);

    public static List<T> GetItems<T>(JsonItemFilter compareMethod ) where T : JsonItemData
    {
        var items = GetItems<T>();
        return items == null ? null : items.Where(item => compareMethod(item)).ToList();
    }

    public static string GetFilePath(Type t)
    {
        var fileName = t.Name;
        fileName = fileName.Substring(fileName.IndexOf("JsonObejct_", StringComparison.Ordinal) + "JsonObejct_".Length);
        return ConstDataFolder + fileName + ".json";
    }

    private static string ReadJsonFile(string filePath)
    {
        var sr1 = new StreamReader(filePath);
        var jsonStr = sr1.ReadToEnd();
        sr1.Close();

        return jsonStr;
    }
}
#endif