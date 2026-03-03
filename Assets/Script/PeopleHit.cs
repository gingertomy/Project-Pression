using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class PeopleHit : MonoBehaviour
{
    [SerializeField] DetectionSystem detectionSystem;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] Animator[] _HitConsequence;
    public int NbHit = 0;
    
    public event Action OnPlayerHit;

    private void Start()
    {
        NbHit = 0;
    }

    private void RandomAnimation()
    {
        int index = Random.Range(0, _HitConsequence.Length);
        _HitConsequence[index].SetTrigger("Hit");
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Coke") || collision.gameObject.CompareTag("Paper"))
        {
            Debug.Log("Le joueur a touché l'objet : " + gameObject.name);
            _audioSource.Play();
            OnPlayerHit?.Invoke();
            detectionSystem.Injured();
            NbHit++;
            RandomAnimation();
        }

    }
}
