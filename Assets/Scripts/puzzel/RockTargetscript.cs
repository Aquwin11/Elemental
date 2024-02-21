using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockTargetscript : MonoBehaviour
{
    [SerializeField]private GameObject Rock;
    [SerializeField]private Transform initializePosition;

    public void ResetRocksPosition()
    {
        Rock.transform.position = initializePosition.position;
        if (!Rock.activeSelf)
            Rock.SetActive(true);
    }
}
