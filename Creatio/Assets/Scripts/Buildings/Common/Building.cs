using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes

public class Building : MonoBehaviour
{
    //Mateusz i Agnieszka zróbcie do tego grafiki, jest tego dużo.
    public BuildingPlacement manager;
    public Button button;
    public Button menu;
    public GameObject closeUIButtonPrefab; // Prefabrykat przycisku
    public Button CloseUIButton;
    public GameObject recipseclone;
    private SpriteRenderer sprite;

    public int numberOfClones;
<<<<<<< Updated upstream

    private bool deleteMode = false;
    private bool groupDeleteMode = false;
    private bool currentDeleteMode = false;
=======
    bool deleteMode = false;
    bool groupDeleteMode = false;

    bool currentDeleteMode = false;
>>>>>>> Stashed changes
    private List<GameObject> clones = new List<GameObject>();  // Lista przechowująca klony

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        if (menu != null)
        {
            menu.onClick.RemoveAllListeners(); 
            menu.onClick.AddListener(OpenUI); 
        }
<<<<<<< Updated upstream
    }

    public class FoundryRecipeList
    {
        public List<FoundryRecipe> recipes = new List<FoundryRecipe>();
=======
>>>>>>> Stashed changes
    }

    void Update()
    {
        if (manager == null) return;

        if (!manager.buildMode)
        {
            if (!manager.deleteMode)
            {
                ResetDeleteModes();
            }
            else if (deleteMode || groupDeleteMode)
            {
                sprite.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                if (Input.GetMouseButtonDown(0))
                {
                    Destroy(gameObject);
                }
            }
        }

        if (Input.GetKeyDown("q"))
        {   
            if (menu != null && menu.gameObject.activeSelf)
            {
                menu.gameObject.SetActive(false);
            }
            if (button != null && button.gameObject.activeSelf)
            {
                button.gameObject.SetActive(false);
            }
            foreach (var clone in clones)
            {
                if (clone != null && clone.activeSelf)
                {
                    Destroy(clone);
                }
            }

            clones.Clear();
        }
<<<<<<< Updated upstream
=======
    }

        private void ResetDeleteModes()
    {
        deleteMode = false;
        groupDeleteMode = false;
        if (manager.deleteMode != currentDeleteMode)
        {
            sprite.color = new Color(1f, 1f, 1f, 1f);
            currentDeleteMode = manager.deleteMode;
        }
>>>>>>> Stashed changes
    }

    private void ResetDeleteModes()
    {
        deleteMode = false;
        groupDeleteMode = false;
        if (manager.deleteMode != currentDeleteMode)
        {
            sprite.color = new Color(1f, 1f, 1f, 1f);
            currentDeleteMode = manager.deleteMode;
        }
    }
    public void SetManager(BuildingPlacement reference, UIManager uimanager)
    {
        manager = reference;
        button = uimanager.myButton;
        menu = uimanager.Menu;
    }
    private void OnMouseEnter()
    {
        if (manager != null && manager.deleteMode)
        {
            deleteMode = true;
            if (Input.GetKey(KeyCode.LeftControl))
            {
                groupDeleteMode = true;
            }
        }
    }
    private void HighlightBuilding()
    {
        sprite.color = new Color(0.8f, 0.8f, 0.8f, 0.8f);
    }
    private void OnMouseOver()
    {
        if (manager == null) return;

        if (!manager.buildMode && !manager.deleteMode)
        {
            HighlightBuilding();

            if (Input.GetMouseButtonDown(1))
            {
                OpenMenu(); 
                Debug.Log("Opened Menu");
            }
        }

        if (manager.deleteMode)
        {
            deleteMode = true;
            groupDeleteMode = Input.GetKey(KeyCode.LeftControl);
        }
    }

    private void OnMouseExit()
    {
        if (!groupDeleteMode)
        {
            deleteMode = false;
            sprite.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    private void HighlightBuilding()
    {
        sprite.color = new Color(0.8f, 0.8f, 0.8f, 0.8f);
    }

    public void SetManager(BuildingPlacement reference, UIManager uimanager)
    {
        manager = reference;
        button = uimanager.myButton;
        menu = uimanager.Menu;
    }
    public void OpenMenu(){
        menu.gameObject.SetActive(true);
    }

    public void OpenUI()
{
    if (button != null && clones.Count == 0)
    {
        button.gameObject.SetActive(false);

        numberOfClones = 5;
        float spacing = (100f / numberOfClones) * 2.5f;

        for (int i = 0; i < numberOfClones; i++)
        {
            GameObject clone = Instantiate(button.gameObject);
            if (clone == null)
            {
                Debug.LogError($"Failed to create clone {i}");
                continue;
            }

            clone.transform.SetParent(button.transform.parent);
            clone.transform.localScale = button.transform.localScale;
            clone.transform.position = button.transform.position + new Vector3(0f, -i * spacing, 0f);
            clone.gameObject.SetActive(true);

            TMP_Text buttonText = clone.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = $"Przepis {i + 1}";
            }
            else
            {
                Debug.LogWarning($"Clone {i} is missing a TMP_Text component!");
            }

            Button cloneButton = clone.GetComponent<Button>();
            if (cloneButton != null)
            {
                string recipeName = $"Przepis {i + 1}";
                cloneButton.onClick.AddListener(() => SelectRecipe(recipeName));
            }
            else
            {
                Debug.LogWarning($"Clone {i} is missing a Button component!");
            }

            clones.Add(clone);
        }
    }
    else if (button == null)
    {
        Debug.LogWarning("Button is not assigned!");
    }
}

    private void SelectRecipe(string recipeName)
{
    Debug.Log($"Selected recipe: {recipeName}");

    if (menu != null)
    {
        TMP_Text menuText = menu.GetComponentInChildren<TMP_Text>();
        if (menuText != null)
        {
            menuText.text = recipeName;
        }
        else
        {
            Debug.LogWarning("Menu does not have a TMP_Text component!");
        }
    }
    else
    {
        Debug.LogWarning("Menu is not assigned!");
    }

    foreach (var clone in clones)
    {
        if (clone != null)
        {
            Destroy(clone);
        }
    }
    clones.Clear();
}


    public void SetManager(BuildingPlacement reference)
    {
        manager = reference;
    }

    FoundryRecipeList foundryRecipeList = new FoundryRecipeList();
    public void OpenMenu(){
        menu.gameObject.SetActive(true);
    }

    public void OpenUI()
    {
        if (button != null && clones.Count == 0)  
        {
            button.gameObject.SetActive(false); 

            numberOfClones = 5;  
            float spacing = (100f / numberOfClones) * 2.5f;

            for (int i = 0; i < numberOfClones; i++)
            {
                Debug.Log($"Creating clone {i}");
                GameObject clone = Instantiate(button.gameObject);
                clone.transform.SetParent(button.transform.parent);
                clone.transform.localScale = button.transform.localScale; 
                clone.transform.position = button.transform.position + new Vector3(0f, -i * spacing, 0f);
                clone.gameObject.SetActive(true);

                // Ustawienie tekstu / docelowo grafika
                TMP_Text buttonText = clone.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    buttonText.text = $"Przepis {i + 1}"; 
                }
                else
                {
                    Debug.LogWarning("Button clone has no Text component!");
                }

                // Kliknięcie
                Button cloneButton = clone.GetComponent<Button>();
                if (cloneButton != null)
                {
                    string recipeName = $"Przepis {i + 1}"; 
                    cloneButton.onClick.AddListener(() => SelectRecipe(recipeName));
                }

                clones.Add(clone);
            }
        }
    }

    private void SelectRecipe(string recipeName)
    {
        Debug.Log($"Selected recipe: {recipeName}");

        // Tekst menu na nazwę przepisu / docelowo grafika - Mateusz i Agnieszka 
        TMP_Text menuText = menu.GetComponentInChildren<TMP_Text>();
        if (menuText != null)
        {
            menuText.text = recipeName;
        }

        foreach (var clone in clones)
        {
            if (clone != null)
            {
                Destroy(clone);
            }
        }
        clones.Clear();
    }
}
