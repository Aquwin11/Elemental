using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{

    [Header("References")]
    public InputManager IM;
    public Rigidbody PlayerRigid;


    [Header("Movement")]
    private float movespeed;
    public float walkspeed;
    public float sprintspeed;
    public Transform move_orientation;
    public Vector3 moveDirection;
    public float Grounddrag;

    [Header("Ground")]
    public float Height;
    public LayerMask whatisGround;
    public bool isGrounded;

    [Header("Jump")]
    public float jumpforce;
    public float jumpcoolDown;
    public float airMultiplier;
    public bool canJump=true;
    public bool canDoubleJump = false;

    [Header("Slope")]
    public float maxslopeAngle;
    public RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Dashing")]
    public float dashspeed;
    public bool isdashing;
    public float dashSpeedChangeFactor;

    [Header("Momentum")]
    private float desiredMoveSpeed;
    private float lastDesiredMovespeed;
    private MoveState lastmovestate;
    private bool keepMomentum;

    [Header("States")]
    public MoveState moveState;

    public enum MoveState
    {
        walking,
        sprinting,
        dashing,
        air
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerRigid = GetComponent<Rigidbody>();
        PlayerRigid.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {

        
        //AddDrag
        if (moveState == MoveState.dashing || moveState == MoveState.walking || moveState == MoveState.sprinting)
            PlayerRigid.drag = Grounddrag;
            
        else
            PlayerRigid.drag = 0;

        //Jump
        if(IM.jumpInput && canJump && isGrounded)
        {
            canJump = false;
            PlayerJump();
            Invoke(nameof(ResetJump), jumpcoolDown); 
        }
        //ouble Jump
        else if (IM.jumpInput && canDoubleJump && !isGrounded)
        {
            canDoubleJump = false;
            PlayerJump();
        }
        //state handler
        MovementStateHandlere();
    }

    void FixedUpdate()
    {
        //CheckifGrounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, Height + 0.25f, whatisGround);

        //movement
        MovePlayer();
        speedControl();
    }
    //movement
    public void MovePlayer()
    {
        if (moveState == MoveState.dashing) return;
        if (OnSlope() && !exitingSlope)
        {
            PlayerRigid.AddForce(GetSlopeMoveDirection() * movespeed * 20f, ForceMode.Force);
            if(PlayerRigid.velocity.y>0f)
            {
                PlayerRigid.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        if (IM.movementInput.x!=0 || IM.movementInput.y!=0)
        {
            moveDirection = move_orientation.forward * IM.movementInput.y + move_orientation.right * IM.movementInput.x;
            //
            if (isGrounded)
                PlayerRigid.AddForce(moveDirection.normalized * movespeed * 10f, ForceMode.Force);
            else if (!isGrounded)
                PlayerRigid.AddForce(moveDirection.normalized * movespeed * 10f * airMultiplier, ForceMode.Force);
        }
        else
        {
            PlayerRigid.velocity = new Vector3(0, PlayerRigid.velocity.y, 0);
        }
        PlayerRigid.useGravity = !OnSlope();
    }


    //Control the movement speed;
    private void speedControl()
    {
        //limitspeed on slope
        if(OnSlope() && !exitingSlope)
        {
            if(PlayerRigid.velocity.magnitude>movespeed)
            {
                PlayerRigid.velocity = PlayerRigid.velocity.normalized * movespeed;
            }
        }
        else
        {
            Vector3 flatvel = new Vector3(PlayerRigid.velocity.x, 0, PlayerRigid.velocity.z);

            //limit
            Vector3 limitedVel = flatvel.normalized * movespeed;
            PlayerRigid.velocity = new Vector3(limitedVel.x, PlayerRigid.velocity.y, limitedVel.z);
        }
    }


    //Jumping
    private void PlayerJump()
    {
        exitingSlope = true;
        PlayerRigid.velocity = new Vector3(PlayerRigid.velocity.x, 0f, PlayerRigid.velocity.z);
        PlayerRigid.AddForce(transform.up * jumpforce, ForceMode.Impulse);
            
    }

    //ResetJump
    private void ResetJump()
    {
        canJump = true;
        exitingSlope = false;
        canDoubleJump = true;
    }

    //Movement state handler
    private void MovementStateHandlere()
    {
        if (!isGrounded)
        {
            moveState = MoveState.air;
            if (desiredMoveSpeed < sprintspeed)
            {
                desiredMoveSpeed = walkspeed;
            }
            else
            {
                desiredMoveSpeed = walkspeed;
            }

        }
        else if(isdashing)
        {
            moveState = MoveState.dashing;
            desiredMoveSpeed = dashspeed;
            speedChangeFactore = dashSpeedChangeFactor;
        }
        else if (isGrounded && IM.sprintInput)
        {
            moveState = MoveState.sprinting;
            desiredMoveSpeed = sprintspeed;
        }
        else if (isGrounded && !IM.sprintInput)
        {
            moveState = MoveState.walking;
            desiredMoveSpeed = walkspeed;
        }

        bool desiredMovespeedHasChanged = desiredMoveSpeed != lastDesiredMovespeed;
        if (lastmovestate == MoveState.dashing) keepMomentum = true;
        if(desiredMovespeedHasChanged)
            if(keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothLerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                movespeed = desiredMoveSpeed;
            }
        lastDesiredMovespeed = desiredMoveSpeed;
        lastmovestate = moveState;
    }


    //slopeCheck
    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position,Vector3.down,out slopeHit,Height+0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxslopeAngle && angle!=0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
    private float speedChangeFactore;

    private IEnumerator SmoothLerpMoveSpeed()
    {
        float time = 0f;
        float difference = Mathf.Abs(desiredMoveSpeed - movespeed);
        float startValue = movespeed;

        float boostFactor = speedChangeFactore;
        while ( time<difference)
        {
            movespeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            time += Time.deltaTime * boostFactor;
            yield return null;
        }
        movespeed = desiredMoveSpeed;
        speedChangeFactore = 1f;keepMomentum = false;
    }
}
