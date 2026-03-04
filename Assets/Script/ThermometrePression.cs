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
    [SerializeField] float BOSSAugmentation = 0.3f;

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

    

    [Header("DEBUG éditeur")]
    [SerializeField, Range(0f, 1f)] float previewPression = 0f;

    [Header("Références")]
    [SerializeField] private Work _workReference;
    [SerializeField] private PeopleHit[] _peopleHitReference; // Tableau d'ennemis

    [SerializeField] private GameObject GameOverPanel;
    
    private int palierActuel = -1;
    private bool gameOverDeclenche;
    private bool isWorking = false;

    private Vector3 cameraInitialLocalPos;
    private float shakeTime;

    void Start()
    {

        // Initialisation de la vignette à 0
        MettreAJourVignette();
        Time.timeScale = 1f; // S'assure que le temps est à l'échelle normale au début
    }

    private void OnEnable()
    {
        // Abonnement au script Work
        if (_workReference != null)
        {
            _workReference.StartWorking += StartAiguille;
            _workReference.StopWorking += StopAiguille;
            _workReference.BossArrival += AugmenterPressionBOSS;
        }

        // Abonnement à TOUS les PeopleHit du tableau
        if (_peopleHitReference != null)
        {
            foreach (PeopleHit person in _peopleHitReference)
            {
                if (person != null)
                    person.OnPlayerHit += DiminuerPression;
            }
        }
    }

    private void OnDisable()
    {
        if (_workReference != null)
        {
            _workReference.StartWorking -= StartAiguille;
            _workReference.StopWorking -= StopAiguille;
            _workReference.BossArrival -= AugmenterPressionBOSS;
        }

        if (_peopleHitReference != null)
        {
            foreach (PeopleHit person in _peopleHitReference)
            {
                if (person != null)
                    person.OnPlayerHit -= DiminuerPression;
            }
        }
    }

    void Update()
    {
        if (gameOverDeclenche) return;

        // Si on travaille, la pression monte fluidement
        if (isWorking)
            ModifierPression(augmentationPassiveParSeconde * Time.deltaTime);

        // Mise à jour constante des effets visuels
        
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

    public void AugmenterPressionBOSS()
    {
        ModifierPression(BOSSAugmentation);
        Debug.Log("PRESSION AUGMENTE");
    }

    public void DiminuerPression()
    {
        ModifierPression(-cranDiminution);
        Debug.Log("PRESSION DIMINUE");
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

            // Libère le curseur pour que le joueur puisse cliquer les boutons
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (GameOverPanel != null)
            {
                GameOverPanel.SetActive(true);

                Time.timeScale = 0f;

            }

            onGameOver.Invoke();
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

        // Utilise la courbe pour définir l'alpha
        float intensite = vignetteCurve.Evaluate(pression) * vignetteAlphaMax;

        Color c = vignetteImage.color;
        c.a = intensite;
        vignetteImage.color = c;
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

    private void StartAiguille() => isWorking = true;
    private void StopAiguille() => isWorking = false;
}