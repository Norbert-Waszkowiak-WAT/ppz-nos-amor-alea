using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Building : MonoBehaviour
{
    public BuildingPlacement manager;
    public Button button;
    public Button menu;
    public GameObject closeUIButtonPrefab;
    public Button CloseUIButton;
    public GameObject recipseclone;
    private SpriteRenderer sprite;

    public int numberOfClones;
    private bool deleteMode = false;
    private bool groupDeleteMode = false;
    private bool currentDeleteMode = false;
    public string nameofbuild;

    private List<GameObject> clones = new List<GameObject>();

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        if (menu != null)
        {
            menu.onClick.RemoveAllListeners(); 
            menu.onClick.AddListener(OpenUI); 
        }
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

        if (Input.GetKeyDown(KeyCode.Q))
        {   
            CloseMenu();
        }
    }

    private void ResetDeleteModes()
    {
        deleteMode = false;
        groupDeleteMode = false;
        if (manager.deleteMode != currentDeleteMode)
        {
            sprite.color = Color.white;
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
            sprite.color = Color.white;
        }
    }

    public void OpenMenu()
    {
        Debug.Log("Menu Opened");
        if (menu != null)
        {
            menu.gameObject.SetActive(true);
        }
        
    }

    public void CloseMenu()
    {
        if (menu != null)
        {
            menu.gameObject.SetActive(false);
        }
        if (button != null)
        {
            button.gameObject.SetActive(false);
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

    public void OpenUI()
{
    if (button != null && clones.Count == 0)
    {
        button.gameObject.SetActive(false);
        nameofbuild = gameObject.name;
        Debug.Log($"Opening UI for building: {nameofbuild}");

        if (nameofbuild == "Constructor")
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject newButton = Instantiate(closeUIButtonPrefab); 
                newButton.transform.SetParent(menu.transform); 
                newButton.SetActive(true); 
                clones.Add(newButton); 
                Debug.Log("Constructor");
            }
        }
        if (nameofbuild == "Foundry")
        {
            GameObject newButton = Instantiate(closeUIButtonPrefab); 
            newButton.transform.SetParent(menu.transform); 
            newButton.SetActive(true); 
            clones.Add(newButton);
            Debug.Log("Foundry");
        }
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
}
