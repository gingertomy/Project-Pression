using System.Collections;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    [SerializeField] private InteractionObject _interactObject; 
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float upwardForce = 2f;
    [SerializeField] private AudioDispatcher _audioDispatcher;

    private GameObject objectToThrow;

    private void OnEnable()
    {
       
        _interactObject.OnCokePicked += HandleObjectPicked;
        _interactObject.OnPaperPicked += HandleObjectPicked;
    }

    private void OnDisable()
    {
       
        _interactObject.OnCokePicked -= HandleObjectPicked;
        _interactObject.OnPaperPicked -= HandleObjectPicked;
    }

    private void HandleObjectPicked(GameObject pickedObject)
    {
        objectToThrow = pickedObject;
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Mouse0) && objectToThrow != null)
        {
            Throw();
        }
    }

    private void Throw()
    {
        
        if (objectToThrow == null || !objectToThrow.activeSelf) return;


        objectToThrow.transform.SetParent(null);

        
        if (objectToThrow.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            _audioDispatcher.PlayAudio(AudioType.Throw);
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero; 

            
            Vector3 forceDirection = _interactObject.playerCamera.transform.forward;
            rb.AddForce(forceDirection * throwForce + Vector3.up * upwardForce, ForceMode.Impulse);
        }

       
        if (objectToThrow.TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = true;
        }

        StartCoroutine(DestroyObject());

        _interactObject.SetHandFree();

        
        objectToThrow = null;
    }


    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(5f);
        if (objectToThrow != null) Destroy(objectToThrow);
    }
}