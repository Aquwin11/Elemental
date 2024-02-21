using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyReset : MonoBehaviour
{
    public EnergyScript energyScript;
    public float energyAmount;

    public void resetEnergy()
    {
        if(energyScript==null)
        {
            energyScript = GameObject.FindGameObjectWithTag("Player").GetComponent<EnergyScript>();
        }
        if(energyScript!=null)
        {
            energyScript.EnergyAdd(energyAmount);
        }
    }

}
