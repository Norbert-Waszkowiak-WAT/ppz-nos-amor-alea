using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class ConstructorRecipe
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
public class ConstructorRecipeList
{
    public List<ConstructorRecipe> recipes;
}

public class ConstructorRecipeLoader : MonoBehaviour
{
    public static string jsonFilePath = "Assets/Resources/JSON Files/ConstructorRecipes.json";
    public static Dictionary<int, ConstructorRecipe> constructorRecipeDictionary;

    void Start()
    {
        LoadConstructorRecipes();
    }

    void LoadConstructorRecipes()
    {
        string json = File.ReadAllText(jsonFilePath);
        ConstructorRecipeList recipeList = JsonUtility.FromJson<ConstructorRecipeList>("{\"recipes\":" + json + "}");
        constructorRecipeDictionary = new Dictionary<int, ConstructorRecipe>();
        foreach (ConstructorRecipe recipe in recipeList.recipes)
        {
            constructorRecipeDictionary[recipe.recipeId] = recipe;
        }
    }

    public ConstructorRecipe GetConstructorRecipe(int recipeId)
    {
        constructorRecipeDictionary.TryGetValue(recipeId, out ConstructorRecipe recipe);
        return recipe;
    }
}


