using UnityEngine;
using UnityEngine.UI;

public class Work : MonoBehaviour
{
    [SerializeField] private InteractionObject _interactionObject; // Référence au script d'interaction

    [SerializeField] private float _workValue = 10f;
    [SerializeField] private float _decaySpeed = 1f;
    [SerializeField] private float _initialWorkValue = 10f;
    [SerializeField] private float _startDelay = 5f;
    private float _currentDelay = 0f;

    public Image workRadialImage;

    private void Start()
    {
        _workValue = _initialWorkValue;
        _currentDelay = _startDelay;

        // Si la référence n'est pas mise dans l'inspecteur, on essaie de la trouver
        if (_interactionObject == null)
            _interactionObject = FindFirstObjectByType<InteractionObject>();
    }

    private void Update()
    {
        // Logique de décrémentation (inchangée)
        if (_currentDelay > 0)
        {
            _currentDelay -= Time.deltaTime;
        }
        else if (_workValue > 0)
        {
            _workValue -= _decaySpeed * Time.deltaTime;
        }

        _workValue = Mathf.Max(_workValue, 0);

        if (workRadialImage != null)
        {
            float ratio = _workValue / _initialWorkValue;
            workRadialImage.fillAmount = 1f - ratio;
            workRadialImage.color = Color.Lerp(Color.green, Color.red, workRadialImage.fillAmount);
        }

        
        if (Input.anyKeyDown)
        {
            
            if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetKeyDown(KeyCode.E) && !Input.GetKeyDown(KeyCode.R))
            { 
                if (_interactionObject != null)
                {
                    bool canWork = !_interactionObject.isObjectHidden && _interactionObject.isHandOccupied == false
                                   || _interactionObject.isObjectHidden;

                    if (canWork)
                    {
                        _workValue = _initialWorkValue;
                        _currentDelay = _startDelay;
                        Debug.Log("Travail en cours...");
                    }
                    else
                    {
                        Debug.Log("Impossible de travailler : vous avez un objet en main !");
                    }
                }
            }
        }

        if (_workValue <= 0)
        {
            Debug.Log("Vous vous êtes endormi au travail...");
        }
    }
}