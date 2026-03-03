using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractionObject : MonoBehaviour
{
    [Header("Input & Hold")]
    public Key focusKey = Key.E;
    public Key toggleKey = Key.R; 
    public float holdDuration = 1.5f;

    [Header("References")]
    public Camera playerCamera;
    public Transform handTransform;
    public CanvasGroup interactionUI;
    public Image radialProgress;
    public CanvasGroup crosshair;
    public CanvasGroup handsFull;

    [Header("UI Inventory Icons")]
    [SerializeField] private CanvasGroup _objectIcon;
  
    [SerializeField] private CanvasGroup _placeholderIcon;

    [Header("Settings")]
    public float interactDistance = 3f;
    public float lerpSpeed = 10f;

    private float holdTimer = 0f;
    private bool isLookingAtObject = false;
    private bool isHoldingKey = false;
    private GameObject currentInteractable;
    private InteractionType currentType;
    private InteractionType typeInInventory; 

    public bool isHandOccupied = false;
    public bool isObjectHidden = false;
    private GameObject objectInInventory;
    [SerializeField] private AudioDispatcher _audioDispatcher;

    
    public event Action<GameObject> OnCokePicked;
    public event Action<GameObject> OnPaperPicked;
    public event Action<GameObject> OnHover;
    public event Action<GameObject> OnNoHover;

    enum InteractionType { 
        None, 
        Burger,
        Assiette,
        Classeur,
        Ciseau,
        Crayon,
        Agraffeuse,
        Tasse,
        Starbucks,
        Cadre,
        Peluche,
        Paper,
        Poubelle,
        Pokeball,
        Carton,
        Chat
    }

    private void Start()
    {
        
        if (interactionUI != null) interactionUI.alpha = 0f;
        if (radialProgress != null) radialProgress.fillAmount = 0f;
        if (handsFull != null) handsFull.alpha = 0f;
        if (crosshair != null) crosshair.alpha = 1f;

        
        if (_placeholderIcon != null) _placeholderIcon.alpha = 0f;
        if (_objectIcon != null) _objectIcon.alpha = 0f;
        
    }

    void Update()
    {
        CheckLook();
        HandleInput();
        HandleToggle();
    }

    void HandleToggle()
    {
        
        if (Keyboard.current[toggleKey].wasPressedThisFrame && isHandOccupied && objectInInventory != null)
        {
            isObjectHidden = !isObjectHidden;

            if (isObjectHidden)
            {
                _audioDispatcher.PlayAudio(AudioType.Ranger);
                objectInInventory.SetActive(false);
                if (_placeholderIcon != null) _placeholderIcon.alpha = 1f;


                _objectIcon.alpha = 1f;
                

                Debug.Log("Objet rangé dans l'inventaire");
            }
            else
            {
               _audioDispatcher.PlayAudio(AudioType.Ranger);
                if (_placeholderIcon != null) _placeholderIcon.alpha = 0f;
                if (_objectIcon != null) _objectIcon.alpha = 0f;


                objectInInventory.SetActive(true);
                objectInInventory.transform.localPosition = Vector3.zero;
                objectInInventory.transform.localRotation = Quaternion.identity;

                Debug.Log("Objet sorti de l'inventaire");
            }
        }
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

                if (currentInteractable != hitObject)
                {
                    if (currentInteractable != null) OnNoHover?.Invoke(currentInteractable);
                    currentInteractable = hitObject;
                    currentType = hitType;
                    isLookingAtObject = true;

                    if (!isHandOccupied) OnHover?.Invoke(currentInteractable);
                }

                
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

        if (isLookingAtObject) StopLooking();
    }

    void StopLooking()
    {
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
        if (tag == "Coke") return InteractionType.Burger;
        if (tag == "Paper") return InteractionType.Paper;
        if (tag == "Assiette") return InteractionType.Assiette;
            if (tag == "Classeur") return InteractionType.Classeur;
            if (tag == "Ciseau") return InteractionType.Ciseau;
            if (tag == "Crayon") return InteractionType.Crayon;
            if (tag == "Agraffeuse") return InteractionType.Agraffeuse;
            if (tag == "Tasse") return InteractionType.Tasse;
            if (tag == "Starbucks") return InteractionType.Starbucks;
            if (tag == "Cadre") return InteractionType.Cadre;
            if (tag == "Peluche") return InteractionType.Peluche;
            if (tag == "Poubelle") return InteractionType.Poubelle;
            if (tag == "Pokeball") return InteractionType.Pokeball;
            if (tag == "Carton") return InteractionType.Carton;
            if (tag == "Chat") return InteractionType.Chat;

        return InteractionType.None;
    }

    void HandleInput()
    {
        if (isHandOccupied || !isLookingAtObject || currentInteractable == null) return;

        if (Keyboard.current[focusKey].isPressed)
        {
            isHoldingKey = true;
            holdTimer += Time.deltaTime;
            if (radialProgress != null) radialProgress.fillAmount = holdTimer / holdDuration;

            if (holdTimer >= holdDuration) Pickup();
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
            _audioDispatcher.PlayAudio(AudioType.Grab);
            GameObject obj = currentInteractable;
            OnNoHover?.Invoke(obj);

            if (obj.TryGetComponent<Rigidbody>(out Rigidbody rb)) rb.isKinematic = true;
            if (obj.TryGetComponent<Collider>(out Collider col)) col.enabled = false;

            obj.transform.SetParent(handTransform);
            StartCoroutine(MoveToHandRoutine(obj));

            
            typeInInventory = currentType;

            

            objectInInventory = obj;
            isHandOccupied = true;
            isObjectHidden = false;

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
        objectInInventory = null;
        typeInInventory = InteractionType.None;

        
        if (_placeholderIcon != null) _placeholderIcon.alpha = 0f;
        if (_objectIcon != null) _objectIcon.alpha = 0f;

        if (crosshair != null) crosshair.alpha = 1f;
    }
}