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

    [Header("Actions externes")]
    [SerializeField] float cranAugmentation = 0.1f;
    [SerializeField] float cranDiminution = 0.15f;

    [Header("Paliers")]
    [SerializeField] float[] seuilsPaliers;

    [Tooltip("Un UnityEvent par palier, dans le même ordre que Seuils Paliers")]
    [SerializeField] UnityEvent[] feedbackParPalier;

    [Header("Game Over")]
    public UnityEvent onGameOver;

    [Header("DEBUG éditeur")]
    [SerializeField, Range(0f, 1f)] float previewPression = 0f;

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

    void OnValidate()
    {
        if (aiguille == null) return;

        float angle = Mathf.Lerp(angleMin, angleMax, previewPression);
        aiguille.localRotation = Quaternion.Euler(0f, 0f, angle);

        if (seuilsPaliers != null)
        {
            for (int i = 0; i < seuilsPaliers.Length; i++)
            {
                float anglePalier = Mathf.Lerp(angleMin, angleMax, seuilsPaliers[i]);
                Debug.Log($"[Preview] Palier {i} | Seuil : {seuilsPaliers[i]:F2} | Angle : {anglePalier:F1}°");
            }
        }
    }

    // =========================
    // APPELS FUTURS DU PLAYER
    // =========================

    public void AugmenterPression() => ModifierPression(cranAugmentation);
    public void DiminuerPression() => ModifierPression(-cranDiminution);
    public void AugmenterPression(float valeur) => ModifierPression(valeur);
    public void DiminuerPression(float valeur) => ModifierPression(-valeur);

    // =========================
    // LOGIQUE INTERNE
    // =========================

    void ModifierPression(float delta)
    {
        pression = Mathf.Clamp01(pression + delta);
        MettreAJourAiguille();
        VerifierPaliers();

        if (pression >= 1f && !gameOverDeclenche)
        {
            gameOverDeclenche = true;
            onGameOver.Invoke();
        }
    }

    void MettreAJourAiguille()
    {
        float angle = Mathf.Lerp(angleMin, angleMax, pression);
        aiguille.localRotation = Quaternion.Euler(0, 0, angle);
    }

    void VerifierPaliers()
    {
        for (int i = 0; i < seuilsPaliers.Length; i++)
        {
            if (pression >= seuilsPaliers[i] && i > palierActuel)
            {
                palierActuel = i;

                float angleActuel = Mathf.Lerp(angleMin, angleMax, pression);
                Debug.Log($"Palier atteint : {palierActuel} | Pression : {pression:F2} | Angle : {angleActuel:F1}°");

                if (feedbackParPalier != null && i < feedbackParPalier.Length)
                    feedbackParPalier[i].Invoke();
            }
        }
    }
    void OnGUI()
    {
        float angleActuel = Mathf.Lerp(angleMin, angleMax, pression);

        GUIStyle style = new GUIStyle(GUI.skin.box) { fontSize = 18 };
        style.normal.textColor = Color.white;

        string texte = $"Pression : {pression:F2}\n" +
                       $"Angle : {angleActuel:F1}°\n" +
                       $"Palier actuel : {palierActuel}";

        GUI.Box(new Rect(10, 10, 250, 80), texte, style);
    }
}
