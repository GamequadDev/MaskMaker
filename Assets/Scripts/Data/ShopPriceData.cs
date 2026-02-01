using UnityEngine;

[CreateAssetMenu(fileName = "ShopPriceData", menuName = "Scriptable Objects/ShopPriceData")]
public class ShopPriceData : ScriptableObject
{
    public int pricePerDiamond;
    public int pricePerFeather;
    public int pricePerLeaf;
    public int pricePerFlower;
    public int pricePerStar;   
}
