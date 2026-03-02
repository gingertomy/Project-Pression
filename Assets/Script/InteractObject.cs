using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractionObject : MonoBehaviour
{
    [Header("Input & Hold")]
    public Key focusKey = Key.E;
    public float holdDuration = 1.5f;

    [Header("References")]
    public Camera playerCamera;
    public Transform handTransform;
    public CanvasGroup interactionUI; // Le texte "Appuyer sur E"
    public Image radialProgress;      // Le cercle de chargement
    public CanvasGroup crosshair;      // Le viseur central
    public CanvasGroup handsFull;       // Message "Mains pleines" (Text ou UI)

    [Header("Settings")]
    public float interactDistance = 3f;
    public float lerpSpeed = 10f;

    private float holdTimer = 0f;
    private bool isLookingAtObject = false;
    private bool isHoldingKey = false;
    private GameObject currentInteractable;
    private InteractionType currentType;
    private bool isHandOccupied = false;

    // --- ÉVÉNEMENTS ---
    public event Action<GameObject> OnCokePicked;
    public event Action<GameObject> OnPaperPicked;
    public event Action<GameObject> OnHover;     // Pour l'Outline
    public event Action<GameObject> OnNoHover;   // Pour l'Outline

    enum InteractionType { None, Coke, Paper }

    private void Start()
    {
        if (interactionUI != null) interactionUI.alpha = 0f;
        if (radialProgress != null) radialProgress.fillAmount = 0f;
        if (handsFull != null) handsFull.alpha = 0f;
        if (crosshair != null) crosshair.alpha = 1f;
    }

    void Update()
    {
        CheckLook();
        HandleInput();
    }

    void CheckLook()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            InteractionType hitType = GetTypeFromTag(hit.collider.tag);

            if (hitType != InteractionType.None)
            {
                GameObject hitObject = hit.collider.gameObject;

                // GESTION DU CHANGEMENT D'OBJET (HOVER)
                if (currentInteractable != hitObject)
                {
                    // Éteindre l'outline de l'ancien objet
                    if (currentInteractable != null) OnNoHover?.Invoke(currentInteractable);

                    currentInteractable = hitObject;
                    currentType = hitType;
                    isLookingAtObject = true;

                    // Allumer l'outline du nouvel objet (seulement si mains libres)
                    if (!isHandOccupied) OnHover?.Invoke(currentInteractable);
                }

                // GESTION DE L'AFFICHAGE UI
                if (isHandOccupied)
                {
                    if (handsFull != null) handsFull.alpha = 1f;
                    if (interactionUI != null) interactionUI.alpha = 0f;
                    if (crosshair != null) crosshair.alpha = 1f;
                }
                else
                {
                    if (handsFull != null) handsFull.alpha = 0f;
                    if (interactionUI != null) interactionUI.alpha = 1f;
                    if (crosshair != null) crosshair.alpha = 0f;
                }
                return;
            }
        }

        // Si on ne regarde plus rien
        if (isLookingAtObject)
        {
            StopLooking();
        }
    }

    void StopLooking()
    {
        // Éteindre l'outline avant de perdre la référence
        if (currentInteractable != null) OnNoHover?.Invoke(currentInteractable);

        isLookingAtObject = false;
        currentInteractable = null;
        currentType = InteractionType.None;

        if (interactionUI != null) interactionUI.alpha = 0f;
        if (crosshair != null) crosshair.alpha = 1f;
        if (handsFull != null) handsFull.alpha = 0f;

        ResetHold();
    }

    InteractionType GetTypeFromTag(string tag)
    {
        if (tag == "Coke") return InteractionType.Coke;
        if (tag == "Paper") return InteractionType.Paper;
        return InteractionType.None;
    }

    void HandleInput()
    {
        if (isHandOccupied || !isLookingAtObject || currentInteractable == null) return;

        if (Keyboard.current[focusKey].isPressed)
        {
            isHoldingKey = true;
            holdTimer += Time.deltaTime;

            if (radialProgress != null)
                radialProgress.fillAmount = holdTimer / holdDuration;

            if (holdTimer >= holdDuration)
            {
                Pickup();
            }
        }
        else if (isHoldingKey)
        {
            ResetHold();
        }
    }

    void Pickup()
    {
        if (!isHandOccupied)
        {
            GameObject obj = currentInteractable;

            // DÉSACTIVER L'OUTLINE AU RAMASSAGE
            OnNoHover?.Invoke(obj);

            if (obj.TryGetComponent<Rigidbody>(out Rigidbody rb)) rb.isKinematic = true;
            if (obj.TryGetComponent<Collider>(out Collider col)) col.enabled = false;

            obj.transform.SetParent(handTransform);
            StartCoroutine(MoveToHandRoutine(obj));

            if (currentType == InteractionType.Coke) OnCokePicked?.Invoke(obj);
            else if (currentType == InteractionType.Paper) OnPaperPicked?.Invoke(obj);

            isHandOccupied = true;
            StopLooking();
        }
    }

    IEnumerator MoveToHandRoutine(GameObject obj)
    {
        while (obj != null && Vector3.Distance(obj.transform.localPosition, Vector3.zero) > 0.005f)
        {
            obj.transform.localPosition = Vector3.Lerp(obj.transform.localPosition, Vector3.zero, Time.deltaTime * lerpSpeed);
            obj.transform.localRotation = Quaternion.Lerp(obj.transform.localRotation, Quaternion.identity, Time.deltaTime * lerpSpeed);
            yield return null;
        }

        if (obj != null)
        {
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
        }
    }

    void ResetHold()
    {
        holdTimer = 0f;
        isHoldingKey = false;
        if (radialProgress != null) radialProgress.fillAmount = 0f;
    }

    public void SetHandFree()
    {
        isHandOccupied = false;
        if (crosshair != null) crosshair.alpha = 1f;
    }
}