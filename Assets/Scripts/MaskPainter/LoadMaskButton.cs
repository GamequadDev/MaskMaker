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

    [Header("Mask Data")]
    public MaskData maskData;
    
    // Prywatne zmienne do przechowywania wybranej maski (niezależne od wyglądu przycisku)
    private Sprite currentMaskSprite;
    private Sprite currentOutlineSprite;
    private Button myButton;

    void Awake()
    {
        // Już nie potrzebujemy pobierać Image z tego obiektu jako źródła danych
        // Ale możemy zostawić referencje jeśli są potrzebne do czegoś innego
        maskImage = GetComponent<Image>();
        myButton = GetComponent<Button>();
    }
    
    void Start()
    {
        // Zablokuj przycisk na starcie, dopóki gracz czegoś nie wybierze
        if (myButton != null)
        {
            myButton.interactable = false;
        }
    }
    
    /// <summary>
    /// Ustawia maskę do użycia (nie zmienia wyglądu przycisku!)
    /// </summary>
    public void SetMaskData(Sprite mask, Sprite outline)
    {
        currentMaskSprite = mask;
        currentOutlineSprite = outline;
        
        // Odblokuj przycisk skoro mamy wybraną maskę
        if (myButton != null)
        {
            myButton.interactable = true;
        }
        
        Debug.Log($"LoadMaskButton: Ustawiono dane - Maska: {(mask ? mask.name : "null")}, Outline: {(outline ? outline.name : "null")}");
    }
    
    public Sprite GetCurrentOutlineSprite()
    {
        return currentOutlineSprite;
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
    /// Aktualizuje dane masek - używa zapamiętanych sprite'ów (currentMaskSprite)
    /// </summary>
    public void UpdateMaskData()
    {
        Sprite maskSprite = currentMaskSprite;
        Sprite outlineSprite = currentOutlineSprite;
        
        if (maskSprite == null)
        {
            Debug.LogWarning("LoadMaskButton: Nie wybrano żadnej maski (currentMaskSprite jest null)!");
            return;
        }
        
        // Zapisz do wszystkich obiektów docelowych
        SaveToObjects(maskSprite, outlineSprite);
        
        // Zapisz typ maski do danych
        if (maskData != null)
        {
            maskData.typeCurrent = maskSprite.name;
            Debug.Log($"LoadMaskButton: Zapisano typ maski '{maskData.typeCurrent}' do MaskData.");
        }
        else
        {
             Debug.LogWarning("LoadMaskButton: Brak przypisanego MaskData!");
        }
        
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
        
        Debug.Log($"✓ Zaktualizowano dane masek: {maskSprite.name} (mask) + {(outlineSprite ? outlineSprite.name : "null")} (outline)");
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
        
        newTexture.name = sprite.name; // WAŻNE: Zachowaj nazwę (np. "Mask_1") dla generatora!
        
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
