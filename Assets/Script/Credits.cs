using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{[SerializeField] private AudioSource _audioSource;
    public void QuitCredit(string sceneName)
    {
        StartCoroutine(Delay(sceneName));
    }

    IEnumerator Delay(string sceneName)
    {
        _audioSource.Play();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
        
        
    }
}