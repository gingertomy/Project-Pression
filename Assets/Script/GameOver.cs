using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameOver : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Tooltip("Durée en secondes à attendre après le trigger (= durée de ton clip d'animation)")]
    [SerializeField] private float dureeAnimation = 0.5f;

    [Header("Son (optionnel)")]
    [SerializeField] private AudioSource audioSource;

    private static readonly int TriggerClickGameO = Animator.StringToHash("ClickGameO");

    private bool transitionEnCours = false;
    
    public void Rejouer (string sceneName)
    {
        LancerTransition(sceneName);
    }

    public void Quitter(string sceneName)
    {
        LancerTransition(sceneName);
    }
    
    void LancerTransition(string sceneName)
    {
        if (transitionEnCours) return;
        transitionEnCours = true;

        StartCoroutine(TransitionVersScene(sceneName));
    }

    IEnumerator TransitionVersScene(string sceneName)
    {
        if (animator != null)
            animator.SetTrigger(TriggerClickGameO);
        
        if (audioSource != null && audioSource.clip != null)
            audioSource.Play();
        
        float attente = dureeAnimation;

        if (audioSource != null && audioSource.clip != null)
            attente = Mathf.Max(attente, audioSource.clip.length);

        yield return new WaitForSeconds(attente);

        SceneManager.LoadScene(sceneName);
    }
}

