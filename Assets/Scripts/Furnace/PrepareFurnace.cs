using UnityEngine;
using UnityEngine.UI;

public class PrepareFurnace : MonoBehaviour
{
   public GameObject buttonDecorateToBake;
   public GameObject decorativePanel;
  

   public void PrepareBake()
   {
      ProgressManager.instance.canBakePanel = true;
      ProgressManager.instance.canDecoratePanel = false;
      ProgressManager.instance.canPaintPanel = false;
      ProgressManager.instance.canChoosePanel = false;
      decorativePanel.SetActive(false);
      BlockUI.instance.HideCursorAndUnpauseGame();
   }
}
