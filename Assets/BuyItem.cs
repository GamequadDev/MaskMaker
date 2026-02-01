using TMPro;
using UnityEngine;
using static BuyItem;

public class BuyItem : MonoBehaviour
{
    public enum ItemType
    {
        Diamond = 1,
        Feather = 2,
        Star = 3,
        Leaf = 4,
        Flower = 5
    }
    
    public PlayerData playerData;
    public BuyItem.ItemType Item;
    public TextMeshProUGUI priceText;
    public ShopPriceData shopPriceData;
    int cost;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI amountText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        switch(Item)
        {
            case ItemType.Diamond:
                priceText.text = "Price: " + shopPriceData.pricePerDiamond + " RUB";
                cost = shopPriceData.pricePerDiamond;
                break;
            case ItemType.Feather:
                priceText.text = "Price: " + shopPriceData.pricePerFeather + " RUB";
                cost = shopPriceData.pricePerFeather;
                break;
                case ItemType.Star:
                priceText.text = "Price: " + shopPriceData.pricePerStar + " RUB";
                cost = shopPriceData.pricePerStar;
                break;
                case ItemType.Leaf:
                priceText.text = "Price: " + shopPriceData.pricePerLeaf + " RUB";
                cost = shopPriceData.pricePerLeaf;
                break;
                case ItemType.Flower:
                priceText.text = "Price: " + shopPriceData.pricePerFlower + " RUB";
                cost = shopPriceData.pricePerFlower;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (coinsText != null)
            coinsText.text = "Money: " + playerData.money.ToString() + " RUB";

        switch(Item)
        {
            case ItemType.Diamond:
                amountText.text = "x" + playerData.diamondsCount.ToString();
                break;
            case ItemType.Feather:
                amountText.text = "x" + playerData.feathersCount.ToString();
                break;
            case ItemType.Star:
                amountText.text = "x" + playerData.starsCount.ToString();
                break;
            case ItemType.Leaf:
                amountText.text = "x" + playerData.leavesCount.ToString();
                break;
            case ItemType.Flower:
                amountText.text = "x" + playerData.flowersCount.ToString();
                break;
        }
    }

    public void Buy()
    {
        if (playerData.money >= cost)
        {
            playerData.money -= cost;
            Debug.Log("Purchased " + Item.ToString() + " for " + cost + " RUB. Remaining money: " + playerData.money + " RUB.");
            switch (Item)
            {
                case ItemType.Diamond:
                    playerData.diamondsCount += 1;
                    break;
                case ItemType.Feather:
                    playerData.feathersCount += 1;
                    break;
                case ItemType.Star:
                    playerData.starsCount += 1;
                    break;
                case ItemType.Leaf:
                    playerData.leavesCount += 1;
                    break;
                case ItemType.Flower:
                    playerData.flowersCount += 1;
                    break;
            }
        }
        
        else
        {
            Debug.Log("Not enough money to purchase " + Item.ToString() + ". Required: " + cost + " RUB, Available: " + playerData.money + " RUB.");
        }
    }
}
