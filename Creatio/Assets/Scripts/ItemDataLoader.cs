using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ItemDataLoader : MonoBehaviour
{
    public static string jsonFilePath = "Assets/Resources/JSON Files/ItemData.json";
    public static GameObject itemPrefab;
    public static Dictionary<int, ItemData> itemDataDictionary;

    void Start()
    {
        LoadItemData();
    }

    void LoadItemData()
    {
        string json = File.ReadAllText(jsonFilePath);
        ItemDataList itemList = JsonUtility.FromJson<ItemDataList>("{\"items\":" + json + "}");

        itemDataDictionary = new Dictionary<int, ItemData>();
        foreach (ItemData item in itemList.items)
        {
            itemDataDictionary[item.id] = item;
        }
    }
    public static ItemData GetItemData(int id)
    {
        itemDataDictionary.TryGetValue(id, out ItemData itemData);
        return itemData;
    }
}

[System.Serializable]
public class ItemData
{
    public int id;
    public string sprite;
    public int stackAmount;
    public string name;
    public string description;
}

[System.Serializable]
public class ItemDataList
{
    public ItemData[] items;
}