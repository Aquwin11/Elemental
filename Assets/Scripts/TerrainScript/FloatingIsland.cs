using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class FloatingIsland : MonoBehaviour
{
    public float islandCOunter;
    public float islandBuffer;
    [SerializeField] Vector3 dir;
    public float movingSpeed;
    public LayerMask exceptPlayer;


    private void Update()
    {
        if (GameManager.gameManagerInstance != null && GameManager.gameManagerInstance.gamePause)
            return;
        IslandCounter();
        //transform.DOLocalMove(transform.forward, movingSpeed);
        transform.position += dir * movingSpeed * Time.deltaTime;
        
    }

    private void IslandCounter()
    {
        if(islandCOunter<islandBuffer)
        {
            islandCOunter += Time.deltaTime;
        }
        else
        {
            ChangeDirection();
        }
    }

    
    public void ChangeDirection()
    {
        islandCOunter = 0; 
        dir = dir * -1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Player")
        {
            other.transform.parent = gameObject.transform;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.transform.parent = null;
        }
    }
}
