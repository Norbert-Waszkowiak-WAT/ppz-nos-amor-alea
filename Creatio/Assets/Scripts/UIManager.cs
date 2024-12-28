using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour{
        public Button myButton; // Do przeciągnięcia

        private void Start()
        {
            if (myButton != null)
            {
            myButton.gameObject.SetActive(false);
            }
        }

        public void ShowButton()
    {
        if (myButton != null)
        {
            myButton.gameObject.SetActive(true); 
        }
    }
    }
