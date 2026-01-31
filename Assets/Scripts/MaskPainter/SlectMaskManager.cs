using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectMaskManager : MonoBehaviour, IPointerDownHandler
{
    public Image choosedMaskImage;
    public Image choosedOutlineImage; // Obiekt, do którego przypisujemy wybraną maskę
    
    [Header("Synchronizacja z LoadMaskButton")]
    [Tooltip("Opcjonalne - przypisz LoadMaskButton który ma zaktualizować dane masek")]
    public LoadMaskButton loadMaskButton;
    
    [Header("Źródła (Skąd brać wygląd)")]
    [Tooltip("Obiekt z którego bierzemy maskę (np. dziecko 'Icon'). Jeśli puste, spróbuje znaleźć automatycznie.")]
    public Image sourceMaskImage;
    [Tooltip("Obiekt z którego bierzemy obrys (np. dziecko 'Outline'). Jeśli puste, spróbuje znaleźć automatycznie.")]
    public Image sourceOutlineImage;
    
    private Image buttonImage; // Image tego przycisku (może być tłem)
    private Sprite selectedSprite; // Aktualnie wybrana maska

    void Awake()
    {
        // 1. Próba znalezienia źródła maski (Icon)
        // Jeśli nie przypisano ręcznie w inspektorze:
        if (sourceMaskImage == null)
        {
            // A. Czy istnieje dziecko "Icon"? (Jeśli skrypt jest na Buttonie)
            Transform iconTrans = transform.Find("Icon");
            if (iconTrans != null)
            {
                sourceMaskImage = iconTrans.GetComponent<Image>();
            }
            // B. Jeśli nie ma dziecka "Icon", zakładamy, że TEN OBIEKT to Icon (Jeśli skrypt jest na Iconie)
            else
            {
                sourceMaskImage = GetComponent<Image>();
            }
        }
        
        // 2. Próba znalezienia źródła obrysu (Outline)
        if (sourceOutlineImage == null)
        {
            // A. Szukamy "Outline" bezpośrednio w dzieciach (dla przypadku Skrypt na Iconie -> dziecko Outline)
            Transform outlineTrans = transform.Find("Outline");
            
            // B. Jeśli nie znaleziono, szukamy głębiej (dla przypadku Skrypt na Buttonie -> Icon -> Outline)
            if (outlineTrans == null && transform.childCount > 0)
            {
                 // Sprawdź czy któreś z dzieci ma Outline
                 foreach(Transform child in transform)
                 {
                     Transform deepOutline = child.Find("Outline");
                     if (deepOutline != null)
                     {
                         outlineTrans = deepOutline;
                         break;
                     }
                 }
            }
             
            if (outlineTrans != null)
            {
                sourceOutlineImage = outlineTrans.GetComponent<Image>();
            }
        }
        
        if (sourceMaskImage == null) Debug.LogWarning($"SelectMaskManager: Nie znaleziono źródła maski w '{name}'!");
        if (sourceOutlineImage == null) Debug.LogWarning($"SelectMaskManager: Nie znaleziono źródła obrysu w '{name}'!");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (sourceMaskImage != null && sourceMaskImage.sprite != null && choosedMaskImage != null)
        {
            selectedSprite = sourceMaskImage.sprite;
            choosedMaskImage.sprite = selectedSprite;
            
            if (sourceOutlineImage != null && choosedOutlineImage != null)
            {
                choosedOutlineImage.sprite = sourceOutlineImage.sprite;
            }
            
            Debug.Log($"Wybrano maskę: {selectedSprite.name}");
            
            // Zaktualizuj dane w LoadMaskButton (jeśli przypisany)
            if (loadMaskButton != null)
            {
                // Przekaż dane (bez zmieniania wyglądu przycisku)
                Sprite outlineSprite = (sourceOutlineImage != null) ? sourceOutlineImage.sprite : null;
                loadMaskButton.SetMaskData(selectedSprite, outlineSprite);
                
                // Teraz odśwież dane i wygeneruj
                loadMaskButton.UpdateMaskData();
            }
        }
    }
    
    /// <summary>
    /// Zwraca teksturę wybranej maski
    /// </summary>
    public Texture2D GetSelectedMaskTexture()
    {
        if (selectedSprite == null)
        {
            Debug.LogWarning("Nie wybrano żadnej maski!");
            return null;
        }
        
        // Konwertuj sprite na Texture2D
        Texture2D texture = selectedSprite.texture;
        
        // Sprawdź czy sprite używa całej tekstury czy tylko jej części
        if (selectedSprite.rect.width != texture.width || selectedSprite.rect.height != texture.height)
        {
            // Sprite jest częścią atlasu - wytnij odpowiedni fragment
            texture = GetTextureFromSprite(selectedSprite);
        }
        
        return texture;
    }
    
    /// <summary>
    /// Ekstraktuje Texture2D z fragmentu Sprite (dla atlasów)
    /// </summary>
    private Texture2D GetTextureFromSprite(Sprite sprite)
    {
        Rect rect = sprite.rect;
        Texture2D tex = new Texture2D((int)rect.width, (int)rect.height);
        Color[] pixels = sprite.texture.GetPixels(
            (int)rect.x, 
            (int)rect.y, 
            (int)rect.width, 
            (int)rect.height
        );
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }
    
    /// <summary>
    /// Sprawdza czy została wybrana jakaś maska
    /// </summary>
    public bool HasSelectedMask()
    {
        return selectedSprite != null;
    }
}