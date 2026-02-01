using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProgressManager : MonoBehaviour
{

    public static ProgressManager instance;

    public MaskData maskData;
    public PlayerData playerData;
    
    public bool canChooseMask = true;
    public bool canPaintMask = false;
    public bool canDecorateMask = false;
    public bool canBakeMask = false;

    public TextMeshProUGUI textOfferedMoney;
    public TextMeshProUGUI textNeedDiamonds;
    public TextMeshProUGUI textNeedFeathers;
    public TextMeshProUGUI textNeedLeaves;
    public TextMeshProUGUI textNeedFlowers;
    public TextMeshProUGUI textNeedStars;
    public GameObject maskImage;
    
    public TextMeshProUGUI textMoney;
    public TextMeshProUGUI textDiamonds;
    public TextMeshProUGUI textFeathers;
    public TextMeshProUGUI textLeaves;
    public TextMeshProUGUI textFlowers;
    public TextMeshProUGUI textStars;

    public GameObject infoPanel;


    private void Awake()
    {
        instance = this;
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            UpdateUI();
            infoPanel.SetActive(true);
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            infoPanel.SetActive(false);
        }
    }

    public void RestartOrder()
    {
        maskData = ScriptableObject.CreateInstance<MaskData>();
        canChooseMask = true;
        canPaintMask = false;
        canDecorateMask = false;
        canBakeMask = false;
    }


    public void GetMaskByName(string name)
    {
        Sprite maskImageFinder = Resources.Load<Sprite>("Masks/" + name);
        maskImage.GetComponent<Image>().sprite = maskImageFinder;
    }

    public void UpdateUI()
    {
        textOfferedMoney.text = maskData.negotiatedMoney.ToString();
        textNeedDiamonds.text = maskData.diamondsNeedToUse.ToString();
        textNeedFeathers.text = maskData.feathersNeedToUse.ToString();
        textNeedLeaves.text = maskData.leavesNeedToUse.ToString();
        textNeedFlowers.text = maskData.flowersNeedToUse.ToString();
        textNeedStars.text = maskData.starsNeedToUse.ToString();
        GetMaskByName(maskData.typeCostumer);

        textMoney.text = playerData.money.ToString();
        textDiamonds.text = playerData.diamondsCount.ToString();
        textFeathers.text = playerData.feathersCount.ToString();
        textLeaves.text = playerData.leavesCount.ToString();
        textFlowers.text = playerData.flowersCount.ToString();
        textStars.text = playerData.starsCount.ToString();
    }

    public void CloseInfoPanel()
    {
        infoPanel.SetActive(false);
    }
       
}
