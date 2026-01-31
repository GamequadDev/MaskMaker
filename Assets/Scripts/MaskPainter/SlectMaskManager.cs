using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectMaskManager : MonoBehaviour, IPointerDownHandler
{
    public static SelectMaskManager instance;
    public Image choosedMaskImage;
    public Image choosedOutlineImage; // Obiekt, do którego przypisujemy wybraną maskę
    
    [Header("Synchronizacja z LoadMaskButton")]
    [Tooltip("Opcjonalne - przypisz LoadMaskButton który ma zaktualizować dane masek")]
    public LoadMaskButton loadMaskButton;
    
    private Image buttonImage; // Image tego przycisku
    private Image outlineButtonImage; // Image tego przycisku
    private Sprite selectedSprite; // Aktualnie wybrana maska

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Więcej niż jedna instancja SelectMaskManager!");
        }
        
        buttonImage = GetComponent<Image>();
        
        // Pobierz Image z dziecka tego obiektu (dla outline)
        if (transform.childCount > 0)
        {
            outlineButtonImage = transform.GetChild(0).GetComponent<Image>();
            if (outlineButtonImage == null)
            {
                Debug.LogWarning($"SelectMaskManager: Dziecko '{transform.GetChild(0).name}' nie ma komponentu Image!");
            }
        }
        else
        {
            Debug.LogWarning("SelectMaskManager: Brak dziecka obiektu dla outline!");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonImage != null && buttonImage.sprite != null && choosedMaskImage != null)
        {
            selectedSprite = buttonImage.sprite;
            choosedMaskImage.sprite = selectedSprite;
            choosedOutlineImage.sprite = outlineButtonImage.sprite;
            
            Debug.Log($"Wybrano maskę: {selectedSprite.name}");
            
            // Zaktualizuj dane w LoadMaskButton (jeśli przypisany)
            if (loadMaskButton != null)
            {
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