using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class SmelterRecipe
{
    public int recipeId;
    public int inputId;
    public int inputAmount;
    public int outputId;
    public int outputAmount;
    public string name;
    public float time;
}

[System.Serializable]
public class SmelterRecipeList
{
    public List<SmelterRecipe> recipes;
}

public class SmelterRecipeLoader : MonoBehaviour
{
    public static string jsonFilePath = "Assets/Resources/JSON Files/SmelterRecipes.json";
    public static Dictionary<int, SmelterRecipe> smelterRecipeDictionary;

    void Start()
    {
        LoadSmelterRecipes();
    }

    void LoadSmelterRecipes()
    {
        string json = File.ReadAllText(jsonFilePath);
        SmelterRecipeList recipeList = JsonUtility.FromJson<SmelterRecipeList>("{\"recipes\":" + json + "}");
        smelterRecipeDictionary = new Dictionary<int, SmelterRecipe>();
        foreach (SmelterRecipe recipe in recipeList.recipes)
        {
            smelterRecipeDictionary[recipe.recipeId] = recipe;
        }
    }

    public SmelterRecipe GetSmelterRecipe(int recipeId)
    {
        smelterRecipeDictionary.TryGetValue(recipeId, out SmelterRecipe recipe);
        return recipe;
    }
}


