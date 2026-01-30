using UnityEngine;
using UnityEngine.EventSystems;


public class TargetMouseObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject uiFurnaceHUD;

    private MeshRenderer meshRenderer;
    
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        
    
    }

    public void OnPointerDown(PointerEventData args)
    {
        
    }

    public void OnPointerUp(PointerEventData args)
    {
     
    }

    public void OnPointerClick(PointerEventData args)
    {
        uiFurnaceHUD.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData args)
    {
           meshRenderer.material.color = Color.red;
        Debug.Log("Mouse Entered");
    }

    public void OnPointerExit(PointerEventData args)
    {
        meshRenderer.material.color = Color.white;
    }
}
