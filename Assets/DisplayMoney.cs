using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (PlayerStats.money < 100)
        {
            SceneManager.LoadSceneAsync(2);
        }

        TextMeshProUGUI tm = GetComponent<TextMeshProUGUI>();

        if(tm != null)
        {
            tm.text = "Money: " + PlayerStats.money.ToString() + " RUB";
        }
    }
}
