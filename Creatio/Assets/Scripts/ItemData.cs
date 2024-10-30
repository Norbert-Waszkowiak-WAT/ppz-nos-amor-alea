using System.Collections.Generic;
using UnityEngine;

// TODO
// fix json parsing - last copilot message

public class ItemData
{
    public string spritePath;
    public string description;
    public int value;
    public Sprite sprite;

    private static Dictionary<string, ItemData> itemDataDictionary;
    private static bool isLoaded = false;

    public static Dictionary<string, ItemData> LoadItemData()
    {
        if (!isLoaded)
        {
            itemDataDictionary = new Dictionary<string, ItemData>();
            TextAsset jsonText = Resources.Load<TextAsset>("ItemData");

            if (jsonText != null)
            {
                Dictionary<string, ItemData> tempData = JsonUtility.FromJson<Dictionary<string, ItemData>>(jsonText.text);
                foreach (var kvp in tempData)
                {
                    ItemData itemData = kvp.Value;
                    Debug.Log($"Loading sprite for itemID: {kvp.Key}, path: {itemData.spritePath}");
                    itemData.sprite = Resources.Load<Sprite>(itemData.spritePath);
                    if (itemData.sprite != null)
                    {
                        Debug.Log($"Sprite loaded successfully at path: {itemData.spritePath}");
                        itemDataDictionary[kvp.Key] = itemData;
                    }
                    else
                    {
                        Debug.LogWarning($"Sprite not found at path: {itemData.spritePath}");
                    }
                }
                Debug.Log("ItemData loaded successfully.");
                isLoaded = true;
            }
            else
            {
                Debug.LogError("ItemData.json not found in Resources folder.");
            }
        }

        return itemDataDictionary;
    }
}