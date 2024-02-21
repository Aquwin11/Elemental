using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNight : MonoBehaviour
{
    [Header("TIme")]
    [SerializeField]
    private float _targetDayLength = 0.5f;//length of day per minutes
    public float targetDayLength
    {
        get
        {
            return _targetDayLength;
        }
    }

    [SerializeField]
    [Range(0,1f)]
    private float _timeOfDay;
    public float timeOfDay
    {
        get
        {
            return _timeOfDay;
        }
    }
    [SerializeField]
    private int _dayNumber=0;
    public int dayNumber
    {
        get
        {
            return _dayNumber;
        }
    }
    [SerializeField]
    private int _yearNumber = 0;
    public int yearNumber
    {
        get
        {
            return _yearNumber;
        }
    }
    private float _timeScale = 100f;
    [SerializeField]
    private int _yearLength=100;
    public int yearLength
    {
        get
        {
            return _yearLength;
        }
    }
    public bool pause = false;
    public Transform dailyRoattion;
    public Light sun;
    public float intensity;
    [SerializeField]
    private float sunBaseIntensity=1f;
    [SerializeField]
    private float sunVariation = 1.5f;
    public Gradient sunColor;

    [Header("Season Variable")]
    [SerializeField]
    private Transform sunSeasonalRotation;
    [SerializeField]
    [Range(-45f, 45f)]
    private float maxSeasonalTilts;


    [Header("Time Curve")]
    public AnimationCurve timeCurve;
    public float timeCurveNormalization;


    public bool isDay;

    [Header("NightColor")]
    public Color nightTopColor;
    public Color nightBottomColor;

    [Header("DayColor")]
    public Color dayTopColor;
    public Color dayBottomColor;

    [Header("SkyBox")]
    public Material skyBoxMat;
    public float colorLerpingTime;
    public float maxStars;
    public float minStars;

    public float minMoonMask;
    public float maxMoonMask;
    public float moonMaskTimeLerp;
    public void Update()
    {
        if (GameManager.gameManagerInstance != null && GameManager.gameManagerInstance.gamePause)
            return;
        if (!pause)
        {
            UpdateTimeScale();
            UpdateTime();
        }
        AdjustSunRotation();
        SunIntensity();
        SunColor();
        checkifDayorNight();
    }

    private void Start()
    {
        NormalizeTimeCurve();
    }

    private void UpdateTimeScale()
    {
        _timeScale = 24 / (_targetDayLength/60);
        _timeScale *= timeCurve.Evaluate(timeOfDay);
        _timeScale /= timeCurveNormalization;
    }

    private void UpdateTime()
    {
        _timeOfDay += Time.deltaTime * _timeScale / 86400;
        if(_timeOfDay>1)
        {
            _dayNumber++;
            _timeOfDay -= 1;
            if(_dayNumber > _yearNumber)
            {
                _yearNumber++;
                _dayNumber = 0;
            }

        }
    }

    private void AdjustSunRotation()
    {
        float sunAngle = timeOfDay * 360f;
        dailyRoattion.transform.localRotation = Quaternion.Euler(new Vector3(sunAngle, 0f, 0f));

        float seasonalAngle = -maxSeasonalTilts * Mathf.Cos(dayNumber / _yearLength * 2f * Mathf.PI);
        sunSeasonalRotation.localRotation = Quaternion.Euler(seasonalAngle, 0f, 0f);
    }
    public void SunIntensity()
    {
        intensity = Vector3.Dot(sun.transform.forward, Vector3.down);
        intensity = Mathf.Clamp01(intensity);
        sun.intensity = intensity * sunVariation + sunBaseIntensity;
    }
    public void SunColor()
    {
        sun.color = sunColor.Evaluate(intensity);
    }

    public void NormalizeTimeCurve()
    {
        float stepSize = 0.01f;
        int numberSteps = Mathf.FloorToInt(1f/ stepSize);
        float curveTotal = 0f;  
        for(int i=0;i<numberSteps;i++)
        {
            curveTotal += timeCurve.Evaluate(i * stepSize);
        }
        timeCurveNormalization = curveTotal / numberSteps;
    }
    public void checkifDayorNight()
    {
        if(timeOfDay>0.1f && timeOfDay<0.5)
        {
            isDay = true;
            DayChanges();
        }
        else
        {
            isDay = false;
            NightChanges();
        }
    }


    private void DayChanges()
    {

        //SkyChange
        Color topCurrentColor = skyBoxMat.GetColor("_TopColor");
        topCurrentColor = Color.Lerp(topCurrentColor, dayTopColor, colorLerpingTime*Time.deltaTime);
        skyBoxMat.SetColor("_TopColor", topCurrentColor);
        Color BottomCurrentColor = skyBoxMat.GetColor("_BottomColor");
        BottomCurrentColor = Color.Lerp(BottomCurrentColor, dayBottomColor, colorLerpingTime * Time.deltaTime);
        skyBoxMat.SetColor("_BottomColor", BottomCurrentColor);
        //StarChange
        if(skyBoxMat.GetFloat("_stars") >minStars)
        {
            float currentStartLevel = skyBoxMat.GetFloat("_stars");
            currentStartLevel = Mathf.Lerp(currentStartLevel, minStars, colorLerpingTime);
            skyBoxMat.SetFloat("_stars", currentStartLevel);
        }
        if(skyBoxMat.GetFloat("_SunMaskSize")>minStars)
        {
            float currentStartMaskLevel = skyBoxMat.GetFloat("_SunMaskSize");
            currentStartMaskLevel = Mathf.Lerp(currentStartMaskLevel, minMoonMask, moonMaskTimeLerp * Time.deltaTime);
            skyBoxMat.SetFloat("_SunMaskSize", currentStartMaskLevel);
        }
    }
    private void NightChanges()
    {
        //SkyChange
        Color topCurrentColor = skyBoxMat.GetColor("_TopColor");
        topCurrentColor = Color.Lerp(topCurrentColor, nightTopColor, colorLerpingTime * Time.deltaTime);
        skyBoxMat.SetColor("_TopColor", topCurrentColor);
        Color BottomCurrentColor = skyBoxMat.GetColor("_BottomColor");
        BottomCurrentColor = Color.Lerp(BottomCurrentColor, nightBottomColor, colorLerpingTime * Time.deltaTime);
        skyBoxMat.SetColor("_BottomColor", BottomCurrentColor);
        /*skyBoxMat.SetColor("_TopColor", nightTopColor);
        skyBoxMat.SetColor("_BottomColor", nightBottomColor);*/

        //Star
        if (skyBoxMat.GetFloat("_stars") <= maxStars)
        {
            float currentStartLevel = skyBoxMat.GetFloat("_stars");
            currentStartLevel = Mathf.Lerp(currentStartLevel, maxStars, colorLerpingTime);
            skyBoxMat.SetFloat("_stars", currentStartLevel);
        }
        if (skyBoxMat.GetFloat("_SunMaskSize") <= maxStars)
        {
            float currentStartMaskLevel = skyBoxMat.GetFloat("_SunMaskSize");
            currentStartMaskLevel = Mathf.Lerp(currentStartMaskLevel, maxMoonMask, moonMaskTimeLerp*Time.deltaTime);
            skyBoxMat.SetFloat("_SunMaskSize", currentStartMaskLevel);
        }
    }
}
