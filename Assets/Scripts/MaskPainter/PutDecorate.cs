using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PutDecorate : MonoBehaviour, IPointerDownHandler
{
    [System.Serializable]
    public class DecorationItem
    {
        public string name;      // Np. "Diament", "Pioro", "Kwiat"
        public GameObject prefab;
        public int maxCount;     // Limit (np. 5)
        [HideInInspector] public int currentCount; // Ile już postawiono
    }

    [Header("Maska - obszar")]
    public RawImage maskDisplay;     // Komponent wyświetlający maskę (z niego weźmiemy teksturę)
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

    // Funkcja do wywoływania z przycisków UI (np. Button Diament -> OnClick -> SelectDecoration(0))
    public void SelectDecoration(int index)
    {
        if (index >= 0 && index < decorations.Count)
        {
            selectedDecorationIndex = index;
            Debug.Log($"Wybrano dekorację: {decorations[index].name} (Dostępne: {decorations[index].maxCount - decorations[index].currentCount})");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("PutDecorate: OnPointerDown detected!");
        
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
        // Sprawdź czy mamy teksturę maski
        if (maskDisplay == null || maskDisplay.texture == null)
        {
            Debug.LogWarning("Brak przypisanej RawImage z maską!");
            return;
        }

        Texture2D maskTex = maskDisplay.texture as Texture2D;
        if (maskTex == null) return; // To nie jest Texture2D (np. RenderTexture)

        // 1. Przelicz koordynaty UI na koordynaty Tekstury
        // Zakładamy, że RectTransform ma ten sam rozmiar co tekstura, lub musimy skalować UV
        float normalizedX = (localPosition.x + rectTransform.rect.width * 0.5f) / rectTransform.rect.width;
        float normalizedY = (localPosition.y + rectTransform.rect.height * 0.5f) / rectTransform.rect.height;

        // Sprawdź czy kliknięcie jest w obrębie prostokąta
        if (normalizedX < 0 || normalizedX > 1 || normalizedY < 0 || normalizedY > 1)
            return;

        // 2. Sprawdź piksel maski
        Color pixel = maskTex.GetPixelBilinear(normalizedX, normalizedY);
        
        // Sprawdzamy jasność (lub alfę)
        float brightness = (pixel.r + pixel.g + pixel.b) / 3f;
        // Opcjonalnie sprawdź alfę jeśli używasz przezroczystości
        if (pixel.a < maskThreshold) brightness = 0; 
        
        if (brightness > maskThreshold)
        {
            SpawnDecoration(localPosition);
        }
        else
        {
            Debug.Log("Poza maską (za ciemno)!");
        }
    }

    void SpawnDecoration(Vector2 localPosition)
    {
        if (selectedDecorationIndex < 0 || selectedDecorationIndex >= decorations.Count) return;

        DecorationItem item = decorations[selectedDecorationIndex];

        // Sprawdź limit
        if (item.currentCount >= item.maxCount)
        {
            Debug.Log($"Osiągnięto limit dla {item.name}! ({item.maxCount})");
            return;
        }

        // 1. Stwórz obiekt
        if (item.prefab != null && decorationsContainer != null)
        {
            GameObject newDeco = Instantiate(item.prefab, decorationsContainer);
            
            // 2. Ustaw pozycję
            newDeco.transform.localPosition = localPosition;
            
            // 3. Zwiększ licznik
            item.currentCount++;
            Debug.Log($"Postawiono {item.name}. Pozostało: {item.maxCount - item.currentCount}");
        }
    }
    
    // Funkcja do resetowania dekoracji
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
    }
}