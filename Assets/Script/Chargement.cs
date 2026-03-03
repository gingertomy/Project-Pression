using System.Collections;
using UnityEngine;

using UnityEngine;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private float delay = 2f;

    private void Start()
    {
        StartCoroutine(LoadingRoutine());
    }

    private IEnumerator LoadingRoutine()
    {
        // Active le loading
        loadingScreen.SetActive(true);

        // Pause le jeu
       

        // Attend en temps réel (important si timeScale = 0)
        yield return new WaitForSecondsRealtime(delay);

        // Désactive le loading
        loadingScreen.SetActive(false);

        // Reprend le jeu
        
    }
}