using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public int Coin;
    public int Generator;
    public Text Coin_text;
    void Start()
    {
        Coin = 1000;
        Coin_text.text = Coin.ToString();
    }
    public void BuyGenerator(){
        if (Coin >= 100){
            Coin -= 100;
            Coin_text.text = Coin.ToString();
            //Kupno generatora, przeciągnięcie
        }
    }
}
