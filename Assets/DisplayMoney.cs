using TMPro;
using UnityEngine;

public class DisplayMoney : MonoBehaviour
{
    public PlayerData PlayerStats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TextMeshProUGUI tm = GetComponent<TextMeshProUGUI>();

        if(tm != null)
        {
            tm.text = "Money: " + PlayerStats.money.ToString() + " RUB";
        }
    }
}
