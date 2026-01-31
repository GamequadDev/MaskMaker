using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class DecorateButton : MonoBehaviour
{
    public RawImage maskPreview;
    public Image maskOutlineImage;
    public GameObject comparePanel;
    public GameObject decoratePanel;
    public LoadMaskButton loadMaskButton;
    
    public void Decorate()
    {
        Debug.Log("DecorateButton: Rozpoczynam Decorate()...");
        
        // Zaktualizuj obrys maski
        if (loadMaskButton != null && maskOutlineImage != null)
        {
            Sprite outline = loadMaskButton.GetCurrentOutlineSprite();
            if (outline != null)
            {
                maskOutlineImage.sprite = outline;
                Debug.Log($"DecorateButton: Ustawiono obrys {outline.name}");
            }
        }
        else if (loadMaskButton == null)
        {
            Debug.LogError("DecorateButton: Brak przypisanego LoadMaskButton!");
        }

        if (comparePanel != null)
        {
             comparePanel.SetActive(false);
             Debug.Log("DecorateButton: Ukryto comparePanel.");
        }
        else
        {
             Debug.LogError("DecorateButton: Brak referencji do Compare Panel!");
        }
        
        LoadPaintedMask();
        
        if (decoratePanel != null)
        {
             decoratePanel.SetActive(true);
             Debug.Log("DecorateButton: Pokazano decoratePanel.");
        }
        else
        {
             Debug.LogError("DecorateButton: Brak referencji do Decorate Panel!");
        }
    }

    private void LoadPaintedMask()
    {
        string filePath = Path.Combine(Application.dataPath, "GeneratedMasks", "painted_mask.png");
        Debug.Log($"DecorateButton: Szukam namalowanej maski w: {filePath}");

        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(fileData))
            {
                // Sprawdź czy tekstura nie jest pusta (przezroczysta)
                if (IsTextureEmpty(texture))
                {
                    Debug.LogWarning("DecorateButton: painted_mask.png jest pusta (przezroczysta). Ładuję szablon...");
                    LoadFallbackTemplate();
                }
                else
                {
                    if (maskPreview != null)
                    {
                        maskPreview.texture = texture;
                        Debug.Log("DecorateButton: Wczytano namalowaną maskę.");
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning($"DecorateButton: Brak painted_mask.png. Ładuję szablon...");
            LoadFallbackTemplate();
        }
    }

    private bool IsTextureEmpty(Texture2D texture)
    {
        // Sprawdź próbkowo co 10 pikseli dla wydajności
        Color[] pixels = texture.GetPixels();
        for (int i = 0; i < pixels.Length; i += 10)
        {
            if (pixels[i].a > 0.1f) return false; // Znaleziono nieprzezroczysty piksel
        }
        return true; // Wszystko przezroczyste = puste
    }

    private void LoadFallbackTemplate()
    {
        if (loadMaskButton == null)
        {
            Debug.LogError("DecorateButton: loadMaskButton jest NULL!");
            return;
        }
        
        if (loadMaskButton.maskData == null)
        {
            Debug.LogError("DecorateButton: loadMaskButton.maskData jest NULL!");
            return;
        }
        
        string maskName = loadMaskButton.maskData.typeCurrent;
        Debug.Log($"DecorateButton: typeCurrent = '{maskName}'");
        
        if (string.IsNullOrEmpty(maskName))
        {
            Debug.LogError("DecorateButton: typeCurrent jest pusty!");
            return;
        }
        
#if UNITY_EDITOR
        string assetPath = $"Assets/Art/Masks/{maskName}.png";
        Debug.Log($"DecorateButton: Próba wczytania z AssetDatabase: {assetPath}");
        
        Texture2D fallbackTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        if (fallbackTexture != null)
        {
            if (maskPreview != null)
            {
                maskPreview.texture = fallbackTexture;
                Debug.Log($"DecorateButton: ✓ Wczytano szablon: {assetPath}");
            }
        }
        else
        {
            Debug.LogError($"DecorateButton: Nie znaleziono szablonu w: {assetPath}");
        }
#else
        string fallbackPath = Path.Combine(Application.dataPath, "Art/Masks", maskName + ".png");
        Debug.Log($"DecorateButton: Próba wczytania z dysku: {fallbackPath}");
        
        if (File.Exists(fallbackPath))
        {
            byte[] fallbackData = File.ReadAllBytes(fallbackPath);
            Texture2D fallbackTexture = new Texture2D(2, 2);
            if (fallbackTexture.LoadImage(fallbackData))
            {
                if (maskPreview != null)
                {
                    maskPreview.texture = fallbackTexture;
                    Debug.Log($"DecorateButton: ✓ Wczytano szablon: {fallbackPath}");
                }
            }
        }
        else
        {
            Debug.LogError($"DecorateButton: Nie znaleziono szablonu: {fallbackPath}");
        }
#endif
    }
}
