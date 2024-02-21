using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SwitchToggle : MonoBehaviour
{
    [SerializeField] RectTransform uiHandlerRectTransform;
    Transform UihandlePos;
    Toggle toggle;
    Vector2 handlePosition;
    [SerializeField] Color BackgroundColor;
    [SerializeField] Color SelecterColor;

    public AudioClip toogleAudioClip;
    public AudioSource toogleAudioSource;

    Color BackgroundDefaultColor, SelecterDefaultColor;
    Image BackgroundDefaultImage, SelecterDefaultImage;
    // Start is called before the first frame update
    void Awake()
    {
        UihandlePos = uiHandlerRectTransform.GetComponent<Transform>();
        toggle = GetComponent<Toggle>();
        handlePosition = uiHandlerRectTransform.anchoredPosition;
        toggle.onValueChanged.AddListener(OnSwitch);
        if (toggle.isOn)
        {
            OnSwitch(true);
        }
        BackgroundDefaultColor = uiHandlerRectTransform.parent.GetComponent<Image>().color;
        SelecterDefaultColor = uiHandlerRectTransform.GetComponent<Image>().color;
    }
    public void OnSwitch(bool on)
    {
        if (on)
        {
            UihandlePos.DOLocalMoveX(UihandlePos.localPosition.x * -1, 0.2f).SetEase(Ease.InCirc).OnComplete(onToggle);
            toogleAudioSource.PlayOneShot(toogleAudioClip);

        }

        else
        {
            UihandlePos.DOLocalMoveX(UihandlePos.localPosition.x * -1, 0.2f).SetEase(Ease.InCirc).OnComplete(offToggle);
            toogleAudioSource.PlayOneShot(toogleAudioClip);
        }


    }


    void onToggle()
    {
        uiHandlerRectTransform.parent.GetComponent<Image>().color = BackgroundColor;
        uiHandlerRectTransform.GetComponent<Image>().color = SelecterColor;
    }

    void offToggle()
    {
        uiHandlerRectTransform.parent.GetComponent<Image>().color = BackgroundDefaultColor;
        uiHandlerRectTransform.GetComponent<Image>().color = SelecterDefaultColor;
    }
}
