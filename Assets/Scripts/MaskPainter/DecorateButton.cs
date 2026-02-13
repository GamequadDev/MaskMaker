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
        
        ProgressManager.instance.canDecoratePanel = true;
        ProgressManager.instance.canPaintPanel = false;
        ProgressManager.instance.canBakePanel = false;
        ProgressManager.instance.canChoosePanel = false;
        BlockUI.instance.HideCursorAndUnpauseGame();
    }

    private void LoadPaintedMask()
    {
        // Użyj persistentDataPath - działa w buildzie
        string folderPath = Path.Combine(Application.persistentDataPath, "GeneratedMasks");
        
        // Szukaj najnowszego pliku painted_mask (z timestampem)
        string filePath = FindLatestPaintedMask(folderPath);
        
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogWarning($"DecorateButton: Brak painted_mask.png. Ładuję szablon...");
            LoadFallbackTemplate();
            return;
        }
        
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
        
        // Użyj Resources.Load - działa zarówno w edytorze jak i w buildzie
        string resourcePath = $"Art/Masks/{maskName}";
        Debug.Log($"DecorateButton: Próba wczytania z Resources: {resourcePath}");
        
        Texture2D fallbackTexture = Resources.Load<Texture2D>(resourcePath);
        if (fallbackTexture != null)
        {
            if (maskPreview != null)
            {
                maskPreview.texture = fallbackTexture;
                Debug.Log($"DecorateButton: ✓ Wczytano szablon z Resources: {resourcePath}");
            }
        }
        else
        {
            Debug.LogError($"DecorateButton: Nie znaleziono szablonu w Resources: {resourcePath}");
        }
    }
    
    /// <summary>
    /// Znajduje najnowszy plik painted_mask w folderze (z timestampem lub bez)
    /// </summary>
    private string FindLatestPaintedMask(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            return null;
        }
        
        // Szukaj plików pasujących do wzorca
        string[] files = Directory.GetFiles(folderPath, "painted_mask*.png");
        
        if (files.Length == 0)
        {
            return null;
        }
        
        // Jeśli jest tylko jeden, zwróć go
        if (files.Length == 1)
        {
            return files[0];
        }
        
        // Jeśli więcej, znajdź najnowszy (po dacie modyfikacji)
        string latestFile = files[0];
        System.DateTime latestTime = File.GetLastWriteTime(files[0]);
        
        for (int i = 1; i < files.Length; i++)
        {
            System.DateTime fileTime = File.GetLastWriteTime(files[i]);
            if (fileTime > latestTime)
            {
                latestTime = fileTime;
                latestFile = files[i];
            }
        }
        
        return latestFile;
    }
}
