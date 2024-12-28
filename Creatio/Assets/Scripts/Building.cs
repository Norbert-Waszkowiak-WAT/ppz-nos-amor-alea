using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
<<<<<<< Updated upstream:Creatio/Assets/Scripts/Building.cs
    public BuildingPlacement buildingManager;
=======
    public BuildingPlacement manager;
    public Button button;
    public GameObject closeUIButtonPrefab; // Prefabrykat przycisku
    public Button CloseUIButton;
    public GameObject recipseclone;
    private SpriteRenderer sprite;
>>>>>>> Stashed changes:Creatio/Assets/Scripts/Buildings/Common/Building.cs

    private bool deleteMode = false;
    private bool groupDeleteMode = false;
    private bool currentDeleteMode = false;
    private List<GameObject> clones = new List<GameObject>();  // Lista przechowująca klony

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public class FoundryRecipeList
    {
        public List<FoundryRecipe> recipes = new List<FoundryRecipe>();
    }

    void Update()
    {
<<<<<<< Updated upstream:Creatio/Assets/Scripts/Building.cs
        if(!buildingManager.buildMode)
=======
        if (manager == null) return;

        if (!manager.buildMode)
>>>>>>> Stashed changes:Creatio/Assets/Scripts/Buildings/Common/Building.cs
        {
            if (!buildingManager.deleteMode)
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
            if (button != null && button.gameObject.activeSelf)
            {
                button.gameObject.SetActive(false);
            }

            // Usuwanie klonów
            foreach (var clone in clones)
            {
                if (clone != null && clone.activeSelf)
                {
                    Destroy(clone);
                }
            }

            // Czyścimy listę klonów po ich zniszczeniu
            clones.Clear();
        }
    }

<<<<<<< Updated upstream:Creatio/Assets/Scripts/Building.cs
=======
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

>>>>>>> Stashed changes:Creatio/Assets/Scripts/Buildings/Common/Building.cs
    private void OnMouseEnter()
    {
        if (buildingManager.deleteMode)
        {
            deleteMode = true;
            if (Input.GetKey(KeyCode.LeftControl))
            {
                groupDeleteMode = true;
            }
        }
    }

    private void OnMouseOver()
    {
<<<<<<< Updated upstream:Creatio/Assets/Scripts/Building.cs
        if (buildingManager.deleteMode)
=======
        if (manager == null) return;

        if (!manager.buildMode && !manager.deleteMode)
>>>>>>> Stashed changes:Creatio/Assets/Scripts/Buildings/Common/Building.cs
        {
            HighlightBuilding();

            if (Input.GetMouseButtonDown(1))
            {
                OpenUI();  // Otwiera UI i tworzy klony
                Debug.Log("Opened");
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
    }

    public void SetManager(BuildingPlacement reference)
    {
        manager = reference;
    }

    FoundryRecipeList foundryRecipeList = new FoundryRecipeList();

    public void OpenUI()
    {
        if (button != null && clones.Count == 0)
        {
            button.gameObject.SetActive(false);

            int numberOfClones = 5; 
            float scaleFactor = 0.8f; // Skala pomniejszenia 
            float spacing = (100f / numberOfClones) * 2.5f; 

            for (int i = 0; i < numberOfClones; i++)
            {
                Debug.Log($"Creating clone {i}");
                GameObject clone = Instantiate(button.gameObject);
                clone.transform.SetParent(button.transform.parent);
                clone.SetActive(true);
                clone.transform.localScale = button.transform.localScale * scaleFactor;
                clone.transform.position = button.transform.position + new Vector3(0f, -i * spacing, 0f);
                clones.Add(clone);
            }
        }
    }
}
