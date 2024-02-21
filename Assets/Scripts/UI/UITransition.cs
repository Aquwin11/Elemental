using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class UITransition : MonoBehaviour
{
    [Header("UIObjects")]
    public CanvasGroup ImageAlpha;
    public Transform ObjectToMove;
    public GameObject DisableObject;


    [Header("LerpDuration")]
    public float AlphaDuration;
    public float EnableDuration;
    public float DisableDuration;


    [Header("Set Ease")]
    public Ease openEase;
    public Ease closeEase;

    [Header("Transition from")]
    public TransitionFrom transitionFrom; 
    public float posOrnegFrom;
    public TransitionFrom transitionTo;
    public float posOrnegTo;
    private void OnEnable()
    {
        OpenAlphaAndDialog();
    }
    public void OpenAlphaAndDialog()
    {
        if (ImageAlpha != null)
        {
            ImageAlpha.alpha = 0;
            ImageAlpha.DOFade(1, AlphaDuration);
        }
        if (ObjectToMove != null)
        {
            if (transitionFrom == TransitionFrom.Width)
            {
                ObjectToMove.localPosition = new Vector2(posOrnegFrom*Screen.width, 0);
                ObjectToMove.DOLocalMoveX(0, EnableDuration).SetEase(openEase);
            }
            else
            {
                ObjectToMove.localPosition = new Vector2(0, posOrnegFrom * Screen.height);
                ObjectToMove.DOLocalMoveY(0, EnableDuration).SetEase(openEase);
            }
        }
    }
    public void CloseAlphaDialog()
    {
        if (ImageAlpha != null)
        {
            ImageAlpha.DOFade(0, 0.5f);
        }
        if (ObjectToMove != null)
        {
            if(transitionTo == TransitionFrom.Height)
            {
                ObjectToMove.DOLocalMoveY(posOrnegTo * Screen.height, DisableDuration).SetEase(closeEase).OnComplete(onDisableObject);
            }
            else
            {
                ObjectToMove.DOLocalMoveX(posOrnegTo * Screen.width, DisableDuration).SetEase(closeEase).OnComplete(onDisableObject);
            }
        }
    }
    public void onDisableObject()
    {
        DisableObject.SetActive(false);
    }

    public enum TransitionFrom
    {
        Width,
        Height
    }
}
