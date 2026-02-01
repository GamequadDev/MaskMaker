using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FurnaceEvent : MonoBehaviour
{

    [Header("UI Elements")]
    public RectTransform indicator;
    public RectTransform bar;
    public RectTransform target;
    public TextMeshProUGUI attemptOneText;
    public TextMeshProUGUI attemptTwoText;

    [Header("Settings")]
    bool movingRight = true;
    public float speed = 100f;
    public float barWidth;

    public MaskData maskData;

    private int currentAttempt = 0;
    private bool isAttemptActive = false;

    private float minX;
    private float maxX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CalculateBounds();   
    }

    void OnEnable()
    {
        // Automatycznie wystartuj minigame gdy panel się włączy
        StartMinigame();
    }

    /// <summary>
    /// Publiczna metoda do uruchomienia minigame - wywołaj ją gdy włączasz panel podczas gry
    /// </summary>
    public void StartMinigame()
    {
        // Musimy przeliczyć granice przed startem
        CalculateBounds();
        // Rozpocznij pierwszą próbę
        startFirstAttempt();
    }

    void CalculateBounds()
    {
        float halfBar = bar.rect.width / 2f;
        float halfIndicator = indicator.rect.width / 2f;

        // Ustalamy sztywne granice, których wskaźnik nie może przekroczyć
        maxX = halfBar - halfIndicator;
        minX = -maxX;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAttemptActive) return;

        MoveIndicator();

        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleAttempt();
        }
    }

    void MoveIndicator()
    {
        float currentX = indicator.anchoredPosition.x;

        if (movingRight)
        {
            currentX += speed * Time.deltaTime;
            if (currentX >= maxX)
            {
                currentX = maxX; // "Dobijamy" do krawędzi
                movingRight = false;
            }
        }
        else
        {
            currentX -= speed * Time.deltaTime;
            if (currentX <= minX)
            {
                currentX = minX; // "Dobijamy" do krawędzi
                movingRight = true;
            }
        }

        indicator.anchoredPosition = new Vector2(currentX, indicator.anchoredPosition.y);
    }

    public void startFirstAttempt()
    {
        currentAttempt = 1;
        isAttemptActive = true;
        attemptOneText.text = "Press E!";
    }

    public void startSecondAttempt()
    {
        currentAttempt = 2;
        isAttemptActive = true;
        attemptTwoText.text = "Press E!";
    }

    void HandleAttempt()
    {
        bool success = CheckSuccess();

        if (currentAttempt == 1)
        {
            if (success)
            {
                attemptOneText.text = "Success";
            }
            else
            {
                attemptOneText.text = "Failure";
                maskData.burntPercent += 30;
            }

            isAttemptActive = false;
            // Automatycznie rozpocznij drugą próbę po zakończeniu pierwszej
            startSecondAttempt();
        }
        else if (currentAttempt == 2)
        {
            if (success)
            {
                attemptTwoText.text = "Success";
            }
            else
            {
                attemptTwoText.text = "Failure";
                maskData.burntPercent += 70;
            }

            isAttemptActive = false;
            // Po drugiej próbie kończymy minigame - czekamy 2 sekundy i zamykamy panel
            StartCoroutine(ExitMinigame());
        }
    }

    bool CheckSuccess()
    {
        // Pobieramy granice Target
        float targetPos = target.anchoredPosition.x;
        float targetHalfWidth = target.rect.width / 2f;

        float minX = targetPos - targetHalfWidth;
        float maxX = targetPos + targetHalfWidth;

        float indicatorX = indicator.anchoredPosition.x;

        if (indicatorX >= minX && indicatorX <= maxX)
        {
            Debug.Log("Success!");
            return true;
        }
        else
        {
            Debug.Log("Failure!");
            return false;
        }
    }

    public IEnumerator ExitMinigame()
    {
        //WAIT 2 SECONDS
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
        BlockUI.instance.HideCursorAndUnpauseGame();
    }

}
