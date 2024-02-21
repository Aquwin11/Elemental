using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyScript : MonoBehaviour
{
    [Header(" Player HUD ")]
    [SerializeField] Image energyBar;
    [SerializeField] float energyBuffer;
    [SerializeField] float energyCounter;
    [SerializeField] bool energyWait;
    public bool energyReset;
    [SerializeField] float energyFillAmount;
    public Color startColor;

    public void Update()
    {
        EnergyBarProgress();
    }
    private void EnergyBarProgress()
    {
        if (energyCounter > energyBuffer && !energyReset)
        {
            energyWait = false;
            energyBar.fillAmount += 0.32f * Time.deltaTime;
        }

        else if (energyWait && energyCounter < energyBuffer)
        {
            energyCounter += Time.deltaTime;
        }


        if (energyBar.fillAmount <= 0)
        {
            energyReset = true;
        }

        if (energyReset)
        {
            energyBar.fillAmount += energyFillAmount * Time.deltaTime;
            energyBar.color = new Color(startColor.r, startColor.g, startColor.b, 0.5f);
            StartCoroutine(EnergyRefill());
        }
    }

    public void ConsumeEnergy(float energyAmount)
    {
        energyWait = true;
        energyCounter = 0f;
        energyBar.fillAmount -= energyAmount;
    }


    public void ConsumeEnergyOverTime(float energyAmount)
    {
        energyWait = true;
        energyCounter = 0f;
        energyBar.fillAmount -= energyAmount*Time.deltaTime;
    }

    IEnumerator EnergyRefill()
    {
        yield return new WaitUntil(() => energyBar.fillAmount >= 1f);
        energyReset = false;
        energyBar.color = startColor;
    }

    public void EnergyAdd(float energyValue)
    {
        energyBar.fillAmount += energyValue;
        if (energyBar.fillAmount>1)
        {
            energyBar.fillAmount = 1;
        }
    }
}
