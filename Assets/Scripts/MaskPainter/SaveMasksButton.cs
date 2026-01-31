using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Skrypt do przycisku - zapisuje maski do ScriptableObject
/// </summary>
public class SaveMasksButton : MonoBehaviour
{
    [Header("Komponenty do Zapisania")]
    [Tooltip("MaskGenerator który wygenerował wzór")]
    public MaskGenerator maskGenerator;
    
    [Tooltip("MaskLinePainter na którym malował gracz")]
    public MaskLinePainter maskLinePainter;
    
    [Header("Opcjonalne")]
    [Tooltip("Przycisk - zostanie automatycznie pobrany jeśli null")]
    public Button saveButton;
    
    void Start()
    {
        // Pobierz przycisk z tego samego GameObject jeśli nie przypisano
        if (saveButton == null)
        {
            saveButton = GetComponent<Button>();
        }
        
        // Przypisz funkcję do przycisku
        if (saveButton != null)
        {
            saveButton.onClick.AddListener(SaveBothMasks);
        }
    }
    
    /// <summary>
    /// Zapisz obie maski jako pliki PNG
    /// Możesz wywołać tę funkcję też z innych skryptów lub OnClick() w inspektorze
    /// </summary>
    public void SaveBothMasks()
    {
        bool success = true;
        
        // Zapisz wygenerowaną maskę
        if (maskGenerator != null)
        {
            maskGenerator.SaveAsPNG();
            Debug.Log("Zapisano wygenerowaną maskę do PNG");
        }
        else
        {
            Debug.LogWarning("Nie przypisano MaskGenerator!");
            success = false;
        }
        
        // Zapisz namalowaną maskę
        if (maskLinePainter != null)
        {
            maskLinePainter.SaveAsPNG();
            Debug.Log("Zapisano namalowaną maskę do PNG");
        }
        else
        {
            Debug.LogWarning("Nie przypisano MaskLinePainter!");
            success = false;
        }
        
        if (success)
        {
            Debug.Log("✓ Obie maski zapisane pomyślnie jako PNG w Assets/GeneratedMasks/!");
        }
    }
}
