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
    
    [Tooltip("MaskGenerator - źródło wygenerowanej maski (wzór)")]
    public MaskGenerator maskGenerator;
    
    [Tooltip("MaskLinePainter - źródło namalowanej maski (gracza)")]
    public MaskLinePainter maskLinePainter;
    
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
        
        // Pobierz tekstury bezpośrednio z generatorów
        Texture2D generatedTexture = null;
        Texture2D paintedTexture = null;
        
        if (maskGenerator != null)
        {
            generatedTexture = maskGenerator.generatedTexture;
        }
        else
        {
            Debug.LogError("CheckAccuracyButton: Nie przypisano MaskGenerator!");
            DisplayResult("ERROR: Brak MaskGenerator!");
            return;
        }
        
        if (maskLinePainter != null)
        {
            // Pobierz drawableTexture z MaskLinePainter (musimy dodać publiczny getter)
            paintedTexture = maskLinePainter.GetDrawableTexture();
        }
        else
        {
            Debug.LogError("CheckAccuracyButton: Nie przypisano MaskLinePainter!");
            DisplayResult("ERROR: Brak MaskLinePainter!");
            return;
        }
        
        if (generatedTexture == null || paintedTexture == null)
        {
            Debug.LogError("CheckAccuracyButton: Jedna z tekstur jest null!");
            DisplayResult("ERROR: Brak tekstur do porównania!");
            return;
        }
        
        // Użyj nowej metody z Texture2D
        float accuracy = similarityChecker.CheckSimilarity(generatedTexture, paintedTexture);
        
        if (maskData != null)
        {
            maskData.accuracyPercent = (int)accuracy;
#if UNITY_EDITOR
            EditorUtility.SetDirty(maskData);
#endif
        }
        else
        {
            Debug.LogWarning("CheckAccuracyButton: Brak przypisanego MaskData! Wynik nie został zapisany.");
        }
        
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

    /// <summary>
    /// Sprawdza czy namalowana maska jest pusta. Jeśli tak, zapisuje szablon jako namalowaną maskę.
    /// </summary>
    // private void EnsurePaintedMaskNotEmpty() ... USUNIĘTE NA PROŚBĘ GRACZA (Opcja 1)
}
