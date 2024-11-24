using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smelter : MonoBehaviour
{
    static SmelterRecipeLoader recipeLoader;
    SmelterRecipe selectedRecipe;

    public ItemBuffer buffer;

    public ItemOutput itemOutput;
    bool isCrafting = false;

    [SerializeField] float timer;

    // Start is called before the first frame update
    void Start()
    {
        if(recipeLoader == null)
        {
            FindRecipeLoader();
        }

        itemOutput = transform.Find("ItemOutput").GetComponent<ItemOutput>();

        selectedRecipe = recipeLoader.GetSmelterRecipe(0);

        itemOutput.Initialize(selectedRecipe.outputId, GetComponent<Building>().manager);

        buffer.itemInputs.Add(selectedRecipe.inputId, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(itemOutput.buffer + selectedRecipe.outputAmount >= itemOutput.maxBuffer || isCrafting) {
            return;
        }
        if (buffer.itemInputs[selectedRecipe.inputId] < selectedRecipe.inputAmount) {
            return;
        }
        
        isCrafting = true;
        StartCoroutine(Craft());
    }

    IEnumerator Craft()
    {
        while (isCrafting) {timer += Time.deltaTime;
            Debug.Log($"Crafting... {timer}/{selectedRecipe.time}");
            if (timer >= selectedRecipe.time) {
                buffer.itemInputs[selectedRecipe.inputId] -= selectedRecipe.inputAmount;

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
            selectedRecipe = recipeLoader.GetSmelterRecipe(inputId);
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
            recipeLoader = recipeLoaderObject.GetComponent<SmelterRecipeLoader>();
        }    
    }
}
