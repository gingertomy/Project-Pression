using System;
using UnityEngine;
using UnityEngine.UI;

public class Work : MonoBehaviour
{
    [SerializeField] private InteractionObject _interactionObject;

    [Header("Work Stats")]
    [SerializeField] private float _workValue = 10f;
    [SerializeField] private float _decaySpeed = 1f;
    [SerializeField] private float _initialWorkValue = 10f;
    [SerializeField] private float _autoWorkSpeed = 2f;
    [SerializeField] private MoveCamera _moveCamera;
    [SerializeField] private GameObject _textWork;

    private bool _isWorkingAutomatically = false;
    private bool _isCameraHigh = false; // Nouvelle condition

    public Image workRadialImage;

    public event Action StartWorking;
    public event Action StopWorking;
    public event Action BossArrival;

    private void Start()
    {
        _workValue = _initialWorkValue;

        _textWork.SetActive(false);

        if (_interactionObject == null)
            _interactionObject = FindFirstObjectByType<InteractionObject>();
    }

    private void OnEnable()
    {
        // On s'abonne aux événements de la caméra
        if (_moveCamera != null)
        {
            _moveCamera.OnCameraHigh += SetCameraHigh;
            _moveCamera.OnCameraLow += SetCameraLow;
        }
    }

    private void OnDisable()
    {
        // On se désabonne pour éviter les erreurs
        if (_moveCamera != null)
        {
            _moveCamera.OnCameraHigh -= SetCameraHigh;
            _moveCamera.OnCameraLow -= SetCameraLow;
        }
    }

    private void SetCameraHigh() => _isCameraHigh = true;
    private void SetCameraLow() => _isCameraHigh = false;

    private void Update()
    {
        HandleAutomaticWork();
        HandleUI();
    }

    private void HandleAutomaticWork()
    {
        // CONDITION MISE À JOUR : 
        // Mains libres ET caméra en position BASSE (Low)
        bool canWork = (_interactionObject != null) &&
                       (!_interactionObject.isHandOccupied || _interactionObject.isObjectHidden) &&
                       !_isCameraHigh; // Si la caméra est High, canWork devient false

        if (canWork)
        {
            if (!_isWorkingAutomatically)
            {
                _isWorkingAutomatically = true;
                StartWorking?.Invoke();
                _textWork.SetActive(true);
                Debug.Log("Travail ON : Caméra basse, l'aiguille monte.");
            }

            // Remplit la barre vers 10 (ou descend vers 0 selon ta logique de Game Over)
            _workValue = Mathf.MoveTowards(_workValue, _initialWorkValue, _autoWorkSpeed * Time.deltaTime);
        }
        else
        {
            if (_isWorkingAutomatically)
            {
                _isWorkingAutomatically = false;
                StopWorking?.Invoke();
                _textWork.SetActive(false);
                Debug.Log("Travail OFF : Caméra haute ou mains prises.");
            }

            // On perd de la progression si on ne travaille pas
            _workValue -= _decaySpeed * Time.deltaTime;
        }

        _workValue = Mathf.Max(_workValue, 0);

        // Si la valeur tombe à 0 (Game Over)
        if (_workValue <= 0f)
        {
            TriggerPression();
        }
    }

    private void HandleUI()
    {
        if (workRadialImage != null)
        {
            float ratio = _workValue / _initialWorkValue;
            // 1f - ratio si tu veux que le rouge soit à 0
            workRadialImage.fillAmount = 1f - ratio;
            workRadialImage.color = Color.Lerp(Color.green, Color.red, workRadialImage.fillAmount);
        }
    }

    private void TriggerPression()
    {
        Debug.Log("GAME OVER");
        enabled = false;
        Time.timeScale = 0f;
    }
}