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

    private bool _isWorkingAutomatically = false;
    

    public Image workRadialImage;

    
    public event Action StartWorking;
    public event Action StopWorking;
    public event Action BossArrival;

    private void Start()
    {
        _workValue = _initialWorkValue;

        if (_interactionObject == null)
            _interactionObject = FindFirstObjectByType<InteractionObject>();
    }

    private void Update()
    {
        

        HandleAutomaticWork();
        HandleUI();
    }

    private void HandleAutomaticWork()
    {
        bool canWork = (_interactionObject != null) &&
                       (!_interactionObject.isHandOccupied || _interactionObject.isObjectHidden);

        if (canWork)
        {
            if (!_isWorkingAutomatically)
            {
                _isWorkingAutomatically = true;
                StartWorking?.Invoke();
                Debug.Log("Travail ON : L'aiguille monte.");
            }

           
            _workValue = Mathf.MoveTowards(_workValue, _initialWorkValue, _autoWorkSpeed * Time.deltaTime);
        }
        else
        {
            if (_isWorkingAutomatically)
            {
                _isWorkingAutomatically = false;
                StopWorking?.Invoke();
                Debug.Log("Travail OFF : L'aiguille s'arrête.");
            }

            
            _workValue -= _decaySpeed * Time.deltaTime;
        }

        _workValue = Mathf.Max(_workValue, 0);

        
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
            workRadialImage.fillAmount = 1f - ratio;
            workRadialImage.color = Color.Lerp(Color.green, Color.red, workRadialImage.fillAmount);
        }
    }

    private void TriggerPression()
    {
        

        Debug.Log("GAME OVER");

        
        enabled = false;

        
        Time.timeScale = 0f;

       
        // - Activer un panel UI GameOver
       
    }
}