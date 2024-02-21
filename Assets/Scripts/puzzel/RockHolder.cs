using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockHolder : MonoBehaviour
{
    public bool rockInPlace;
    public RockPuzzle rockPuzzle;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Puzzle" && !rockInPlace)
        {
            GameObject bolder = other.gameObject;
            bolder.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(true);
            rockInPlace = true;
            rockPuzzle.CheckIfPuzzleComplete();
        }
    }
}
