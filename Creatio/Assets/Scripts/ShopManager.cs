using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public GameObject shopPanel;
    public Button openShopButton;
    public Button closeShopButton;

    public Button buyGenerator1Button;
    public Button buyGenerator2Button;
    public Button buyGenerator3Button;

    public TextMeshProUGUI moneyText;

    public GameObject cartPanel;
    public Button cartButton;
    public Button closeCartButton;

    public Transform cartContentParent;
    public GameObject cartItemPrefab;

    public Sprite gen1Sprite;
    public Sprite gen2Sprite;
    public Sprite gen3Sprite;

    private int money = 500;
    private int itemCost = 100;

    private int gen1Count = 0;
    private int gen2Count = 0;
    private int gen3Count = 0;

    void Start()
    {
        shopPanel.SetActive(false);
        cartPanel.SetActive(false);

        openShopButton.onClick.AddListener(OpenShop);
        closeShopButton.onClick.AddListener(CloseShop);
        cartButton.onClick.AddListener(OpenCart);
        closeCartButton.onClick.AddListener(CloseCart);

        buyGenerator1Button.onClick.AddListener(() => BuyItem("Generator 1"));
        buyGenerator2Button.onClick.AddListener(() => BuyItem("Generator 2"));
        buyGenerator3Button.onClick.AddListener(() => BuyItem("Generator 3"));

        UpdateMoneyUI();
    }

    void OpenShop()
    {
        shopPanel.SetActive(true);
        closeShopButton.gameObject.SetActive(true);
        openShopButton.gameObject.SetActive(false);
        cartButton.gameObject.SetActive(false);
    }

    void CloseShop()
    {
        shopPanel.SetActive(false);
        closeShopButton.gameObject.SetActive(false);
        openShopButton.gameObject.SetActive(true);
        cartButton.gameObject.SetActive(true);
    }

    void BuyItem(string itemName)
    {
        if (money >= itemCost)
        {
            money -= itemCost;

            switch (itemName)
            {
                case "Generator 1": gen1Count++; break;
                case "Generator 2": gen2Count++; break;
                case "Generator 3": gen3Count++; break;
            }

            UpdateMoneyUI();
        }
        else
        {
            Debug.Log("Za mało pieniędzy na: " + itemName);
        }
    }

    void UpdateMoneyUI()
    {
        moneyText.text = money + " $";
    }

    void OpenCart()
    {
        cartPanel.SetActive(true);
        RefreshCartUI();
    }

    void CloseCart()
    {
        cartPanel.SetActive(false);
    }

    void RefreshCartUI()
    {
        foreach (Transform child in cartContentParent)
        {
            Destroy(child.gameObject);
        }

        if (gen1Count > 0) CreateCartItem("Generator 1", gen1Count, gen1Sprite, () => UseGenerator(ref gen1Count, "Generator 1"));
        if (gen2Count > 0) CreateCartItem("Generator 2", gen2Count, gen2Sprite, () => UseGenerator(ref gen2Count, "Generator 2"));
        if (gen3Count > 0) CreateCartItem("Generator 3", gen3Count, gen3Sprite, () => UseGenerator(ref gen3Count, "Generator 3"));
    }

    void CreateCartItem(string name, int count, Sprite sprite, UnityEngine.Events.UnityAction onUse)
    {
        GameObject item = Instantiate(cartItemPrefab, cartContentParent);
        item.transform.Find("Image").GetComponent<Image>().sprite = sprite;
        item.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = name;
        item.transform.Find("CountText").GetComponent<TextMeshProUGUI>().text = "Ilość: " + count;
        item.transform.Find("UseButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            onUse.Invoke();
            RefreshCartUI();
        });
    }

    void UseGenerator(ref int count, string name)
    {
        if (count > 0)
        {
            count--;
            Debug.Log("Użyto - " + name);
        }
    }
}
