using UnityEngine;
using System.Collections;

public class DetectionSystem : MonoBehaviour
{
    
    [SerializeField] InteractionObject interactObject;
    [SerializeField] ThermometrePression thermometrePression;
    [SerializeField] PeopleHit _peopleHit;

    [SerializeField] Animator _StarsAnimator;
    [SerializeField] Animator _InterrogationAnimator;
    [SerializeField] Animator _AttentionAnimator;
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
    private bool attentionactivated = false;
    
    [SerializeField] private AudioSource audioSurpris;
    [SerializeField] private AudioSource audioAttention;
    
    // récupérer variable du joueur : si il travaille ou si il fait des betises
    // récupérer variable du joueur : variable qui illustre le nombre de fois qu'il a embêté des gens. Si beaucoup, alors fréquence augmente
    // récupérer la fonction qui augmente la jauge de pression
    
    void Start()
    {
        StartDetection();
    }

    private void OnEnable()
    {
        _peopleHit.OnPlayerHit += Injured;
    }
    private void OnDisable()
    {
        _peopleHit.OnPlayerHit -= Injured;
    }
    void Update()
    {
        //LevelUpDifficulty();
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
            if (audioSurpris != null)
                audioSurpris.Play();
        }
    }

    IEnumerator TriggerDetection()
    {
        float verificationTime= Random.Range(MinVerificationTime, MaxVerificationTime + 1);
        float timer = 0f;
        Debug.Log(verificationTime);
        GetComponent<MeshRenderer>().material.color = Color.red;
        _InterrogationAnimator.SetTrigger("InDetection");
        attentionactivated = true;
        while (timer < verificationTime)
        {
            timer += Time.deltaTime;
            Debug.Log("regarde");
            if (!interactObject.isObjectHidden && interactObject.isHandOccupied)
            {  
                if (attentionactivated)
                {
                    if (audioAttention != null)
                        audioAttention.Play();
                    thermometrePression.AugmenterPression();
                    _AttentionAnimator.SetTrigger("Detected");
                }
                attentionactivated = false;
                // je sais pas si ca va marcher
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
            Debug.Log("LEVELUP");
        }
        else
        {
            return;
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
        _StarsAnimator.SetTrigger("IsStunned");
        StartCoroutine(StunnedEmployee());
    }

    IEnumerator StunnedEmployee()
    {
        yield return new WaitForSeconds(StunedTime);
        StartDetection();
        Debug.Log("STUN");
        IsInjured = false;
        GetComponent<MeshRenderer>().material.color = Color.white;
        _StarsAnimator.SetTrigger("EndStunned");
    }
}
