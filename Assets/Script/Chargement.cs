using System.Collections;
using UnityEngine;

using UnityEngine;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float delay = 2f;

    private void Start()
    {
        StartCoroutine(LoadingRoutine());
    }

    private IEnumerator LoadingRoutine()
    {
       
        yield return new WaitForSecondsRealtime(delay);

        animator.SetTrigger("Down"); 


    }
}