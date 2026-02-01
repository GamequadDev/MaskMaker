using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    public static OrderManager instance;

    public MaskData maskData;
    public ShopPriceData shopPriceData;
    public string[] mask_names;

    public GameObject offerPanel;

    public GameObject acceptButton;
    public GameObject rejectButton;


    public TextMeshProUGUI textOfferedMoney;
    public TextMeshProUGUI textNeedDiamonds;
    public TextMeshProUGUI textNeedFeathers;
    public TextMeshProUGUI textNeedLeaves;
    public TextMeshProUGUI textNeedFlowers;
    public TextMeshProUGUI textNeedStars;
    public GameObject maskImage;

    // Generated Offer Data
    private string offerMasktype;
    private int offerDiamondsNeedToUse;
    private int offerFeathersNeedToUse;
    private int offerLeavesNeedToUse;
    private int offerFlowersNeedToUse;
    private int offerStarsNeedToUse;
    private int offerMoney;
    
    

    private void Awake()
    {
        instance = this;
    }


    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            GenerateNewOrder();
            OpenOfferPanel();
        }
    }



    public int CalculateOfferBasedOnNeeds(int baseMin, int baseMax)
    {
         int price = Random.Range(baseMin, baseMax);
         price += offerDiamondsNeedToUse * shopPriceData.pricePerDiamond;
         price += offerFeathersNeedToUse * shopPriceData.pricePerFeather;
         price += offerLeavesNeedToUse * shopPriceData.pricePerLeaf;
         price += offerFlowersNeedToUse * shopPriceData.pricePerFlower;
         price += offerStarsNeedToUse * shopPriceData.pricePerStar;
         return price;
    }

    public int calculateBadOffer()
    {
         return CalculateOfferBasedOnNeeds(50, 150);
    }

    public int calculateGoodOffer()
    {
        return CalculateOfferBasedOnNeeds(250, 400);
    }

    public int calculateAverageOffer()
    {
        return CalculateOfferBasedOnNeeds(150, 250);
    }

    public void SelectOfferType()
    {
        int offerType = Random.Range(0, 3);
        if(offerType == 0)
        {
            offerMoney = calculateBadOffer();
        }
        else if(offerType == 1)
        {
            offerMoney = calculateAverageOffer();
        }
        else
        {
            offerMoney = calculateGoodOffer();
        }
    }


    public void AcceptOffer()
    {
        
        ProgressManager.instance.UpdateUI();
        maskData.diamondsNeedToUse = offerDiamondsNeedToUse;
        maskData.feathersNeedToUse = offerFeathersNeedToUse;
        maskData.leavesNeedToUse = offerLeavesNeedToUse;
        maskData.flowersNeedToUse = offerFlowersNeedToUse;
        maskData.starsNeedToUse = offerStarsNeedToUse;
        maskData.negotiatedMoney = offerMoney;
        maskData.typeCostumer = offerMasktype;
        offerPanel.SetActive(false);
    }

    public void RejectOffer()
    {
        offerPanel.SetActive(false);
    }

    public void GenerateNewOrder()
    {
        offerMasktype = mask_names[Random.Range(0, mask_names.Length)];
        offerDiamondsNeedToUse = Random.Range(1, 5);
        offerFeathersNeedToUse = Random.Range(1, 5);
        offerLeavesNeedToUse = Random.Range(1, 5);
        offerFlowersNeedToUse = Random.Range(1, 5);
        offerStarsNeedToUse = Random.Range(1, 5);
        SelectOfferType();
    }


    public void OpenOfferPanel()
    {
        offerPanel.SetActive(true);
        GenerateNewOrder();
        textOfferedMoney.text = offerMoney.ToString();
        textNeedDiamonds.text = offerDiamondsNeedToUse.ToString();
        textNeedFeathers.text = offerFeathersNeedToUse.ToString();
        textNeedLeaves.text = offerLeavesNeedToUse.ToString();
        textNeedFlowers.text = offerFlowersNeedToUse.ToString();
        textNeedStars.text = offerStarsNeedToUse.ToString();
        GetMaskByName(offerMasktype);
    }

    public void GetMaskByName(string name)
    {
        Sprite maskImageFinder = Resources.Load<Sprite>("Masks/" + name);
        maskImage.GetComponent<Image>().sprite = maskImageFinder;
    }



}
