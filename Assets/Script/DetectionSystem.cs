using UnityEngine;
using System.Collections;

public class DetectionSystem : MonoBehaviour
{
    
    [SerializeField] InteractionObject interactObject;
    [SerializeField] ThermometrePression thermometrePression;
    [SerializeField] PeopleHit _peopleHit;
    //temps entre les verif
    public float MinDelay = 5f;
    public float MaxDelay = 15f;
    
    //temps pendant les vérifs
    public float MinVerificationTime = 1f;
    public float MaxVerificationTime = 3f;
    
    //temps stuned
    public float StunedTime = 3f;
    public bool IsInjured = false;
    
    public int NbHitForLevelUp = 5;
    
    
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
        GetComponent<MeshRenderer>().material.color = Color.red;
        while (timer < verificationTime)
        {
            timer += Time.deltaTime;
            Debug.Log("regarde");
            if (!interactObject.isObjectHidden && interactObject.isHandOccupied)
            {
                thermometrePression.AugmenterPression();
            }
            yield return null;
        }
        GetComponent<MeshRenderer>().material.color = Color.white;
        Debug.Log("fin de verification");
    }

    private void LevelUpDifficulty()
    {
        if (_peopleHit.NbHit > NbHitForLevelUp)
        {
            MaxDelay -= 1;
            MaxVerificationTime += 1;
        }
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
        StopDetection();
        Debug.Log("STOP");
        GetComponent<MeshRenderer>().material.color = Color.green;
        StartCoroutine(StunnedEmployee());
    }

    IEnumerator StunnedEmployee()
    {
        yield return new WaitForSeconds(StunedTime);
        StartDetection();
        Debug.Log("STUN");
        IsInjured = false;
        GetComponent<MeshRenderer>().material.color = Color.white;
    }
}
