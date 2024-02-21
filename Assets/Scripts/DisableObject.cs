using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObject : MonoBehaviour
{
    public float disableDuration;
    public void OnEnable()
    {
        StopCoroutine(DisableObj());
        StartCoroutine(DisableObj());
    }

    IEnumerator DisableObj()
    {
        yield return new WaitForSeconds(disableDuration);
        gameObject.SetActive(false);
    }

}
