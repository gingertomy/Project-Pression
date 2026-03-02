using UnityEngine;
using UnityEngine.Events;

public class HorlogeJournee : MonoBehaviour
{
    [Header("Aiguille")]
    public Transform aiguilleHeure;
    public float angleMin = 0f;
    public float angleMax = -360f;

    [Header("Temps")]
    [SerializeField] int heuresTotales = 24;
    [SerializeField] float dureeParHeure = 30f;

    [Header("Events")]
    public UnityEvent onJourneeTerminee;

    float tempsEcoule = 0f;
    bool tempsArrete = false;

    void Start()
    {
        MettreAJourAiguille();
    }

    void Update()
    {
        if (tempsArrete) return;

        tempsEcoule += Time.deltaTime;

        float progression = Mathf.Clamp01(
            tempsEcoule / (heuresTotales * dureeParHeure)
        );

        float angle = Mathf.Lerp(angleMin, angleMax, progression);
        aiguilleHeure.localRotation = Quaternion.Euler(0, 0, angle);

        if (tempsEcoule >= heuresTotales * dureeParHeure)
        {
            tempsArrete = true;
            onJourneeTerminee.Invoke();
        }
    }

    // =========================
    // GAME OVER
    // =========================

    public void ArreterTemps()
    {
        tempsArrete = true;
    }

    // =========================
    // SCORE
    // =========================

    public int GetHeureTenue()
    {
        return Mathf.FloorToInt(tempsEcoule / dureeParHeure);
    }

    void MettreAJourAiguille()
    {
        aiguilleHeure.localRotation = Quaternion.Euler(0, 0, angleMin);
    }
}