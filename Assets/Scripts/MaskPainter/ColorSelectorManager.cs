using UnityEngine;
using UnityEngine.UI;

public class ColorSelectorManager : MonoBehaviour
{
    public void SetRed() => MaskLinePainter.currentColor = Color.red;
    public void SetBlue() => MaskLinePainter.currentColor = Color.blue;
    public void SetGreen() => MaskLinePainter.currentColor = Color.green;
    public void setBlack() => MaskLinePainter.currentColor = Color.black;
    public void setPurple() => MaskLinePainter.currentColor = Color.magenta;
    public void setWhite() => MaskLinePainter.currentColor = Color.white;

    // Funkcja uniwersalna, którą możesz przypisać w Inspektorze do OnClick()
    public void SelectColor(Image buttonImage)
    {
        MaskLinePainter.currentColor = buttonImage.color;
    }
    
}
