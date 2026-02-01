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
        Debug.Log("OpenSellPanel called!");
        
        if (sellPanel == null)
        {
            Debug.LogError("SellPanel is NULL! Not assigned in Inspector!");
            return;
        }
        
        textPrice.text = maskData.finalMoney.ToString();
        
        Debug.Log($"Setting sellPanel active. Current state: {sellPanel.activeSelf}");
        sellPanel.SetActive(true);
        Debug.Log("SellPanel set to active");
        
        if (BlockUI.instance != null)
        {
            BlockUI.instance.ShowCursorAndPauseGame();
            Debug.Log("BlockUI ShowCursor called");
        }
        else
        {
            Debug.LogError("BlockUI.instance is NULL!");
        }
    }

    public void SellMaskFunction()
    {
        playerData.money += maskData.finalMoney;
        textPrice.text = maskData.finalMoney.ToString();
        ProgressManager.instance.RestartOrder();
        sellPanel.SetActive(false);
        BlockUI.instance.HideCursorAndUnpauseGame();
        
        // Powiadom OrderManager że maska została sprzedana (co wywołuje Customer.OnMaskSold)
        if (OrderManager.instance != null)
        {
            OrderManager.instance.NotifyCustomerMaskSold();
        }
    }
   
}