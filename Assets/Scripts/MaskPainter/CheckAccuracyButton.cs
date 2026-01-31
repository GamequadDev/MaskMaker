using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

/// <summary>
/// Przycisk do sprawdzenia dokładności maski i wyświetlenia wyniku
/// </summary>
public class CheckAccuracyButton : MonoBehaviour
{
    [Header("Komponenty")]
    [Tooltip("MaskSimilarityChecker który sprawdza podobieństwo")]
    public MaskSimilarityChecker similarityChecker;
    
    [Tooltip("Text UI gdzie zostanie wyświetlony wynik (dla standardowego UI.Text)")]
    public Text resultText;
    
    [Tooltip("TextMeshPro UI gdzie zostanie wyświetlony wynik (alternatywa dla TMPro)")]
    public TextMeshProUGUI resultTextTMP;
    
    [Header("Opcjonalne")]
    [Tooltip("Przycisk - zostanie automatycznie pobrany jeśli null")]
    public Button checkButton;

    [Header("Zapis do danych maski")]
    public MaskData maskData;
    
    void Start()
    {
        // Pobierz przycisk z tego samego GameObject jeśli nie przypisano
        if (checkButton == null)
        {
            checkButton = GetComponent<Button>();
        }
        
        // Przypisz funkcję do przycisku
        if (checkButton != null)
        {
            checkButton.onClick.AddListener(CheckAndDisplayAccuracy);
        }
        
        // Ukryj tekst na starcie
        HideResult();
    }
    
    /// <summary>
    /// Sprawdź dokładność i wyświetl wynik
    /// Możesz wywołać tę funkcję też z innych skryptów lub OnClick() w inspektorze
    /// </summary>
    public void CheckAndDisplayAccuracy()
    {
        if (similarityChecker == null)
        {
            Debug.LogError("Nie przypisano MaskSimilarityChecker!");
            DisplayResult("ERROR: Brak MaskSimilarityChecker!");
            return;
        }
        
        // Sprawdź podobieństwo
        float accuracy = similarityChecker.CheckSimilarity();
        maskData.accuracyPercent = (int)accuracy;
        EditorUtility.SetDirty(maskData);
        
        // Wyświetl wynik
        string message = $"ACCURACY SCORE OF YOUR MASK: {accuracy:F1}%";
        DisplayResult(message);
        
        Debug.Log($"✓ {message}");
    }
    
    /// <summary>
    /// Wyświetl wiadomość w Text UI
    /// </summary>
    private void DisplayResult(string message)
    {
        if (resultText != null)
        {
            resultText.text = message;
            resultText.gameObject.SetActive(true);
        }
        
        if (resultTextTMP != null)
        {
            resultTextTMP.text = message;
            resultTextTMP.gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// Ukryj wynik
    /// </summary>
    public void HideResult()
    {
        if (resultText != null)
        {
            resultText.gameObject.SetActive(false);
        }
        
        if (resultTextTMP != null)
        {
            resultTextTMP.gameObject.SetActive(false);
        }
    }
}
