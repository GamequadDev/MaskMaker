using UnityEngine;

/// <summary>
/// NAJPROSTSZA wersja - sprawdza podobieństwo dwóch masek
/// Ładuje maski z plików PNG zapisanych w Assets/GeneratedMasks/
/// </summary>
public class MaskSimilarityChecker : MonoBehaviour
{
    [Header("Ścieżki do plików PNG")]
    [Tooltip("Ścieżka do wygenerowanej maski (wzór)")]
    public string generatedMaskPath = "Assets/GeneratedMasks/generated_mask.png";
    
    [Tooltip("Ścieżka do namalowanej maski (gracza)")]
    public string paintedMaskPath = "Assets/GeneratedMasks/painted_mask.png";
    
    [Tooltip("Opcjonalna maska obszaru - jeśli null, sprawdza wszystkie piksele")]
    public Texture2D maskTexture;
    
    [Header("Wynik")]
    public float similarityPercent = 0f;
    
    /// <summary>
    /// Porównaj maski i zwróć procent podobieństwa
    /// </summary>
    public float CheckSimilarity()
    {
        // Wczytaj tekstury z plików PNG
        Texture2D targetTexture = LoadTextureFromFile(generatedMaskPath);
        Texture2D paintedTexture = LoadTextureFromFile(paintedMaskPath);
        
        if (targetTexture == null || paintedTexture == null)
        {
            Debug.LogError("Nie można wczytać tekstur! Sprawdź czy pliki PNG istnieją w podanych ścieżkach.");
            return 0f;
        }
        
        // Sprawdź czy rozmiary się zgadzają
        if (targetTexture.width != paintedTexture.width || targetTexture.height != paintedTexture.height)
        {
            Debug.LogError($"Rozmiary tekstur się nie zgadzają! Generated: {targetTexture.width}x{targetTexture.height}, Painted: {paintedTexture.width}x{paintedTexture.height}");
            return 0f;
        }
        
        int width = targetTexture.width;
        int height = targetTexture.height;
        
        int totalPixels = 0;
        int matchingPixels = 0;
        
        // Debug - pokaż przykładowe kolory
        bool showDebug = true;
        int debugSamples = 0;
        
        // Sprawdź każdy piksel
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Jeśli jest maska, sprawdź czy piksel jest w środku
                if (maskTexture != null)
                {
                    float u = (float)x / width;
                    float v = (float)y / height;
                    Color maskPixel = maskTexture.GetPixelBilinear(u, v);
                    
                    // Pomiń piksele poza maską
                    if (maskPixel.a < 0.5f) continue;
                }
                
                // Porównaj kolory
                Color targetColor = targetTexture.GetPixel(x, y);
                Color paintedColor = paintedTexture.GetPixel(x, y);
                
                // Sprawdź przezroczystość (czy piksel jest pusty)
                bool targetTransparent = targetColor.a < 0.1f;
                bool paintedTransparent = paintedColor.a < 0.1f;
                
                // Przypadek 1: Oba puste -> Ignorujemy (tło)
                if (targetTransparent && paintedTransparent)
                {
                    continue; 
                }
                
                // Przypadek 2: Niezgodność obecności (jeden jest, drugiego nie ma) -> BŁĄD
                if (targetTransparent != paintedTransparent)
                {
                    // Jeden piksel jest, drugi nie - to duża różnica, więc nie zwiększamy matchingPixels
                    totalPixels++;
                    continue; 
                }
                
                // Przypadek 3: Oba są widoczne -> Porównujemy kolory RGB
                totalPixels++;
                
                float colorDiff = Mathf.Abs(targetColor.r - paintedColor.r) +
                                  Mathf.Abs(targetColor.g - paintedColor.g) +
                                  Mathf.Abs(targetColor.b - paintedColor.b);
                
                if (colorDiff < 0.5f) // Kolory pasują
                {
                    matchingPixels++;
                }
            }
        }
        
        // Oblicz procent
        similarityPercent = totalPixels > 0 ? (float)matchingPixels / totalPixels * 100f : 0f;
        
        Debug.Log($"Podobieństwo: {similarityPercent:F1}% ({matchingPixels}/{totalPixels})");
        
        return similarityPercent;
    }
    
    /// <summary>
    /// Wczytaj teksturę z pliku PNG z poprawnymi ustawieniami importu
    /// </summary>
    /// <summary>
    /// Wczytaj teksturę z pliku PNG (Loading directly from IO to avoid Cache issues)
    /// </summary>
    private Texture2D LoadTextureFromFile(string path)
    {
        if (!System.IO.File.Exists(path))
        {
             Debug.LogError($"Nie znaleziono pliku: {path}");
             return null;
        }

        byte[] fileData = System.IO.File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData); // This auto-resizes
        
        return texture;
    }
    
    /// <summary>
    /// Skonfiguruj import settings dla tekstury aby nie była kompresowana
    /// </summary>
    private void ConfigureTextureImportSettings(string path)
    {
#if UNITY_EDITOR
        UnityEditor.TextureImporter importer = UnityEditor.AssetImporter.GetAtPath(path) as UnityEditor.TextureImporter;
        if (importer != null)
        {
            bool changed = false;
            
            // Bez kompresji - KLUCZOWE dla poprawnego porównywania!
            if (importer.textureCompression != UnityEditor.TextureImporterCompression.Uncompressed)
            {
                importer.textureCompression = UnityEditor.TextureImporterCompression.Uncompressed;
                changed = true;
            }
            
            // Read/Write enabled - potrzebne do GetPixel()
            if (!importer.isReadable)
            {
                importer.isReadable = true;
                changed = true;
            }
            
            // RGBA32 format dla pełnej precyzji kolorów
            if (importer.textureType != UnityEditor.TextureImporterType.Default)
            {
                importer.textureType = UnityEditor.TextureImporterType.Default;
                changed = true;
            }
            
            if (changed)
            {
                Debug.Log($"⚙️ Zaktualizowano import settings dla: {path}");
                UnityEditor.AssetDatabase.ImportAsset(path, UnityEditor.ImportAssetOptions.ForceUpdate);
            }
        }
#endif
    }
    
    /// <summary>
    /// Konwertuj kolor do czytelnego stringa (dla debugowania)
    /// </summary>
    private string ColorToString(Color c)
    {
        return $"RGB({c.r:F2},{c.g:F2},{c.b:F2})";
    }
    
    // Context menu do testowania
    [ContextMenu("Sprawdź Podobieństwo")]
    public void Test()
    {
        CheckSimilarity();
    }
}
