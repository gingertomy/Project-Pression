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
        public string objectTag;      // Le tag de l'objet (ex: "Burger")
        public GameObject effectPrefab; // L'effet visuel associé
    }

    [SerializeField] DetectionSystem detectionSystem;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] Animator[] _HitConsequence;

    [Header("Configuration des Effets")]
    [SerializeField] private List<HitEffectData> _hitEffects; // Liste à remplir dans l'inspecteur
    [SerializeField] private GameObject _defaultEffect;      // Effet par défaut si le tag n'est pas trouvé
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

        // On cherche si le tag de l'objet qui nous touche est dans notre liste d'effets
        GameObject prefabToSpawn = GetEffectForTag(tag);

        // Si on a trouvé un effet (ou si c'est un tag de projectile valide)
        if (prefabToSpawn != null || tag == "Burger" || tag == "Coffee" || tag == "Starbucks" || tag == "Poubelle")
        {
            Debug.Log("Le personnage " + gameObject.name + " a été touché par : " + tag);

            if (_audioSource != null) _audioSource.Play();

            // Faire apparaître l'effet au point d'impact
            SpawnEffect(collision, prefabToSpawn);

            // Logique de jeu
            OnPlayerHit?.Invoke();
            detectionSystem.Injured();
            NbHit++;
            RandomAnimation();
        }
    }

    private GameObject GetEffectForTag(string tag)
    {
        foreach (var data in _hitEffects)
        {
            if (data.objectTag == tag) return data.effectPrefab;
        }
        return _defaultEffect; // Retourne l'effet par défaut si le tag n'est pas listé
    }

    private void SpawnEffect(Collision collision, GameObject prefab)
    {
        if (prefab == null) return;

        ContactPoint contact = collision.contacts[0];
        Vector3 position = contact.point;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);

        GameObject effectInstance = Instantiate(prefab, position, rotation);
        Destroy(effectInstance, destroyDelay);
    }
}