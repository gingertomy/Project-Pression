using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Nécessaire pour les Coroutines

public class Work : MonoBehaviour
{
    [SerializeField] private InteractionObject _interactionObject;

    [Header("Work Stats")]
    [SerializeField] private float _workValue = 10f;
    [SerializeField] private float _decaySpeed = 1f;
    [SerializeField] private float _initialWorkValue = 10f;
    [SerializeField] private float _autoWorkSpeed = 2f;
    [SerializeField] private float _startDelay = 5f; // Délai de 3 secondes
    [SerializeField] private MoveCamera _moveCamera;
    [SerializeField] private GameObject _textWork;
    [SerializeField] private AudioSource _audioSource;

    private bool _isWorkingAutomatically = false;
    private bool _isCameraHigh = false;
    private bool _isWaitingToWork = false; // Pour savoir si on est dans le délai des 3s
    private Coroutine _startWorkCoroutine;

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
        if (_moveCamera != null)
        {
            _moveCamera.OnCameraHigh += SetCameraHigh;
            _moveCamera.OnCameraLow += SetCameraLow;
        }
    }

    private void OnDisable()
    {
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
        bool canWork = (_interactionObject != null) &&
                       (!_interactionObject.isHandOccupied || _interactionObject.isObjectHidden) &&
                       !_isCameraHigh;

        if (canWork)
        {
            if (!_isWorkingAutomatically && !_isWaitingToWork)
            {
                // On commence à attendre 3 secondes avant de travailler
                _startWorkCoroutine = StartCoroutine(StartWorkAfterDelay());
            }

            if (_isWorkingAutomatically)
            {
                // L'aiguille ne remonte QUE si on a fini d'attendre
                _workValue = Mathf.MoveTowards(_workValue, _initialWorkValue, _autoWorkSpeed * Time.deltaTime);
            }
        }
        else
        {
            // Si on ne peut plus travailler, on annule l'attente ou on arrête le travail
            if (_isWaitingToWork)
            {
                StopCoroutine(_startWorkCoroutine);
                _isWaitingToWork = false;
            }

            if (_isWorkingAutomatically)
            {
                _isWorkingAutomatically = false;
                _audioSource.Pause();
                StopWorking?.Invoke();
                _textWork.SetActive(false);
                Debug.Log("Travail OFF");
            }

            // La barre descend TOUJOURS si on ne travaille pas activement
            _workValue -= _decaySpeed * Time.deltaTime;
        }

        _workValue = Mathf.Max(_workValue, 0);

        if (_workValue <= 0f)
        {
            TriggerPression();
        }
    }

    private IEnumerator StartWorkAfterDelay()
    {
        _isWaitingToWork = true;
        Debug.Log("Préparation au travail... attente de " + _startDelay + "s");

        yield return new WaitForSeconds(_startDelay);

        _isWorkingAutomatically = true;
        _isWaitingToWork = false;

        _audioSource.Play();
        StartWorking?.Invoke();
        _textWork.SetActive(true);
        Debug.Log("Travail ON : L'aiguille remonte enfin.");
    }

    private void HandleUI()
    {
        if (workRadialImage != null)
        {
            float ratio = _workValue / _initialWorkValue;
            workRadialImage.fillAmount = 1f - ratio;
            workRadialImage.color = Color.Lerp(Color.green, Color.red, workRadialImage.fillAmount);
        }
    }

    private void TriggerPression()
    {
        _workValue = 10f;
        BossArrival?.Invoke();
    }
}