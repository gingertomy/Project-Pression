using UnityEngine;

public class PeopleHit : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Coke") || collision.gameObject.CompareTag("Paper"))
        {
            Debug.Log("Le joueur a touché l'objet : " + gameObject.name);

        }

    }
}
