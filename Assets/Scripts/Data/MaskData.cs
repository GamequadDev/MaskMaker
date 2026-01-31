using UnityEngine;

[CreateAssetMenu(fileName = "MaskData", menuName = "Scriptable Objects/MaskData")]
public class MaskData : ScriptableObject
{
    
    public int worthPercent = 100;
    public int negotiatedMoney = 100;
    public int finalMoney = 100;
    public int diamondsUsed = 0;
    public int feathersUsed = 0;
    public int flowersUsed = 0;
    public int leavesUsed = 0;
    public int starsUsed = 0;
    public int starsWorthCount = 5;
    public int burntPercent = 0;
}
