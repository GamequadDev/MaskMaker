using UnityEngine;
using UnityEngine.UI;

public class LoadMaskButton : MonoBehaviour
{
    public GameObject paintingPanel;
    public GameObject choosePanel;
    
    [Header("Mask Images")]
    public Image maskImage; // Image tego GameObject
    public Image outlineImage; // Image dziecka OutlineImage

    [Header("Masks Objects")]
    public GameObject maskObjectMain;
    public GameObject maskObjectPaintedArea;
    public GameObject maskObjectPaintedOutline;
    public GameObject maskObjectGenerated;
    public GameObject maskObjectGeneratedOutline;
    
    [Header("Generator")]
    public MaskGenerator maskGenerator;
    
    [Header("Painting")]
    public MaskLinePainter maskLinePainter;

    void Awake()
    {
        // Pobierz Image tego GameObject
        maskImage = GetComponent<Image>();
        if (maskImage == null)
        {
            Debug.LogWarning("LoadMaskButton: Brak komponentu Image na tym GameObject!");
        }
        
        // Pobierz Image z dziecka o nazwie "OutlineImage"
        Transform outlineChild = transform.Find("OutlineImage");
        if (outlineChild != null)
        {
            outlineImage = outlineChild.GetComponent<Image>();
            if (outlineImage == null)
            {
                Debug.LogWarning($"LoadMaskButton: Dziecko 'OutlineImage' nie ma komponentu Image!");
            }
        }
        else
        {
            Debug.LogWarning("LoadMaskButton: Nie znaleziono dziecka o nazwie 'OutlineImage'!");
        }
    }
    
    /// <summary>
    /// Pokazuje panel wyboru masek, ukrywa panel malowania
    /// </summary>
    public void ShowChoosePanel()
    {
        if (choosePanel != null)
            choosePanel.SetActive(true);
            
        if (paintingPanel != null)
            paintingPanel.SetActive(false);
    }
    
    /// <summary>
    /// Pokazuje panel malowania, ukrywa panel wyboru
    /// </summary>
    public void ShowPaintingPanel()
    {
        if (paintingPanel != null)
            paintingPanel.SetActive(true);
            
        if (choosePanel != null)
            choosePanel.SetActive(false);
    }

    /// <summary>
    /// Aktualizuje dane masek - odczytuje sprite z maskImage i outlineImage,
    /// a następnie zapisuje do wszystkich obiektów docelowych.
    /// WYWOŁAJ TĘ METODĘ po zmianie sprite'ów w maskImage/outlineImage!
    /// </summary>
    public void UpdateMaskData()
    {
        if (maskImage == null || outlineImage == null)
        {
            Debug.LogWarning("LoadMaskButton: maskImage lub outlineImage są null! Nie można zaktualizować danych.");
            return;
        }
        
        Sprite maskSprite = maskImage.sprite;
        Sprite outlineSprite = outlineImage.sprite;
        
        if (maskSprite == null || outlineSprite == null)
        {
            Debug.LogWarning("LoadMaskButton: Brak sprite'ów w maskImage lub outlineImage!");
            return;
        }
        
        // Zapisz do wszystkich obiektów docelowych
        SaveToObjects(maskSprite, outlineSprite);
        
        // Wygeneruj maskę w generatorze
        if (maskGenerator != null)
        {
            Texture2D maskTex = GetTextureFromSprite(maskSprite);
            maskGenerator.GenerateRandomMask(maskTex);
            
            // Zainicjalizuj malowanie tą samą maską
            if (maskLinePainter != null)
            {
                maskLinePainter.Initialize(maskTex);
            }
        }
        else if (maskLinePainter != null)
        {
            // Jeśli nie ma generatora, ale jest painter, też zainicjalizuj
             Texture2D maskTex = GetTextureFromSprite(maskSprite);
             maskLinePainter.Initialize(maskTex);
        }
        
        Debug.Log($"✓ Zaktualizowano dane masek: {maskSprite.name} (mask) + {outlineSprite.name} (outline)");
    }
    
    /// <summary>
    /// Zapisuje sprite'y do wszystkich obiektów docelowych
    /// Obsługuje zarówno Image (sprite) jak i RawImage (texture)
    /// </summary>
    public void SaveToObjects(Sprite maskSprite, Sprite outlineSprite)
    {
        // Pomocnicza metoda do bezpiecznego przypisania sprite do Image
        void SafeSetSprite(GameObject obj, Sprite sprite, string objectName)
        {
            if (obj == null) return;
            
            Image img = obj.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = sprite;
                return;
            }
            
            Debug.LogWarning($"LoadMaskButton: {objectName} nie ma komponentu Image!");
        }
        
        // Pomocnicza metoda do bezpiecznego przypisania sprite do RawImage
        void SafeSetTexture(GameObject obj, Sprite sprite, string objectName)
        {
            if (obj == null) return;
            
            RawImage rawImg = obj.GetComponent<RawImage>();
            if (rawImg != null)
            {
                // Konwertuj Sprite na Texture2D - wyciągnij tylko fragment sprite'a, nie całą teksturę!
                Texture2D spriteTexture = GetTextureFromSprite(sprite);
                rawImg.texture = spriteTexture;
                return;
            }
            
            Debug.LogWarning($"LoadMaskButton: {objectName} nie ma komponentu RawImage!");
        }
        
        // Obiekty z Image
        SafeSetSprite(maskObjectMain, maskSprite, "maskObjectMain");
        SafeSetSprite(maskObjectPaintedOutline, outlineSprite, "maskObjectPaintedOutline");
        SafeSetSprite(maskObjectGeneratedOutline, outlineSprite, "maskObjectGeneratedOutline");
        
        // Obiekty z RawImage
        SafeSetTexture(maskObjectPaintedArea, maskSprite, "maskObjectPaintedArea");
        SafeSetTexture(maskObjectGenerated, maskSprite, "maskObjectGenerated");
    }
    
    /// <summary>
    /// Ekstraktuje Texture2D z fragmentu Sprite (dla atlasów i sprite'ów)
    /// </summary>
    private Texture2D GetTextureFromSprite(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogWarning("GetTextureFromSprite: sprite jest null!");
            return null;
        }
        
        Rect rect = sprite.rect;
        Texture2D sourceTexture = sprite.texture;
        
        // Sprawdź czy sprite używa całej tekstury
        if (rect.width == sourceTexture.width && rect.height == sourceTexture.height && rect.x == 0 && rect.y == 0)
        {
            // Sprite używa całej tekstury - możemy zwrócić bezpośrednio
            return sourceTexture;
        }
        
        // Sprite jest częścią atlasu - wytnij odpowiedni fragment
        Texture2D newTexture = new Texture2D((int)rect.width, (int)rect.height);
        Color[] pixels = sourceTexture.GetPixels(
            (int)rect.x, 
            (int)rect.y, 
            (int)rect.width, 
            (int)rect.height
        );
        newTexture.SetPixels(pixels);
        newTexture.Apply();
        
        return newTexture;
    }
    
    /// <summary>
    /// Przełącza między panelami - włącza jeden, wyłącza drugi
    /// </summary>
    public void TogglePanels()
    {
        if (choosePanel != null && paintingPanel != null)
        {
            bool choosePanelActive = choosePanel.activeSelf;
            
            choosePanel.SetActive(!choosePanelActive);
            paintingPanel.SetActive(choosePanelActive);
        }
    }
}
