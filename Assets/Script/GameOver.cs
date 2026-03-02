using UnityEngine;

public class GameOver : MonoBehaviour
{
    
    public HorlogeJournee horloge;

    public void OnGameOver()
    {
        horloge.ArreterTemps();

        int heureTenue = horloge.GetHeureTenue();
        Debug.Log("Heures tenues : " + heureTenue);

        // Afficher UI, bloquer le jeu, etc.
    }
}
