using System;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    public float mouseSensitivity = 100f;
    public Transform playerBody;

    [SerializeField]public float targetHeightOffset = 0.5f; 
    public float transitionSpeed = 5f;    

    private float xRotation = 0f;
    private float defaultYPos;

    public event Action OnCameraHigh;
    public event Action OnCameraLow;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        defaultYPos = transform.localPosition.y;
    }

    void Update()
    {
 
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

  
        float currentHeight = transform.localPosition.y;
        float targetY;

        if (Input.GetKey(KeyCode.Space))
        {
            targetY = defaultYPos + targetHeightOffset;
            OnCameraHigh?.Invoke();
        }
        else
        {

            targetY = defaultYPos;
            OnCameraLow?.Invoke();
        }


        float newY = Mathf.Lerp(currentHeight, targetY, Time.deltaTime * transitionSpeed);

        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}