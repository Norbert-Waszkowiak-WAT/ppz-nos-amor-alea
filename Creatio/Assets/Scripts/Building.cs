using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Building : MonoBehaviour
{
    public BuildingPlacement buildingManager;

    bool deleteMode = false;
    bool groupDeleteMode = false;
    SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!buildingManager.buildMode)
        {
            if (!buildingManager.deleteMode)
            {
                deleteMode = false;
                groupDeleteMode = false;
                sprite.color = new Color(1f, 1f, 1f, 1f);
            }

            else if (deleteMode || groupDeleteMode)
            {
                sprite.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                if (Input.GetMouseButtonDown(0))
                {
                    Destroy(gameObject);
                }
            }

            else 
            {
                sprite.color = new Color(1f, 1f, 1f, 1f);
            }
        }

    }

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
        if (buildingManager.deleteMode)
        {
            deleteMode = true;
            if (Input.GetKey(KeyCode.LeftControl))
            {
                groupDeleteMode = true;
            }
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                groupDeleteMode = false;
            }
        }
    }
    private void OnMouseExit()
    {
        if (!groupDeleteMode)
        {
            deleteMode = false;
        }
    }

        public void SetDeleteMode(bool mode)
    {
        deleteMode = mode;
    }

    public void SetGroupDeleteMode(bool mode)
    {
        groupDeleteMode = mode;
    }

}
