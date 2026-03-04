using System.Collections;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    [SerializeField] private InteractionObject _interactObject;
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float upwardForce = 2f;
    [SerializeField] private AudioSource _audioSource;

    private GameObject objectToThrow;

    private void OnEnable()
    {
        // On s'abonne à l'événement générique de ramassage
        if (_interactObject != null)
            _interactObject.OnObjectPicked += HandleObjectPicked;
    }

    private void OnDisable()
    {
        if (_interactObject != null)
            _interactObject.OnObjectPicked -= HandleObjectPicked;
    }

    private void HandleObjectPicked(GameObject pickedObject)
    {
        // L'objet ramassé devient le projectile prêt à être lancé
        objectToThrow = pickedObject;
    }

    private void Update()
    {
        // On ne peut lancer que si l'objet n'est pas "rangé" (donc actif)
        if (Input.GetKeyDown(KeyCode.Mouse0) && objectToThrow != null && objectToThrow.activeSelf)
        {
            Throw();
        }
    }

    private void Throw()
    {
        if (objectToThrow == null) return;

        // --- ÉTAPE CRUCIALE ---
        // On crée une copie locale de la référence avant de mettre la variable globale à null
        GameObject thrownInstance = objectToThrow;

        // On détache l'objet de la main
        thrownInstance.transform.SetParent(null);

        // Application de la physique
        if (thrownInstance.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            _audioSource.Play();
            rb.isKinematic = false;

            // On reset les forces actuelles pour un lancer propre
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Calcul de la direction (face à la caméra)
            Vector3 forceDirection = _interactObject.playerCamera.transform.forward;
            rb.AddForce(forceDirection * throwForce + Vector3.up * upwardForce, ForceMode.Impulse);

            // Ajoute une petite rotation aléatoire pour le réalisme
            rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
        }

        // On réactive le collider pour qu'il touche les PeopleHit
        if (thrownInstance.TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = true;
        }

        // On lance la destruction automatique sur l'instance qu'on vient de lancer


        // On libère la logique de la main
        _interactObject.SetHandFree();

        // On vide la variable pour le prochain ramassage
        objectToThrow = null;
    }

    // On passe l'objet en paramètre pour être sûr de détruire le bon !
    
}