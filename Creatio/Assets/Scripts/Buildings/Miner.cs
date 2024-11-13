using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Miner : MonoBehaviour
{
    [SerializeField] float extractionRate;
    [SerializeField] int resourceType;

    public ResourceTile resourceTile;

    ItemOutput itemOutput;
    [SerializeField] float timer;





    // Start is called before the first frame update
    void Start()
    {
        itemOutput = GetComponent<ItemOutput>();
        itemOutput.Initialize(resourceType);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= (1/extractionRate))
        {
            //Debug.Log("Adding to spawner buffer");
            itemOutput.AddToBuffer();
            timer = 0f;
        }
    }

    public void SetParams(ResourceTile resourceTile) {
        if(resourceTile == null) {
            Debug.LogError("ResourceTile is null");
            return;
        }

        resourceTile.GetParams(out resourceType, out int inputExtractionRate);
        this.resourceTile = resourceTile;
        extractionRate = inputExtractionRate;
    }
}
