using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.IO;

public class PutDecorate : MonoBehaviour, IPointerDownHandler
{
    [System.Serializable]
    public class DecorationItem
    {
        public string name;      
        public GameObject prefab;
        [HideInInspector] public int currentCount; 
    }

    [Header("Maska - Dane")]
    public MaskData maskData;

    [Header("Gracz - Dane")]
    public PlayerData playerData;

    [Header("Maska - obszar")]
    public RawImage maskDisplay;     // Komponent wyświetlający maskę
    public float maskThreshold = 0.1f;
    
    [Header("Kontener")]
    public Transform decorationsContainer; // Gdzie wrzucać nowe obiekty

    [Header("Lista Dekoracji (Ustaw w Inspektorze)")]
    public List<DecorationItem> decorations = new List<DecorationItem>();

    // Aktualnie wybrany indeks dekoracji
    private int selectedDecorationIndex = -1;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Funkcja do wywoływania z przycisków UI
    public void SelectDecoration(int index)
    {
        if (index >= 0 && index < decorations.Count)
        {
            selectedDecorationIndex = index;
            string decoName = decorations[index].name;
            int limit = GetMaxCount(decoName);
            int current = decorations[index].currentCount;
            
            Debug.Log($"Wybrano dekorację: {decoName} (Dostępne: {limit - current}/{limit})");
        }
    }

    // Helper pobierający limit z PlayerData na podstawie nazwy
    private int GetMaxCount(string decorationName)
    {
        if (playerData == null) return 99; // Fallback jeśli brak PlayerData

        switch (decorationName)
        {
            case "Diament": // Obsługa polskich nazw jeśli takie są w liście
            case "Diamond": return playerData.diamondsCount;
            
            case "Pioro":
            case "Feather": return playerData.feathersCount;
            
            case "Kwiat":
            case "Flower": return playerData.flowersCount;
            
            case "Lisc":
            case "Leaf": return playerData.leavesCount;
            
            case "Gwiazdka":
            case "Star": return playerData.starsCount;
            
            default: return 0;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Jeśli nic nie wybrano, nie rób nic
        if (selectedDecorationIndex < 0) return;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            TryPlaceDecorate(localPoint);
        }
    }

    void TryPlaceDecorate(Vector2 localPosition)
    {
        if (maskDisplay == null || maskDisplay.texture == null)
        {
            Debug.LogWarning("Brak przypisanej RawImage z maską!");
            return;
        }

        Texture2D maskTex = maskDisplay.texture as Texture2D;
        if (maskTex == null) return; 

        // Przelicz koordynaty
        float normalizedX = (localPosition.x + rectTransform.rect.width * 0.5f) / rectTransform.rect.width;
        float normalizedY = (localPosition.y + rectTransform.rect.height * 0.5f) / rectTransform.rect.height;

        if (normalizedX < 0 || normalizedX > 1 || normalizedY < 0 || normalizedY > 1)
            return;

        Color pixel = maskTex.GetPixelBilinear(normalizedX, normalizedY);
        
        // Zmieniono logikę: Sprawdzamy tylko Alfę (przezroczystość).
        // Dzięki temu można stawiać dekoracje na czarnym tle, o ile nie jest przezroczyste.
        if (pixel.a > maskThreshold)
        {
            SpawnDecoration(localPosition);
        }
        else
        {
            Debug.Log("Poza maską (przezroczysto)!");
        }
    }

    void SpawnDecoration(Vector2 localPosition)
    {
        if (selectedDecorationIndex < 0 || selectedDecorationIndex >= decorations.Count) return;

        DecorationItem item = decorations[selectedDecorationIndex];
        int maxCount = GetMaxCount(item.name);

        // Sprawdź limit
        if (item.currentCount >= maxCount)
        {
            Debug.Log($"Osiągnięto limit dla {item.name}! (Masz tylko {maxCount})");
            return;
        }

        if (item.prefab != null && decorationsContainer != null)
        {
            GameObject newDeco = Instantiate(item.prefab, decorationsContainer);
            newDeco.transform.localPosition = localPosition;
            
            item.currentCount++;
            
            // Logika aktualizacji MaskData
            if (maskData != null)
            {
                // Używamy nazwy z listy, upewnij się że pasuje do switcha
                string n = item.name;
                
                if(n == "Diamond" || n == "Diament") maskData.diamondsUsed++;
                else if(n == "Feather" || n == "Pioro") maskData.feathersUsed++;
                else if(n == "Flower" || n == "Kwiat") maskData.flowersUsed++;
                else if(n == "Leaf" || n == "Lisc") maskData.leavesUsed++;
                else if(n == "Star" || n == "Gwiazdka") maskData.starsUsed++;
            }

            Debug.Log($"Postawiono {item.name}. Pozostało: {maxCount - item.currentCount}");
        }
    }
    
    public void ClearDecorations()
    {
        if (decorationsContainer != null)
        {
            foreach (Transform child in decorationsContainer)
            {
                Destroy(child.gameObject);
            }
        }
        
        foreach (var item in decorations)
        {
            item.currentCount = 0;
        }
        
        // Resetujemy też w MaskData? (zależy od logiki gry, na razie nie ruszam persistent data)
        // Jeśli chcesz resetować MaskData:
        // if(maskData != null) { maskData.diamondsUsed = 0; ... }
    }

    [ContextMenu("Zapisz Dekoracje do PNG")]
    public void SaveWithDecorations()
    {
        if (maskDisplay == null || maskDisplay.texture == null)
        {
            Debug.LogError("Brak tekstury maski do zapisu!");
            return;
        }

        Texture2D baseTex = maskDisplay.texture as Texture2D;
        if (baseTex == null) return;

        Texture2D resultTex = new Texture2D(baseTex.width, baseTex.height);
        resultTex.SetPixels(baseTex.GetPixels());
        resultTex.Apply();

        foreach (Transform child in decorationsContainer)
        {
            Image img = child.GetComponent<Image>();
            if (img == null || img.sprite == null) continue;

            RectTransform childRect = child.GetComponent<RectTransform>();
            Vector2 localPos = child.localPosition; 
            
            int texX = (int)(localPos.x + baseTex.width * 0.5f);
            int texY = (int)(localPos.y + baseTex.height * 0.5f);
            
            int decoWidth = (int)childRect.rect.width;
            int decoHeight = (int)childRect.rect.height;
            
            int startX = texX - decoWidth / 2;
            int startY = texY - decoHeight / 2;
            
            Sprite sprite = img.sprite;
            Texture2D spriteTex = sprite.texture;
            Rect spriteRect = sprite.rect;

            for (int y = 0; y < decoHeight; y++)
            {
                for (int x = 0; x < decoWidth; x++)
                {
                    int targetX = startX + x;
                    int targetY = startY + y;

                    if (targetX >= 0 && targetX < baseTex.width && targetY >= 0 && targetY < baseTex.height)
                    {
                        float u = (spriteRect.x + ((float)x / decoWidth) * spriteRect.width) / spriteTex.width;
                        float v = (spriteRect.y + ((float)y / decoHeight) * spriteRect.height) / spriteTex.height;
                        
                        Color decoColor = spriteTex.GetPixelBilinear(u, v);
                        
                        if (decoColor.a > 0)
                        {
                            Color baseColor = resultTex.GetPixel(targetX, targetY);
                            Color blendedColor = Color.Lerp(baseColor, decoColor, decoColor.a);
                            resultTex.SetPixel(targetX, targetY, blendedColor);
                        }
                    }
                }
            }
        }
        
        resultTex.Apply();

        // Użyj persistentDataPath - działa zarówno w edytorze jak i w buildzie
        string folderPath = Path.Combine(Application.persistentDataPath, "GeneratedMasks");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        
        // Zapisz z timestampem aby uniknąć nadpisywania
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"painted_mask_{timestamp}.png";
        string path = Path.Combine(folderPath, fileName);
        
        byte[] bytes = resultTex.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        
        Debug.Log($"Zapisano maskę z dekoracjami: {path}");
    }
}