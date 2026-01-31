using UnityEngine;

public class HandlerStoppedMovementPlayer : MonoBehaviour
{
    [Tooltip("Referencja do skryptu gracza. Jeśli puste, znajdzie automatycznie.")]
    public PlayerMovement playerMovement;

    private void Awake()
    {
        
        if (playerMovement == null)
        {
            playerMovement = FindFirstObjectByType<PlayerMovement>();
        }
    }

    private void OnEnable()
    {
        StopPlayerMovment();
    }

    private void OnDisable()
    {
        StartPlayerMovment();
    }

    public void StopPlayerMovment()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void StartPlayerMovment()
    {
        
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

}
