using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MaskLinePainter : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    // Statyczna zmienna, którą zmienia Twój ColorSelectorManager
    public static Color currentColor = Color.black;

    [Header("Ustawienia Pędzla")]
    public int brushSize = 10;
    
    [Header("Maska")]
    [Tooltip("Texture2D definiująca obszar malowania")]
    public Texture2D maskTexture;
    
    [Tooltip("Czy używać kanału alfa czy jasności do określenia obszaru malowania")]
    public bool useAlphaChannel = false; // false = używaj jasności (lepsze dla czarnych masek)
    
    [Tooltip("Próg alfa/jasności (0-1). Powyżej tego progu można malować.")]
    [Range(0f, 1f)]
    public float maskThreshold = 0.1f;
    
    [Header("Wizualizacja Obwódki")]
    [Tooltip("Opcjonalny Image UI do pokazania czarnej obwódki maski")]
    public Image outlineImage;
    
    private RawImage canvasImage;
    private Texture2D drawableTexture;
    private Texture2D processedMask; // Maska przeskalowana do rozmiaru płótna
    private RectTransform rectTransform;

    void Start()
    {
        canvasImage = GetComponent<RawImage>();
        rectTransform = GetComponent<RectTransform>();

        int width = (int)rectTransform.rect.width;
        int height = (int)rectTransform.rect.height;

        drawableTexture = new Texture2D(width, height);
        
        Color[] fillPixels = new Color[width * height];
        for (int i = 0; i < fillPixels.Length; i++) fillPixels[i] = Color.clear; // Przezroczyste tło
        
        drawableTexture.SetPixels(fillPixels);
        drawableTexture.Apply();

        canvasImage.texture = drawableTexture;
        
        // Jeśli maska jest już przypisana w inspektorze, użyj jej
        if (maskTexture != null)
        {
            Initialize(maskTexture);
        }
    }

    /// <summary>
    /// Inicjalizuje malowanie na podstawie podanej maski
    /// </summary>
    public void Initialize(Texture2D newMask)
    {
        if (newMask == null) 
        {
            Debug.LogWarning("MaskLinePainter: Próba inicjalizacji null maską!");
            return;
        }

        maskTexture = newMask;
        string mode = useAlphaChannel ? "ALFA" : "JASNOSC";
        Debug.Log($"MaskLinePainter: Inicjalizacja maska '{maskTexture.name}' | Tryb: {mode}");

        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (canvasImage == null) canvasImage = GetComponent<RawImage>();

        int width = (int)rectTransform.rect.width;
        int height = (int)rectTransform.rect.height;
        
        PrepareMask(width, height);
        
        // Ustaw obwódkę jeśli jest przypisana
        if (outlineImage != null)
        {
            outlineImage.sprite = Sprite.Create(
                maskTexture,
                new Rect(0, 0, maskTexture.width, maskTexture.height),
                new Vector2(0.5f, 0.5f)
            );
        }
    }

    void PrepareMask(int targetWidth, int targetHeight)
    {
        processedMask = new Texture2D(targetWidth, targetHeight);
        
        for (int y = 0; y < targetHeight; y++)
        {
            for (int x = 0; x < targetWidth; x++)
            {
                float u = (float)x / targetWidth;
                float v = (float)y / targetHeight;
                
                Color maskColor = maskTexture.GetPixelBilinear(u, v);
                processedMask.SetPixel(x, y, maskColor);
            }
        }
        
        processedMask.Apply();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Paint(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Paint(eventData);
    }

    private void Paint(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            int x = (int)((localPoint.x + rectTransform.rect.width * 0.5f) * (drawableTexture.width / rectTransform.rect.width));
            int y = (int)((localPoint.y + rectTransform.rect.height * 0.5f) * (drawableTexture.height / rectTransform.rect.height));

            DrawBrush(x, y);
        }
    }

    void DrawBrush(int centerX, int centerY)
    {
        for (int x = centerX - brushSize; x < centerX + brushSize; x++)
        {
            for (int y = centerY - brushSize; y < centerY + brushSize; y++)
            {
                if (x >= 0 && x < drawableTexture.width && y >= 0 && y < drawableTexture.height)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                    if (dist <= brushSize)
                    {
                        if (IsInsideMask(x, y))
                        {
                            drawableTexture.SetPixel(x, y, currentColor);
                        }
                    }
                }
            }
        }
        drawableTexture.Apply();
    }
    
    bool IsInsideMask(int x, int y)
    {
        if (processedMask == null)
            return true;
        
        Color maskPixel = processedMask.GetPixel(x, y);
        
        if (useAlphaChannel)
        {
            // Tryb alfa - sprawdza przezroczystość
            return maskPixel.a > maskThreshold;
        }
        else
        {
            // Tryb jasności - POPRAWNA LOGIKA:
            // JASNY/BIAŁY piksel = MOŻNA malować (obszar maski)
            // CIEMNY/CZARNY piksel = NIE MOŻNA malować (poza maską)
            float brightness = (maskPixel.r + maskPixel.g + maskPixel.b) / 3f;
            return brightness > maskThreshold; // jasne piksele = maluj
        }
    }

    /// <summary>
    /// Zapisz namalowaną teksturę jako plik PNG
    /// </summary>
    [ContextMenu("Zapisz jako PNG")]
    public void SaveAsPNG()
    {
        if (drawableTexture == null)
        {
            Debug.LogError("Brak tekstury do zapisania!");
            return;
        }
        
        // Utwórz folder jeśli nie istnieje
        string folderPath = "Assets/GeneratedMasks";
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
            Debug.Log($"Utworzono folder: {folderPath}");
        }
        
        // Zapisz jako PNG
        byte[] pngData = drawableTexture.EncodeToPNG();
        string filePath = folderPath + "/painted_mask.png";
        System.IO.File.WriteAllBytes(filePath, pngData);
        
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        
        // Ustaw import settings aby tekstura nie była kompresowana
        UnityEditor.TextureImporter importer = UnityEditor.AssetImporter.GetAtPath(filePath) as UnityEditor.TextureImporter;
        if (importer != null)
        {
            importer.textureCompression = UnityEditor.TextureImporterCompression.Uncompressed;
            importer.isReadable = true;
            UnityEditor.AssetDatabase.ImportAsset(filePath, UnityEditor.ImportAssetOptions.ForceUpdate);
        }
        
        Debug.Log($"✓ Zapisano namalowaną maskę: {filePath}");
#else
        Debug.Log($"✓ Zapisano namalowaną maskę: {filePath}");
#endif
    }
}

