using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public void QuitCredit(string sceneName)
    {
        SceneManager.LoadScene("MainMenu");
    }
}