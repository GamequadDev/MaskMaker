using UnityEngine;
using UnityEngine.UI;

public class PaintingPanelCursor : MonoBehaviour
{
    [Header("Hardware Cursor (Max 128x128)")]
    public Texture2D paintCursor;  // Twój własny obrazek kursora (np. pędzel)
    public Vector2 hotspot = Vector2.zero;  // Punkt "gorący" kursora (środek kliknięcia)
    
    [Header("UI Cursor (Dowolny rozmiar - POLECANE)")]
    public Image uiCursor;  // UI Image który będzie podążał za myszą
    public Vector2 cursorOffset = Vector2.zero;  // Offset względem pozycji myszy
    
    private RectTransform cursorRect;
    
    private void OnEnable()
    {
        // Jeśli używasz UI Cursor - ukryj domyślny kursor
        if (uiCursor != null)
        {
            cursorRect = uiCursor.GetComponent<RectTransform>();
            uiCursor.gameObject.SetActive(true);
            Cursor.visible = false;
        }
        // Jeśli nie - użyj hardware cursor
        else if (paintCursor != null)
        {
            Cursor.SetCursor(paintCursor, hotspot, CursorMode.Auto);
        }
    }
    
    private void Update()
    {
        // Jeśli używasz UI Cursor - aktualizuj jego pozycję
        if (uiCursor != null && cursorRect != null)
        {
            cursorRect.position = Input.mousePosition + (Vector3)cursorOffset;
        }
    }
    
    private void OnDisable()
    {
        // Ukryj UI Cursor jeśli był używany
        if (uiCursor != null)
        {
            uiCursor.gameObject.SetActive(false);
            Cursor.visible = true;
        }
        
        // Przywróć domyślny hardware cursor
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}