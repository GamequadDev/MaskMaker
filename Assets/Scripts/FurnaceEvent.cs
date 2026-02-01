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
    public UnityEngine.UI.Button exitButton; // Przycisk wyjścia - włącza się po 2 próbach
    public GameObject panelToClose; // Panel który zostanie zamknięty

    [Header("Settings")]
    bool movingRight = true;
    public float speed = 100f;
    public float barWidth;

    public MaskData maskData;

    private int currentAttempt = 0;
    private bool isAttemptActive = false;
    private bool bothAttemptsCompleted = false;

    private float minX;
    private float maxX;

    void Start()
    {
        CalculateBounds();   
    }

    void OnEnable()
    {
        // Wyłącz przycisk wyjścia na początku
        if (exitButton != null)
        {
            exitButton.interactable = false;
            Debug.Log("Exit button disabled - waiting for 2 attempts");
        }
        
        bothAttemptsCompleted = false;
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
        Debug.Log($"HandleAttempt called - currentAttempt: {currentAttempt}");
        bool success = CheckSuccess();

        if (currentAttempt == 1)
        {
            Debug.Log("Processing FIRST attempt");
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
            Debug.Log("Starting second attempt...");
            startSecondAttempt();
        }
        else if (currentAttempt == 2)
        {
            Debug.Log("Processing SECOND attempt");
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
            bothAttemptsCompleted = true;
            
            // Włącz przycisk wyjścia po zakończeniu obu prób
            if (exitButton != null)
            {
                exitButton.interactable = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Debug.Log("Exit button ENABLED - player can now close the panel");
            }
            else
            {
                Debug.LogError("Exit button is NULL! Assign it in Inspector!");
            }
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

    /// <summary>
    /// Wywołane przez exitButton po zakończeniu obu prób
    /// Zamyka panel i ustawia maskę jako ukończoną
    /// </summary>
    public void OnExitButtonClicked()
    {
        Debug.Log("=== EXIT BUTTON CLICKED ===");
        
        // Ustaw flagę że maska jest gotowa do sprzedaży
        if (ProgressManager.instance != null)
        {
            ProgressManager.instance.isMaskFinished = true;
            Debug.Log("✓ isMaskFinished set to TRUE");
        }
        else
        {
            Debug.LogError("✗ ProgressManager.instance is NULL!");
        }
        
        // Zamknij panel
        if (panelToClose != null)
        {
            Debug.Log($"✓ Closing panel: {panelToClose.name}");
            panelToClose.SetActive(false);
        }
        else
        {
            Debug.LogError("✗ panelToClose is NULL! Assign it in Inspector!");
        }
        
        // Odblokuj grę i ukryj kursor
        if (BlockUI.instance != null)
        {
            BlockUI.instance.HideCursorAndUnpauseGame();
            Debug.Log("✓ Game unpaused, cursor hidden");
        }
        else
        {
            Debug.LogError("✗ BlockUI.instance is NULL!");
        }
        
        Debug.Log("=== BAKING COMPLETE ===");
    }

}
