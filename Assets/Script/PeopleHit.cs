using System;
using UnityEngine;

public class PeopleHit : MonoBehaviour
{
    [SerializeField] DetectionSystem detectionSystem;
    
    public event Action OnPlayerHit;
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Coke") || collision.gameObject.CompareTag("Paper"))
        {
            Debug.Log("Le joueur a touché l'objet : " + gameObject.name);
            OnPlayerHit?.Invoke();
            detectionSystem.Injured();
        }

    }
}
