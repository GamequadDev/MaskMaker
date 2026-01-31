using UnityEngine;
using UnityEngine.UI;

public class ColorSelectorManager : MonoBehaviour
{
    public void SetRed() => MaskLinePainter.currentColor = Color.red;
    public void SetBlue() => MaskLinePainter.currentColor = Color.blue;
    public void SetGreen() => MaskLinePainter.currentColor = new Color32(28, 186, 42, 255); 
    public void setBlack() => MaskLinePainter.currentColor = Color.black;
    public void setYellow() => MaskLinePainter.currentColor = new Color32(255, 227, 0, 255); 
    public void setBeige() => MaskLinePainter.currentColor = new Color32(229, 181, 79, 255); 

    // Funkcja uniwersalna, którą możesz przypisać w Inspektorze do OnClick()
    public void SelectColor(Image buttonImage)
    {
        MaskLinePainter.currentColor = buttonImage.color;
    }
    
}
