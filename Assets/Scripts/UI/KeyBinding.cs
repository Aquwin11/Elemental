using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyBinding : MonoBehaviour
{
    [SerializeField] private InputActionReference jumpAction = null;
    [SerializeField] private PlayerInput playerInput = null;
    [SerializeField] private TMP_Text bindingDisplayName = null;
    [SerializeField] private GameObject startReBinding = null;
    [SerializeField] private GameObject waitForReBinding = null;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private const string RebindsKey = "rebinds";

    private void Start()
    {
        string rebinds = PlayerPrefs.GetString(RebindsKey, string.Empty);

        if (string.IsNullOrEmpty(rebinds)) { return; }

        playerInput.actions.LoadBindingOverridesFromJson(rebinds);

        int bindingIndex = jumpAction.action.GetBindingIndexForControl(jumpAction.action.controls[0]);

        bindingDisplayName.text = InputControlPath.ToHumanReadableString(
            jumpAction.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    }
    public void Save()
    {
        string rebinds = playerInput.actions.SaveBindingOverridesAsJson();

        PlayerPrefs.SetString(RebindsKey, rebinds);
    }

    public void StartRebinding()
    {
        Debug.Log("Check if eneter");
        startReBinding.SetActive(false);
        waitForReBinding.SetActive(true);

        playerInput.SwitchCurrentActionMap("Menu");

        rebindingOperation = jumpAction.action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete())
            .Start();
    }

    private void RebindComplete()
    {
        int bindingIndex = jumpAction.action.GetBindingIndexForControl(jumpAction.action.controls[0]);

        bindingDisplayName.text = InputControlPath.ToHumanReadableString(
            jumpAction.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        rebindingOperation.Dispose();

        startReBinding.SetActive(true);
        waitForReBinding.SetActive(false);

        playerInput.SwitchCurrentActionMap("Player");
    }
}