using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Tooltip("Durée en secondes à attendre après le trigger (= durée de ton clip d'animation)")]
    [SerializeField] private float dureeAnimation = 0.5f;

    [Header("Son (optionnel)")]
    [SerializeField] private AudioSource audioSource;

    private static readonly int TriggerClick = Animator.StringToHash("Click");

    private bool transitionEnCours = false;

    /// <summary>Lance l'animation + son puis charge la scène donnée.</summary>
    public void StartGame(string sceneName)
    {
        LancerTransition(sceneName);
    }

    /// <summary>Lance l'animation + son puis charge la scène Credits.</summary>
    public void Credits(string sceneName)
    {
        LancerTransition(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // =========================
    // LOGIQUE INTERNE
    // =========================

    void LancerTransition(string sceneName)
    {
        if (transitionEnCours) return;
        transitionEnCours = true;

        StartCoroutine(TransitionVersScene(sceneName));
    }

    IEnumerator TransitionVersScene(string sceneName)
    {
        if (animator != null)
            animator.SetTrigger(TriggerClick);
        
        if (audioSource != null && audioSource.clip != null)
            audioSource.Play();
        
        float attente = dureeAnimation;

        if (audioSource != null && audioSource.clip != null)
            attente = Mathf.Max(attente, audioSource.clip.length);

        yield return new WaitForSeconds(attente);

        SceneManager.LoadScene(sceneName);
    }
}
