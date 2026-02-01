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
    public int cost;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
