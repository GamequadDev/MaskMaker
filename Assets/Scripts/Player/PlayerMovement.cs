using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Speed Settings")]
    [Tooltip("Speed")]
    public float moveSpeed = 6f;
    
    [Tooltip("Gravity")]
    public float gravity = -9.81f;

    [Header("Camera Settings")]
    [Tooltip("Camera")]
    public Transform playerCamera;
    
    [Tooltip("Mouse sentensivity")]
    public float mouseSensitivity = 2f;
    
    [Tooltip("Max look X")]
    public float lookXLimit = 85f;

    [Header("Trigger Colider")]
    [Tooltip("Trigger")]
    public SphereCollider trigger;

 
    private CharacterController characterController;
    private Vector3 velocity;
    private float rotationX = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerCamera == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
                playerCamera = cam.transform;
            else
                Debug.LogError("No camera");
        }

        if (trigger != null)
        {
            trigger.isTrigger = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            EnableCameraRotation();
        }

        if (playerCamera != null && Cursor.lockState == CursorLockMode.Locked)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            playerCamera.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }

    
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        characterController.Move(move * moveSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    public void EnableCameraRotation()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger Entered: {other.name}");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"Trigger Exited: {other.name}");
    }
}
