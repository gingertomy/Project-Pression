using UnityEngine;
using UnityEngine.Events;

public class Horloge3DCrans : MonoBehaviour
{
    [Header("Aiguille")]
    public Transform aiguille;

    [Header("Rotation / vitesse")]
    public float vitesse = 30f; 
    public float angleDepart = 0f;

    [Header("Crans")]
    public float[] anglesCrans; 

    [Header("Feedback")]
    public UnityEvent<int> onCranAtteint;
   
    [Header("Victoire")]
    public float angleVictoire; // angle à atteindre pour victoire
    public UnityEvent onVictoire;
    bool victoireAtteinte = false;
    float angleActuel;
    int indexCranAtteint = -1;

    [SerializeField] private GameObject VictoryPanel;
    void Start()
    {
        angleActuel = angleDepart;
        
        if (anglesCrans.Length == 0)
        {
            int nombreCrans = 7; 
            anglesCrans = new float[nombreCrans];
            float angleParCran = 360f / nombreCrans;
            for (int i = 0; i < nombreCrans; i++)
                anglesCrans[i] = angleDepart + i * angleParCran;
        }
    }

    void Update()
    {
        angleActuel += vitesse * Time.deltaTime;
        angleActuel %= 360f;
        aiguille.localRotation = Quaternion.Euler(0, angleActuel, 0);

        
        for (int i = 0; i < anglesCrans.Length; i++)
        {
            // si on n'a pas encore déclenché ce cran
            if (indexCranAtteint < i && angleActuel >= anglesCrans[i])
            {
                indexCranAtteint = i;
                Debug.Log("Cran atteint : " + i + " | Angle : " + angleActuel.ToString("F1"));
                onCranAtteint.Invoke(i);

                // vérifier la victoire séparément
                if (!victoireAtteinte && angleActuel >= angleVictoire)
                {
                    victoireAtteinte = true;
                    if (VictoryPanel != null)
                        VictoryPanel.SetActive(true);
                }
            }
        }
    }
}