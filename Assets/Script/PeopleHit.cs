using System;
using UnityEngine;

public class PeopleHit : MonoBehaviour
{
    [SerializeField] DetectionSystem detectionSystem;
    public int NbHit = 0;
    
    public event Action OnPlayerHit;

    private void Start()
    {
        NbHit = 0;
    }
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Coke") || collision.gameObject.CompareTag("Paper"))
        {
            Debug.Log("Le joueur a touché l'objet : " + gameObject.name);
            OnPlayerHit?.Invoke();
            detectionSystem.Injured();
            NbHit++;
        }

    }
}
