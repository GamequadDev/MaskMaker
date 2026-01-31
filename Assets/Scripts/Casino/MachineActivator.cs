using UnityEngine;

public class MachineActivator : MonoBehaviour
{
    public Canvas machineUICanvas;
    bool isPlayerInRange = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        machineUICanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            machineUICanvas.enabled = true;
            Debug.Log("Toggled machine UI canvas: " + machineUICanvas.enabled);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        if(isPlayerInRange && Input.GetKeyDown(KeyCode.Escape))
        {
            machineUICanvas.enabled = false;
            Debug.Log("Toggled machine UI canvas: " + machineUICanvas.enabled);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the machine activator area.");
            // Here you can add code to activate the machine

            isPlayerInRange = true;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited the machine activator area.");
            // Here you can add code to deactivate the machine
            isPlayerInRange = false;
        }
    }
}
