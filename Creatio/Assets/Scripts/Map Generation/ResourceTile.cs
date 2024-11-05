using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityEngine.Tilemaps
{
    [CreateAssetMenu(fileName = "Resource Tile", menuName = "New Resource Tile")]

    [Serializable]
    public class ResourceTile : TileBase
    {
        /// <summary>
        /// The Sprites used for defining the Pipeline.
        /// </summary>
        [SerializeField]
        public Sprite[] m_Sprites; // Sprites depending on the purity of the resource
        public Resource resource; 
        [SerializeField] Purity purity;
        [SerializeField] int extractionRate;

        public void SetParams(int purity,int worldExtractionModifier) {
            this.purity = (Purity) purity;
            extractionRate = ((int) purity + 1) * worldExtractionModifier;
        }

        public void SetPurity(int purity) {
            this.purity = (Purity) purity;
        }

        public void GetParams(out int resource, out int extractionRate) {
            resource = (int) this.resource;
            extractionRate = this.extractionRate;
        }

        private Sprite GetSprite()
        {
            int index = (int)purity;

            if (index >= 0 && index < m_Sprites.Length)
            {
                return m_Sprites[index];
            }
            else
            {
                Debug.LogWarning("Invalid purity index or m_Sprites array is not properly set.");
                return null;
            }
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.transform = Matrix4x4.identity;
            tileData.color = Color.white;
            tileData.sprite = GetSprite();

            tileData.flags = TileFlags.LockTransform | TileFlags.LockColor;
            tileData.colliderType = Tile.ColliderType.Sprite;
        }

    //     private void UpdateTile(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    //     {
            
    //    }
           


        enum Purity {
            Impure,
            Normal,
            Pure
        }

        public enum Resource {
            Stone,
            Iron,
            Copper,
            Tin,
            Lead,
            Coal,
            Sulfur,
            Bauxite,
            Uranium,
            Quartz,
            Gold,
            Silver,
            Diamond
        }
    }
}
