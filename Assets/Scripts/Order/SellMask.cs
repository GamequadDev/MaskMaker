using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SellMask : MonoBehaviour
{

    public MaskData maskData;
    public PlayerData playerData;
    public GameObject sellPanel;

    public TextMeshProUGUI textPrice;

    public void OpenSellPanel()
    {
        sellPanel.SetActive(true);
        BlockUI.instance.ShowCursorAndPauseGame();
    }

    public void SellMaskFunction()
    {
        playerData.money += maskData.finalMoney;
        textPrice.text = maskData.finalMoney.ToString();
        ProgressManager.instance.RestartOrder();
        sellPanel.SetActive(false);
        BlockUI.instance.HideCursorAndUnpauseGame();
    }
   
}