using UnityEngine;
using System.Collections;

public class DetectionSystem : MonoBehaviour
{
    //temps entre les verif
    public float MinDelay = 5f;
    public float MaxDelay = 15f;
    
    //temps pendant les vérifs
    public float MinVerificationTime = 1f;
    public float MaxVerificationTime = 3f;

    public bool IsInjured = false;
    
    // récupérer variable du joueur : si il travaille ou si il fait des betises
    // récupérer variable du joueur : variable qui illustre le nombre de fois qu'il a embêté des gens. Si beaucoup, alors fréquence augmente
    // récupérer la fonction qui augmente la jauge de pression
    
    void Start()
    {
        StartDetection();
    }

    IEnumerator RandomDetection()
    {
        while (!IsInjured)
        {
            float DetectionDelay = Random.Range(MinDelay, MaxDelay+1);
            Debug.Log(DetectionDelay);
            Debug.Log("Attend");
            yield return new WaitForSeconds(DetectionDelay);
            yield return StartCoroutine(TriggerDetection());
            LevelUpDifficulty();
            // l'employer se retourne
        }
    }

    IEnumerator TriggerDetection()
    {
        float verificationTime= Random.Range(MinVerificationTime, MaxVerificationTime + 1);
        float timer = 0f;
        Debug.Log(verificationTime);
        while (timer < verificationTime)
        {
            timer += Time.deltaTime;
            Debug.Log("regarde");
            // if (il fait des bêtises)
            // fonction qui augmente la jauge
            yield return null;
        }
        Debug.Log("fin de verification");
    }

    private void LevelUpDifficulty()
    {
        // if(il a embeter bcp de gens)
        MaxDelay -= 1;
        MaxVerificationTime += 1;
    }

    public void StopDetection()
    {
        StopCoroutine(RandomDetection());
    }

    private void StartDetection()
    {
        StartCoroutine(RandomDetection());
    }

    public void Injured()
    {
        IsInjured = true;
    }
}
