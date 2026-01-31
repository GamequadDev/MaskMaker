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
        
        if (decoratePanel != null)
        {
             decoratePanel.SetActive(true);
             Debug.Log("DecorateButton: Pokazano decoratePanel.");
        }
        else
        {
             Debug.LogError("DecorateButton: Brak referencji do Decorate Panel!");
        }
    }

    private void LoadPaintedMask()
    {
        string filePath = Path.Combine(Application.dataPath, "GeneratedMasks", "painted_mask.png");

        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(fileData))
            {
                if (maskPreview != null)
                {
                    maskPreview.texture = texture;
                }
            }
        }
        else
        {
            Debug.LogWarning($"DecorateButton: Nie znaleziono pliku maski w: {filePath}");
        }
    }
}
