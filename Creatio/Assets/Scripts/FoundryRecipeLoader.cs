using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class FoundryRecipe
{
    public int recipeId;
    public int input1Id;
    public int input1Amount;
    public int input2Id;
    public int input2Amount;
    public int outputId;
    public int outputAmount;
    public string name;
    public float time;
}

[System.Serializable]
public class FoundryRecipeList
{
    public List<FoundryRecipe> recipes;
}

public class FoundryRecipeLoader : MonoBehaviour
{
    public static string jsonFilePath = "Assets/Resources/JSON Files/FoundryRecipes.json";
    public static Dictionary<int, FoundryRecipe> FoundryRecipeDictionary;

    void Start()
    {
        LoadFoundryRecipes();
    }

    void LoadFoundryRecipes()
    {
        string json = File.ReadAllText(jsonFilePath);
        FoundryRecipeList recipeList = JsonUtility.FromJson<FoundryRecipeList>("{\"recipes\":" + json + "}");
        FoundryRecipeDictionary = new Dictionary<int, FoundryRecipe>();
        foreach (FoundryRecipe recipe in recipeList.recipes)
        {
            FoundryRecipeDictionary[recipe.recipeId] = recipe;
        }
    }

    public FoundryRecipe GetFoundryRecipe(int recipeId)
    {
        FoundryRecipeDictionary.TryGetValue(recipeId, out FoundryRecipe recipe);
        return recipe;
    }
}


