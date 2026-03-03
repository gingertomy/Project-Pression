using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ThermometrePression : MonoBehaviour
{
    [Header("Aiguille")]
    public Transform aiguille;
    public float angleMin = -90f;
    public float angleMax = 90f;

    [Header("Pression")]
    [SerializeField, Range(0f, 1f)]
    float pression = 0f;

    [SerializeField]
    float augmentationPassiveParSeconde = 0.02f;

    [Header("Actions externes")]
    [SerializeField] float cranAugmentation = 0.1f;
    [SerializeField] float cranDiminution = 0.15f;

    [Header("Paliers")]
    [SerializeField] float[] seuilsPaliers;
    [Tooltip("Un UnityEvent par palier, dans le même ordre que Seuils Paliers")]
    [SerializeField] UnityEvent[] feedbackParPalier;

    [Header("Game Over")]
    public UnityEvent onGameOver;

    [Header("Vignette")]
    [SerializeField] private Image vignetteImage;
    [SerializeField, Range(0f, 1f)] private float vignetteAlphaMax = 0.75f;
    [SerializeField] private AnimationCurve vignetteCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Camera Shake")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float shakeAmplitudeMax = 0.08f;
    [SerializeField] private float shakeFrequency = 18f;
    [SerializeField] private AnimationCurve shakeCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("DEBUG éditeur")]
    [SerializeField, Range(0f, 1f)] float previewPression = 0f;

    [SerializeField] private Work _workReference;
    [SerializeField] private PeopleHit _peopleHitReference;

    int palierActuel = -1;
    bool gameOverDeclenche;
    private bool isWorking = false;

    private Vector3 cameraInitialLocalPos;
    private float shakeTime;

    void Start()
    {
        if (cameraTransform != null)
            cameraInitialLocalPos = cameraTransform.localPosition;
    }

    private void OnEnable()
    {
        if (_workReference != null)
        {
            _workReference.StartWorking += StartAiguille;
            _workReference.StopWorking += StopAiguille;
            _workReference.BossArrival += AugmenterPression;
        }

        if (_peopleHitReference != null)
            _peopleHitReference.OnPlayerHit += DiminuerPression;
    }

    private void OnDisable()
    {
        if (_workReference != null)
        {
            _workReference.StartWorking -= StartAiguille;
            _workReference.StopWorking -= StopAiguille;
            _workReference.BossArrival -= AugmenterPression;
        }

        if (_peopleHitReference != null)
            _peopleHitReference.OnPlayerHit -= DiminuerPression;
    }

    void Update()
    {
        if (gameOverDeclenche) return;

        if (isWorking)
            ModifierPression(augmentationPassiveParSeconde * Time.deltaTime);

        MettreAJourCameraShake();
    }

    void OnValidate()
    {
        if (aiguille != null)
        {
            float angle = Mathf.Lerp(angleMin, angleMax, previewPression);
            aiguille.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    // =========================
    // APPELS EXTERNES
    // =========================

    public void AugmenterPression()
    {
        ModifierPression(cranAugmentation);
        Debug.Log("PRESSION AUGMENTE");
    }

    public void DiminuerPression()
    {
        ModifierPression(-cranDiminution);
    }

    // =========================
    // LOGIQUE INTERNE
    // =========================

    void ModifierPression(float delta)
    {
        pression = Mathf.Clamp01(pression + delta);

        MettreAJourAiguille();
        MettreAJourVignette();
        VerifierPaliers();

        if (pression >= 1f && !gameOverDeclenche)
        {
            gameOverDeclenche = true;
            onGameOver?.Invoke();
        }
    }

    void MettreAJourAiguille()
    {
        if (aiguille == null) return;

        float angle = Mathf.Lerp(angleMin, angleMax, pression);
        aiguille.localRotation = Quaternion.Euler(0, 0, angle);
    }

    void MettreAJourVignette()
    {
        if (vignetteImage == null) return;

        float intensite = vignetteCurve.Evaluate(pression) * vignetteAlphaMax;

        Color c = vignetteImage.color;
        c.a = intensite;
        vignetteImage.color = c;
    }

    void MettreAJourCameraShake()
    {
        if (cameraTransform == null) return;

        float intensite = shakeCurve.Evaluate(pression) * shakeAmplitudeMax;

        if (intensite <= 0.001f)
        {
            cameraTransform.localPosition = cameraInitialLocalPos;
            return;
        }

        shakeTime += Time.deltaTime * shakeFrequency;

        float offsetX = (Mathf.PerlinNoise(shakeTime, 0f) - 0.5f) * 2f;
        float offsetY = (Mathf.PerlinNoise(0f, shakeTime) - 0.5f) * 2f;

        Vector3 offset = new Vector3(offsetX, offsetY, 0f) * intensite;

        cameraTransform.localPosition = cameraInitialLocalPos + offset;
    }

    void VerifierPaliers()
    {
        if (seuilsPaliers == null) return;

        for (int i = 0; i < seuilsPaliers.Length; i++)
        {
            if (pression >= seuilsPaliers[i] && i > palierActuel)
            {
                palierActuel = i;

                if (feedbackParPalier != null && i < feedbackParPalier.Length)
                    feedbackParPalier[i]?.Invoke();
            }
        }
    }

    private void StartAiguille()
    {
        isWorking = true;
    }

    private void StopAiguille()
    {
        isWorking = false;
    }
}