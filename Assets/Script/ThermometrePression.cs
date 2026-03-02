using UnityEngine;
using UnityEngine.Events;

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

    [Header("Actions externes (Player plus tard)")]
    [SerializeField] float cranAugmentation = 0.1f;
    [SerializeField] float cranDiminution = 0.15f;

    [Header("Paliers")]
    public int nombrePaliers = 5;
    public UnityEvent<int> onPalierAtteint;

    [Header("Game Over")]
    public UnityEvent onGameOver;

    int palierActuel = -1;
    bool gameOverDeclenche;

    void Start()
    {
        MettreAJourAiguille();
    }

    void Update()
    {
        if (gameOverDeclenche) return;

        ModifierPression(augmentationPassiveParSeconde * Time.deltaTime);
    }

    // =========================
    // APPELS FUTURS DU PLAYER
    // =========================

    public void AugmenterPression()
    {
        ModifierPression(cranAugmentation);
    }

    public void DiminuerPression()
    {
        ModifierPression(-cranDiminution);
    }

    public void AugmenterPression(float valeur)
    {
        ModifierPression(valeur);
    }

    public void DiminuerPression(float valeur)
    {
        ModifierPression(-valeur);
    }

    // =========================
    // LOGIQUE INTERNE
    // =========================

    void ModifierPression(float delta)
    {
        float anciennePression = pression;
        pression = Mathf.Clamp01(pression + delta);

        MettreAJourAiguille();
        VerifierPaliers();

        if (pression >= 1f && !gameOverDeclenche)
        {
            gameOverDeclenche = true;
            onGameOver.Invoke(); // SAFE même sans listener
        }
    }

    void MettreAJourAiguille()
    {
        float angle = Mathf.Lerp(angleMin, angleMax, pression);
        aiguille.localRotation = Quaternion.Euler(0, 0, angle);
    }

    void VerifierPaliers()
    {
        int palier = Mathf.FloorToInt(pression * nombrePaliers);

        if (palier != palierActuel)
        {
            palierActuel = palier;
            onPalierAtteint.Invoke(palier); // SAFE même vide
        }
    }
}