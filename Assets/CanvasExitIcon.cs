using UnityEngine;

public class CanvasExitIcon : MonoBehaviour
{
    public Canvas canvasToClose;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        if (canvasToClose != null)
        {
            canvasToClose.enabled = false;
            Debug.Log("CanvasExitIcon: Closed canvas " + canvasToClose.name);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Debug.LogError("CanvasExitIcon: No canvas assigned to close!");
        }
    }
}
