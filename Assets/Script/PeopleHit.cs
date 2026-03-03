using UnityEngine;

public class PeopleHit : MonoBehaviour
{
    [SerializeField] DetectionSystem detectionSystem;
    
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Coke") || collision.gameObject.CompareTag("Paper"))
        {
            Debug.Log("Le joueur a touché l'objet : " + gameObject.name);
            detectionSystem.Injured();
        }

    }
}
