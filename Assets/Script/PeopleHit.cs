using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class PeopleHit : MonoBehaviour
{
    [Serializable]
    public struct HitEffectData
    {
        public string objectTag;
        public GameObject effectPrefab;
    }

    [SerializeField] private DetectionSystem detectionSystem;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Animator[] _HitConsequence;

    [Header("Configuration des Effets")]
    [SerializeField] private List<HitEffectData> _hitEffects;
    [SerializeField] private GameObject _defaultEffect;
    [SerializeField] private float destroyDelay = 2f;

    [Header("Stats")]
    public int NbHit = 0;
    public event Action OnPlayerHit;

    private void Start()
    {
        NbHit = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 1. On récupère le tag de l'objet qui nous percute
        string incomingTag = collision.gameObject.tag;

        // --- SÉCURITÉ --- 
        // On ne déclenche la logique que si l'objet a un tag d'interaction valide
        // (Évite de détruire le sol ou les murs si le perso les touche)
        if (!IsInteractableTag(incomingTag)) return;

        Debug.Log($"[IMPACT] {gameObject.name} touché par {incomingTag}");

        // 2. Feedback Sonore
        if (_audioSource != null)
            _audioSource.Play();

        // 3. Gestion de l'effet visuel (AVANT la destruction)
        GameObject prefabToSpawn = GetEffectForTag(incomingTag);
        if (prefabToSpawn != null)
        {
            SpawnEffect(collision, prefabToSpawn);
        }

        // 4. Logique de Gameplay
        OnPlayerHit?.Invoke();
        if (detectionSystem != null) detectionSystem.Injured();

        NbHit++;
        RandomAnimation();

        // 5. --- DESTRUCTION DU PROJECTILE ---
        // On détruit l'objet (la canette, le burger, etc.) pour qu'il ne reste pas au sol
        Destroy(collision.gameObject);
    }

    private bool IsInteractableTag(string tag)
    {
        // Vérifie si le tag correspond à un de tes objets définis dans l'Enum
        return Enum.TryParse(tag, out InteractionObject.InteractionType result)
               && result != InteractionObject.InteractionType.None;
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
        // On récupère le point exact où l'objet a touché le corps
        ContactPoint contact = collision.contacts[0];
        Vector3 position = contact.point;

        // On oriente l'effet pour qu'il "jaillisse" vers l'extérieur
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);

        GameObject effectInstance = Instantiate(prefab, position, rotation);
        Destroy(effectInstance, destroyDelay);
    }

    private void RandomAnimation()
    {
        if (_HitConsequence == null || _HitConsequence.Length == 0) return;

        int index = Random.Range(0, _HitConsequence.Length);
        if (_HitConsequence[index] != null)
        {
            _HitConsequence[index].SetTrigger("Hit");
        }
    }
}