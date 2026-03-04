using System;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float mouseSensitivity = 2f; // Réduit car on enlève deltaTime
    public Transform playerBody;

    [SerializeField] public float targetHeightOffset = 0.5f;
    public float transitionSpeed = 5f;

    private float xRotation = 0f;
    private float defaultYPos;
    private bool isCurrentlyHigh = false; // Pour éviter de spam les events

    public event Action OnCameraHigh;
    public event Action OnCameraLow;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        defaultYPos = transform.localPosition.y;
    }

    void Update()
    {
        // 1. ROTATION (Sans Time.deltaTime pour la souris)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

        // 2. LOGIQUE DE HAUTEUR (Espace)
        float targetY;

        if (Input.GetKey(KeyCode.Space))
        {
            targetY = defaultYPos + targetHeightOffset;

            // On n'envoie l'event qu'au moment où on passe en "High"
            if (!isCurrentlyHigh)
            {
                isCurrentlyHigh = true;
                OnCameraHigh?.Invoke();
            }
        }
        else
        {
            targetY = defaultYPos;

            // On n'envoie l'event qu'au moment où on repasse en "Low"
            if (isCurrentlyHigh)
            {
                isCurrentlyHigh = false;
                OnCameraLow?.Invoke();
            }
        }

        // 3. LERP DE LA POSITION (Ici Time.deltaTime est nécessaire car c'est un mouvement fluide)
        float newY = Mathf.Lerp(transform.localPosition.y, targetY, Time.deltaTime * transitionSpeed);
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);

        // 4. CURSEUR
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}