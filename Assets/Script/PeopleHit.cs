using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class PeopleHit : MonoBehaviour
{
    [Serializable]
    public struct HitEffectData
    {
        public string objectTag;
        public GameObject effectPrefab;
    }

    [SerializeField] DetectionSystem detectionSystem;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] Animator[] _HitConsequence;

    [Header("Configuration des Effets")]
    [SerializeField] private List<HitEffectData> _hitEffects;
    [SerializeField] private GameObject _defaultEffect;
    [SerializeField] private float destroyDelay = 2f;

    public int NbHit = 0;
    public event Action OnPlayerHit;

    private void Start()
    {
        NbHit = 0;
    }

    private void RandomAnimation()
    {
        if (_HitConsequence.Length == 0) return;

        int index = Random.Range(0, _HitConsequence.Length);
        _HitConsequence[index].SetTrigger("Hit");
    }

    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;

        Debug.Log("Le personnage " + gameObject.name + " a été touché par : " + tag);

        if (_audioSource != null)
            _audioSource.Play();

        // On cherche un effet associé (peut être null)
        GameObject prefabToSpawn = GetEffectForTag(tag);

        // Spawn uniquement si un prefab existe
        if (prefabToSpawn != null)
            SpawnEffect(collision, prefabToSpawn);

        // Logique de jeu TOUJOURS exécutée
        OnPlayerHit?.Invoke();
        
        NbHit++;
        RandomAnimation();
    }

    private GameObject GetEffectForTag(string tag)
    {
        foreach (var data in _hitEffects)
        {
            if (data.objectTag == tag)
                return data.effectPrefab;
        }

        return _defaultEffect;
    }

    private void SpawnEffect(Collision collision, GameObject prefab)
    {
        ContactPoint contact = collision.contacts[0];
        Vector3 position = contact.point;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);

        GameObject effectInstance = Instantiate(prefab, position, rotation);
        Destroy(effectInstance, destroyDelay);
    }
}