using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foundry : MonoBehaviour
{
    static FoundryRecipeLoader recipeLoader;
    public ItemBuffer buffer;
    FoundryRecipe selectedRecipe;
    public ItemOutput itemOutput;
    bool isCrafting = false;
    [SerializeField] float timer;

    void Start()
    {
        if(recipeLoader == null)
        {
            FindRecipeLoader();
        }

        buffer = GetComponent<ItemBuffer>();
        buffer.itemInputs = new Dictionary<int, int>();
        
        itemOutput = transform.Find("ItemOutput").GetComponent<ItemOutput>();

        selectedRecipe = recipeLoader.GetFoundryRecipe(0);

        buffer.itemInputs.Add(selectedRecipe.input1Id, 0);
        buffer.itemInputs.Add(selectedRecipe.input2Id, 0);

        itemOutput = transform.Find("ItemOutput").GetComponent<ItemOutput>();
        itemOutput.Initialize(selectedRecipe.outputId, GetComponent<Building>().manager);
    }

        

    void Update()
    {
        if(itemOutput.buffer + selectedRecipe.outputAmount >= itemOutput.maxBuffer || isCrafting) {
            return;
        }
    
        if (buffer.itemInputs[selectedRecipe.input1Id] < selectedRecipe.input1Amount || buffer.itemInputs[selectedRecipe.input2Id] < selectedRecipe.input2Amount) {
            return;
        }
        
        //Debug.Log("Adding to spawner buffer");
        // Start crafting animation
        isCrafting = true;
        StartCoroutine(Craft());
    }

    IEnumerator Craft()
    {
        while (isCrafting) {timer += Time.deltaTime;
            Debug.Log($"Crafting... {timer}/{selectedRecipe.time}");
            if (timer >= selectedRecipe.time) {

                buffer.itemInputs[selectedRecipe.input1Id] -= selectedRecipe.input1Amount;
                buffer.itemInputs[selectedRecipe.input2Id] -= selectedRecipe.input2Amount;

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
            selectedRecipe = recipeLoader.GetFoundryRecipe(inputId);
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
            recipeLoader = recipeLoaderObject.GetComponent<FoundryRecipeLoader>();
        }    
    }
}