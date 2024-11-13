using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constructor : MonoBehaviour
{
    static ConstructorRecipeLoader recipeLoader;
    ConstructorRecipe selectedRecipe;

    public ItemOutput itemOutput;
    public ItemInput itemInput;
    bool isCrafting = false;

    [SerializeField] float timer;

    // Start is called before the first frame update
    void Start()
    {
        if(recipeLoader == null)
        {
            FindRecipeLoader();
        }
        itemInput = GetComponent<ItemInput>();
        itemOutput = GetComponent<ItemOutput>();

        selectedRecipe = recipeLoader.GetConstructorRecipe(0);

        itemInput.Initialize(selectedRecipe.inputId);
        itemOutput.Initialize(selectedRecipe.outputId);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Adding to spawner buffer");
        if(!isCrafting && itemInput.buffer >= selectedRecipe.inputAmount && itemOutput.buffer + selectedRecipe.outputAmount < itemOutput.maxBuffer) {
            // Start crafting animation
            isCrafting = true;
            StartCoroutine(Craft());
        }
    }

    IEnumerator Craft()
    {
        while (isCrafting) {timer += Time.deltaTime;
            Debug.Log($"Crafting... {timer}/{selectedRecipe.time}");
            if (timer >= selectedRecipe.time) {
                itemInput.buffer -= selectedRecipe.inputAmount;
                itemOutput.buffer += selectedRecipe.outputAmount;
                timer = 0;
                isCrafting = false;
                yield break;
            }
            yield return null;
        }
        yield break;     
    }

    public void SelectRecipe(int inputId)
    {
        if (recipeLoader != null)
        {
            selectedRecipe = recipeLoader.GetConstructorRecipe(inputId);
            if (selectedRecipe != null)
            {
                Debug.Log($"Selected Recipe: {selectedRecipe.name}");
            }
            else
            {
                Debug.LogWarning($"No recipe found for input ID: {inputId}");
            }
        }
        else
        {
            Debug.LogError("ConstructorRecipeLoader not found!");
        }
    }

    void FindRecipeLoader()
    {
        GameObject recipeLoaderObject = GameObject.Find("RecipeLoader");

        if (recipeLoaderObject != null)
        {
            recipeLoader = recipeLoaderObject.GetComponent<ConstructorRecipeLoader>();
        }    
    }
}
