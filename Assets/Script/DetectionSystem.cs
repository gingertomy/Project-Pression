using UnityEngine;
using System.Collections;

public class DetectionSystem : MonoBehaviour
{
    [SerializeField] InteractionObject interactObject;
    [SerializeField] ThermometrePression thermometrePression;
    [SerializeField] PeopleHit _peopleHit;

    [SerializeField] Animator _StarsAnimator;
    [SerializeField] Animator _BustedAnimator;
    [SerializeField] Animator _InterrogationAnimator;
    [SerializeField] Animator _AttentionAnimator;
    [SerializeField] AudioDispatcher _AudioDispatcher;

    public float MinDelay = 5f;
    public float MaxDelay = 15f;
    public float MinVerificationTime = 1f;
    public float MaxVerificationTime = 3f;
    public float StunedTime = 3f;
    public bool IsInjured = false;
    public int NbHitForLevelUp = 5;
    private bool attentionactivated = false;

    // --- AJOUT : Sprites par état ---
    [Header("Sprites États")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite SideEye;
    [SerializeField] private Sprite detectingSprite;
    [SerializeField] private Sprite alertSprite;
    [SerializeField] private Sprite stunnedSprite;

    private void SetSprite(Sprite sprite)
    {
        if (spriteRenderer != null && sprite != null)
            spriteRenderer.sprite = sprite;
    }
    // --- FIN AJOUT ---

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
            float DetectionDelay = Random.Range(MinDelay, MaxDelay + 1);
            Debug.Log(DetectionDelay);
            Debug.Log("Attend");
            yield return new WaitForSeconds(DetectionDelay);
            yield return StartCoroutine(TriggerDetection());
            LevelUpDifficulty();
            _AudioDispatcher.PlayAudio(AudioType.Surpris);
        }
        //SetSprite(SideEye);
    }

    IEnumerator TriggerDetection()
    {
        float verificationTime = Random.Range(MinVerificationTime, MaxVerificationTime + 1);
        float timer = 0f;
        Debug.Log(verificationTime);
        _InterrogationAnimator.SetTrigger("InDetection");
        attentionactivated = true;
        SetSprite(detectingSprite); // AJOUT

        while (timer < verificationTime)
        {
            timer += Time.deltaTime;
            Debug.Log("regarde");
            if (!interactObject.isObjectHidden && interactObject.isHandOccupied)
            {
                if (attentionactivated)
                {
                    thermometrePression.AugmenterPression();
                    _AttentionAnimator.SetTrigger("Detected");
                    _BustedAnimator.SetTrigger("Detected");
                    _AudioDispatcher.PlayAudio(AudioType.Busted);
                    SetSprite(alertSprite); // AJOUT
                }
                attentionactivated = false;
            }
            yield return null;
        }
        
        SetSprite(idleSprite); // AJOUT
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
        _StarsAnimator.SetTrigger("IsStunned");
        SetSprite(stunnedSprite); // AJOUT
        StartCoroutine(StunnedEmployee());
    }

    IEnumerator StunnedEmployee()
    {
        yield return new WaitForSeconds(StunedTime);
        StartDetection();
        Debug.Log("STUN");
        IsInjured = false;
        _StarsAnimator.SetTrigger("EndStunned");
        SetSprite(idleSprite); // AJOUT
    }
}