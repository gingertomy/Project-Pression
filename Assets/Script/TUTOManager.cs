using System.Collections;
using UnityEngine;

public class TUTOManager : MonoBehaviour
{
    [SerializeField] private GameObject _clickE;
    [SerializeField] private GameObject _clickR;
    [SerializeField] private GameObject _clickGauche;
    [SerializeField] private GameObject _clickSpace;
    

    [SerializeField] InteractionObject _interaction;
    [SerializeField] Work _work;

    private float delay = 8f;


    private void Start()
    {
        _clickE.SetActive(false);
        _clickR.SetActive(false);
        _clickGauche.SetActive(false);
        _clickSpace.SetActive(false);
        
        
    }
    private void OnEnable()
    {
        _interaction.Interact += ShowClickE;
        _interaction.Pressed += ShowClickR;
        _interaction.Pressed += ShowClickGauche;
        _interaction.Pressed += ShowClickSpace;
        

    }

    private void OnDisable()
    {
        _interaction.Interact -= ShowClickE;
        _interaction.Pressed -= ShowClickR;
        _interaction.Pressed -= ShowClickGauche;
        _interaction.Pressed -= ShowClickSpace;
        
    }

    private void ShowClickE()
    {
        _clickE.SetActive(true);
        StartCoroutine(ShowEDelay());
    }


    IEnumerator ShowEDelay()
    {
        yield return new WaitForSeconds(delay);
        _clickE.SetActive(false);
    }

    private void ShowClickR()
    {
        _clickR.SetActive(true);
        StartCoroutine(ShowRDelay());
    }

    IEnumerator ShowRDelay()
    {
        yield return new WaitForSeconds(delay);
        _clickR.SetActive(false);
    }
     private void ShowClickGauche()
    {
        _clickGauche.SetActive(true);
        StartCoroutine(ShowGaucheDelay());
    }

    IEnumerator ShowGaucheDelay()
    {
        yield return new WaitForSeconds(delay);
        _clickGauche.SetActive(false);
    }
     private void ShowClickSpace()
    {
        _clickSpace.SetActive(true);
        StartCoroutine(ShowSpaceDelay());
    }

    IEnumerator ShowSpaceDelay()
    {
        yield return new WaitForSeconds(delay);
        _clickSpace.SetActive(false);
    }

}
