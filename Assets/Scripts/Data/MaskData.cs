using UnityEngine;

[CreateAssetMenu(fileName = "MaskData", menuName = "Scriptable Objects/MaskData")]
public class MaskData : ScriptableObject
{
    public int negotiatedMoney = 100;
    public int diamondsNeedToUse = 0;
    public int feathersNeedToUse = 0;
    public int flowersNeedToUse = 0;
    public int leavesNeedToUse = 0;
    public int starsNeedToUse = 0;
    public int diamondsUsed = 0;
    public int feathersUsed = 0;
    public int flowersUsed = 0;
    public int leavesUsed = 0;
    public int starsUsed = 0;
    public int starsWorthCount = 5;
    public int burntPercent = 0;
    public int accuracyPercent = 0;
    public string typeCostumer;
    public string typeCurrent;


    public int worthPercent
    {
        get
        {
            int worth = 100;

            if (typeCostumer != typeCurrent)
            {
                worth = (int)(worth * 0.5f);
            }

            if (accuracyPercent < 30)
            {
                worth = (int)(worth * 0.6f);
            }

            if (accuracyPercent < 70)
            {
                worth = (int)(worth * 0.3f);
            }

            if (diamondsNeedToUse > diamondsUsed)
            {
                worth = (int)(worth * 0.9f);
            }
            if (feathersNeedToUse > feathersUsed)
            {
                worth = (int)(worth * 0.9f);
            }
            if (flowersNeedToUse > flowersUsed)
            {
                worth = (int)(worth * 0.9f);
            }
            if (leavesNeedToUse > leavesUsed)
            {
                worth = (int)(worth * 0.9f);
            }
            if (starsNeedToUse > starsUsed)
            {
                worth = (int)(worth * 0.9f);
            }

            if (burntPercent > 30)
            {
                worth = (int)(worth * 0.5f);
            }

            if (burntPercent > 70)
            {
                worth = (int)(worth * 0.3f);
            }

            return worth;
        }
    }

    public int finalMoney
    {
        get
        {
            return (int)(negotiatedMoney * worthPercent / 100f);
        }
    }
}
