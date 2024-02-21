using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playercontrols;
    public Vector2 movementInput;

    public bool jumpInput;
    public bool sprintInput;
    public bool dashInput;
    public bool interactInput;
    public bool aimInput;
    public bool shootInput;

    private void OnEnable()
    {
        if(playercontrols==null)
        {
            playercontrols = new PlayerControls();
            playercontrols.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playercontrols.PlayerAction.Jump.performed += i => jumpInput = true;
            playercontrols.PlayerAction.Jump.canceled += i => jumpInput = false;
            playercontrols.PlayerAction.Sprint.performed += i => sprintInput = true;
            playercontrols.PlayerAction.Sprint.canceled += i => sprintInput = false;
            playercontrols.PlayerAction.Dash.performed += i => dashInput = true;
            playercontrols.PlayerAction.Dash.canceled += i => dashInput = false;
            playercontrols.PlayerAction.Interact.performed += i => interactInput = true;
            playercontrols.PlayerAction.Interact.canceled += i => interactInput = false;

        }
        playercontrols.Enable();
    }

    private void OnDisable()
    {
        playercontrols.Disable();
    }
}
