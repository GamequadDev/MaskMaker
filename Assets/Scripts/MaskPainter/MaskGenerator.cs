using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generuje losowo pokolorowana maskę używając kolorów dostępnych w ColorSelectorManager
/// </summary>
public class MaskGenerator : MonoBehaviour
{
    [Header("Ustawienia Generacji")]
    [Tooltip("Maska definiująca obszar do kolorowania")]
    public Texture2D maskTexture;
    
    [Tooltip("Czy używać kanału alfa czy jasności do określenia obszaru")]
    public bool useAlphaChannel = false; // false = używaj jasności (lepsze dla czarnych masek)
    
    [Tooltip("Próg alfa/jasności (0-1). Powyżej tego progu będzie kolorowane.")]
    [Range(0f, 1f)]
    public float maskThreshold = 0.1f;
    
    [Header("Kolory do Losowania")]
    [Tooltip("Lista kolorów z których będzie losowane - odpowiada kolorom z ColorSelectorManager")]
    public Color[] availableColors = new Color[]
    {
        Color.red,      // SetRed()
        Color.blue,     // SetBlue()
        Color.green,    // SetGreen()
        Color.black,    // setBlack()
        Color.magenta,  // setPurple()
        Color.white     // setWhite()
    };
    
    [Header("Parametry Generacji")]
    [Tooltip("Rozmiar obszarów kolorowych (większa wartość = większe plamy koloru)")]
    [Range(5, 50)]
    public int regionSize = 20;
    
    [Tooltip("Opcjonalny RawImage do wyświetlenia wygenerowanej maski")]
    public RawImage previewImage;
    

    
    [Header("Wynik")]
    [Tooltip("Wygenerowana tekstura - można ją później porównać z namalowaną")]
    public Texture2D generatedTexture;
    
    private int textureWidth = 512;
    private int textureHeight = 512;
    

    
    /// <summary>
    /// Generuje losowo pokolorowaną maskę
    /// </summary>
    /// <summary>
    /// Generuje losowo pokolorowaną maskę na podstawie podanej tekstury
    /// </summary>
    public void GenerateRandomMask(Texture2D newMaskTexture)
    {
        if (newMaskTexture != null)
        {
            maskTexture = newMaskTexture;
            string mode = useAlphaChannel ? "ALFA" : "JASNOSC";
            Debug.Log($"MaskGenerator: Otrzymano nową maskę '{maskTexture.name}' | Tryb: {mode}");
        }
        
        GenerateRandomMask();
    }
    
    /// <summary>
    /// Generuje losowo pokolorowaną maskę używając aktualnie przypisanej maskTexture
    /// </summary>
    public void GenerateRandomMask()
    {
        
        if (maskTexture == null)
        {
            Debug.LogError("Brak maskTexture! Wybierz maskę lub przypisz teksturę maski w inspektorze.");
            return;
        }
        
        if (availableColors == null || availableColors.Length == 0)
        {
            Debug.LogError("Brak dostępnych kolorów!");
            return;
        }
        
        // Utwórz nową teksturę
        generatedTexture = new Texture2D(textureWidth, textureHeight);
        
        // Wypełnij przezroczystym tłem
        Color[] fillPixels = new Color[textureWidth * textureHeight];
        for (int i = 0; i < fillPixels.Length; i++)
        {
            fillPixels[i] = Color.clear; // Przezroczyste tło zamiast białego
        }
        generatedTexture.SetPixels(fillPixels);
        
        // Generuj losowe regiony kolorów
        GenerateColorRegions();
        
        generatedTexture.Apply();
        
        // Pokaż podgląd jeśli przypisany
        if (previewImage != null)
        {
            previewImage.texture = generatedTexture;
        }
        
        Debug.Log($"Wygenerowano losową maskę rozmiaru {textureWidth}x{textureHeight}");
    }
    
    /// <summary>
    /// Generuje losowe regiony kolorów na masce - ZBALANSOWANA WERSJA
    /// Tworzy zarówno pionowe jak i poziome sekcje dla większej różnorodności
    /// </summary>
    private void GenerateColorRegions()
    {
        // Losowo wybierz sposób generacji
        int generationType = Random.Range(0, 3);
        
        switch (generationType)
        {
            case 0:
                // Pionowe sekcje (3-5 pasów)
                GenerateVerticalSections();
                break;
            case 1:
                // Poziome sekcje (3-5 pasów)
                GenerateHorizontalSections();
                break;
            case 2:
                // Siatka bloków (2x2 lub 3x2)
                GenerateGridBlocks();
                break;
        }
    }
    
    /// <summary>
    /// Generuje pionowe sekcje
    /// </summary>
    private void GenerateVerticalSections()
    {
        int numberOfSections = Random.Range(3, 6);
        float sectionWidth = textureWidth / (float)numberOfSections;
        
        for (int sectionIndex = 0; sectionIndex < numberOfSections; sectionIndex++)
        {
            Color sectionColor = availableColors[Random.Range(0, availableColors.Length)];
            int startX = Mathf.RoundToInt(sectionIndex * sectionWidth);
            int endX = Mathf.RoundToInt((sectionIndex + 1) * sectionWidth);
            
            FillRectangle(startX, 0, endX, textureHeight, sectionColor);
        }
    }
    
    /// <summary>
    /// Generuje poziome sekcje
    /// </summary>
    private void GenerateHorizontalSections()
    {
        int numberOfSections = Random.Range(3, 6);
        float sectionHeight = textureHeight / (float)numberOfSections;
        
        for (int sectionIndex = 0; sectionIndex < numberOfSections; sectionIndex++)
        {
            Color sectionColor = availableColors[Random.Range(0, availableColors.Length)];
            int startY = Mathf.RoundToInt(sectionIndex * sectionHeight);
            int endY = Mathf.RoundToInt((sectionIndex + 1) * sectionHeight);
            
            FillRectangle(0, startY, textureWidth, endY, sectionColor);
        }
    }
    
    /// <summary>
    /// Generuje siatkę bloków (2x2, 3x2, lub 2x3)
    /// </summary>
    private void GenerateGridBlocks()
    {
        int gridCols = Random.Range(2, 4); // 2 lub 3 kolumny
        int gridRows = Random.Range(2, 4); // 2 lub 3 rzędy
        
        float blockWidth = textureWidth / (float)gridCols;
        float blockHeight = textureHeight / (float)gridRows;
        
        for (int row = 0; row < gridRows; row++)
        {
            for (int col = 0; col < gridCols; col++)
            {
                Color blockColor = availableColors[Random.Range(0, availableColors.Length)];
                
                int startX = Mathf.RoundToInt(col * blockWidth);
                int endX = Mathf.RoundToInt((col + 1) * blockWidth);
                int startY = Mathf.RoundToInt(row * blockHeight);
                int endY = Mathf.RoundToInt((row + 1) * blockHeight);
                
                FillRectangle(startX, startY, endX, endY, blockColor);
            }
        }
    }
    
    /// <summary>
    /// Wypełnia prostokąt kolorem z małą losowością na krawędziach
    /// </summary>
    private void FillRectangle(int startX, int startY, int endX, int endY, Color color)
    {
        for (int x = startX; x < endX && x < textureWidth; x++)
        {
            for (int y = startY; y < endY && y < textureHeight; y++)
            {
                if (IsInsideMask(x, y))
                {
                    // Dodaj małą losowość na granicach
                    bool isEdge = (x == startX || x == endX - 1 || y == startY || y == endY - 1);
                    
                    if (isEdge)
                    {
                        if (Random.value > 0.2f) // 80% szans na pomalowanie krawędzi
                        {
                            generatedTexture.SetPixel(x, y, color);
                        }
                    }
                    else
                    {
                        generatedTexture.SetPixel(x, y, color);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Sprawdza czy piksel jest wewnątrz maski
    /// </summary>
    bool IsInsideMask(int x, int y)
    {
        if (maskTexture == null)
            return true;
        
        // Przelicz współrzędne na UV
        float u = (float)x / textureWidth;
        float v = (float)y / textureHeight;
        
        Color maskPixel = maskTexture.GetPixelBilinear(u, v);
        
        if (useAlphaChannel)
        {
            // Tryb alfa - sprawdza przezroczystość
            return maskPixel.a > maskThreshold;
        }
        else
        {
            // Tryb jasności - POPRAWNA LOGIKA:
            // JASNY/BIAŁY piksel = MOŻNA generować (obszar maski)
            // CIEMNY/CZARNY piksel = NIE MOŻNA generować (poza maską)
            float brightness = (maskPixel.r + maskPixel.g + maskPixel.b) / 3f;
            return brightness > maskThreshold; // jasne piksele = generuj
        }
    }
    
    /// <summary>
    /// Ustawia rozmiar generowanej tekstury
    /// </summary>
    public void SetTextureSize(int width, int height)
    {
        textureWidth = width;
        textureHeight = height;
    }
    
    /// <summary>
    /// Zapisz wygenerowaną teksturę jako plik PNG
    /// Skaluje x2 (512x512 → 1024x1024) aby pasowała do rozmiaru namalowanej maski
    /// </summary>
    [ContextMenu("Zapisz jako PNG")]
    public void SaveAsPNG()
    {
        if (generatedTexture == null)
        {
            Debug.LogError("Brak wygenerowanej tekstury! Najpierw wygeneruj maskę.");
            return;
        }
        
        // Utwórz folder jeśli nie istnieje
        string folderPath = "Assets/GeneratedMasks";
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
            Debug.Log($"Utworzono folder: {folderPath}");
        }
        
        // Skaluj teksturę x2 (512x512 → 1024x1024)
        int targetWidth = generatedTexture.width * 2;
        int targetHeight = generatedTexture.height * 2;
        Texture2D scaledTexture = ScaleTexture(generatedTexture, targetWidth, targetHeight);
        
        // Zapisz jako PNG
        byte[] pngData = scaledTexture.EncodeToPNG();
        string filePath = folderPath + "/generated_mask.png";
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
        
        Debug.Log($"✓ Zapisano wygenerowaną maskę: {filePath} (skalowana {generatedTexture.width}x{generatedTexture.height} → {targetWidth}x{targetHeight})");
#else
        Debug.Log($"✓ Zapisano wygenerowaną maskę: {filePath}");
#endif
    }
    
    /// <summary>
    /// Skaluje teksturę do nowego rozmiaru używając nearest neighbor
    /// </summary>
    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight);
        
        for (int y = 0; y < targetHeight; y++)
        {
            for (int x = 0; x < targetWidth; x++)
            {
                // Nearest neighbor - pobierz kolor z oryginalnej tekstury
                float u = (float)x / targetWidth;
                float v = (float)y / targetHeight;
                Color pixelColor = source.GetPixelBilinear(u, v);
                result.SetPixel(x, y, pixelColor);
            }
        }
        
        result.Apply();
        return result;
    }
    
    /// <summary>
    /// Generuje maskę przy starcie gry (opcjonalne)
    /// </summary>
    [ContextMenu("Generate Random Mask")]
    public void GenerateManual()
    {
        GenerateRandomMask();
    }
}
