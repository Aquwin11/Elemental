using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Reference")]
    public InputManager PlayerIM;
    public Rigidbody playerRigidBody;
    public ThirdPersonMovement movementScript;
    public Transform Playerorientation;
    public Transform playerCamera;

    [Header("Dash")]
    public float dashforce;
    public float dashupwardforce;
    public float dashDuration;

    public float dashCD;
    public float dashCDTimer;


    [Header("Setting")]
    public bool allowAllDirection;
    public bool disableGravity = false;
    //public bool resetVal = true;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody>();
        movementScript = GetComponent<ThirdPersonMovement>(); 
    }




    // Update is called once per frame
    void Update()
    {
        if (PlayerIM.dashInput)
            Dashing();


        if(dashCDTimer>0)
        {
            dashCDTimer -= Time.deltaTime;
        }
    }
    private Vector3 delayForceToApply;
    public void Dashing()
    {
        if (dashCDTimer > 0) return;
        else dashCDTimer = dashCD;
        movementScript.isdashing = true;
        Transform forwardT;
        forwardT = Playerorientation;

        Vector3 direction = GetDirection(forwardT);


        Vector3 forceApply = direction * dashforce + Playerorientation.up * dashupwardforce;

        if (disableGravity)
            playerRigidBody.useGravity = false; 
        delayForceToApply = forceApply;
        Invoke(nameof(DelayAddForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    
    private void DelayAddForce()
    {
        playerRigidBody.AddForce(delayForceToApply, ForceMode.Impulse);
    }
    private void ResetDash()
    {
        if (disableGravity)
            playerRigidBody.useGravity = true;
        movementScript.isdashing = false;
    }


    private Vector3 GetDirection(Transform forwardT)
    {
        Vector3 direction = new Vector3();
        if(allowAllDirection)
        {
            direction = forwardT.forward * PlayerIM.movementInput.y + forwardT.right * PlayerIM.movementInput.x;
        }
        if(PlayerIM.movementInput.x==0 && PlayerIM.movementInput.y==0)
        {
            direction = forwardT.forward;
        }
        return direction.normalized;
    }
}
