using UnityEngine;
using UnityEngine.UI;

public class BlockUI : MonoBehaviour
{

    public static BlockUI instance;

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        HideCursorAndUnpauseGame();
    }
    //show cursor, pause game
    public void ShowCursorAndPauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    //hide cursor, unpause game
    public void HideCursorAndUnpauseGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
    }
}