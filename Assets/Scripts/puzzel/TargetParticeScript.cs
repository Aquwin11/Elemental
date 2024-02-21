using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetParticeScript : MonoBehaviour
{
    public bool isTargetable;
    public TargetPuzzel targetTwo;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Bullet" && isTargetable)
        {
            targetTwo.score += 1;
            targetTwo.challengeTimer = 0;
            targetTwo.chooseRandomTarget();
            Debug.Log("Test Puzzle Two complete");
        }
    }
}
