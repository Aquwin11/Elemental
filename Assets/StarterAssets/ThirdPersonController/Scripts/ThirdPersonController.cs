using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using DG.Tweening;
using System.Collections.Generic;
using Cinemachine;
using FischlWorks;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{

    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
       
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float WalkSpeed = 2.0f;
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float airSprintSpeed = 6f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float glideSpeed = 8f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;
        public float Sensitivity = 1f;
        public AudioSource playerAudio;
        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;
        public float alteredGravity = -7.5f;
        public float setMaxVerticalVelocity;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        public bool canDoubleJump=false;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        [SerializeField] private CinemachineVirtualCamera followCamera;
        public float normalCameraFOV;
        public float runCameraFOV;
        public float glideCameraFOV;
        public float duration_BetweenFOV;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;

        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDGlide;
        private int _animIDAim;
        private int _animIDMoveX;
        private int _animIDMoveY;
        private int _animIDClimb;
        private int _animIDClimbX;
        private int _animIDClimbY;
        private int _animIDVault;
        private int _animIDMotionSpeed;
        private int _animIDPunch;
        private int _animIDThrow;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        [Header("FeetGrounder")]
        [SerializeField] private LayerMask groundLayer;
        public bool canEnableAdvanceIK;
        public bool EnableAdvanceIK;
        float ikWeight = 1f; //ik will completly override animation
        public float footPlacementOffset;
        [SerializeField]csHomebrewIK IKscript;


        [Header("VisualEeffects")]
        public ParticleSystem runningWindEffect;
        public VisualEffect l_GlidingFire_Effect;
        public VisualEffect r_GlidingFire_Effect;
        public List<Transform> handRocks;

        [Header("Dash")]
        public AudioClip dashAudioClip;
        public bool isDashing;

        [Header("Gliding")]
        public AudioSource glideAudioSource;
        public AudioClip glideAudioClip;
        public float glideForce;
        public bool canGlide;
        public bool isGliding;
        [SerializeField] float glideEnergyConsum;

        [Header("RockHands")]
        public bool enablerocks=false;
        [Header("Climbing")]
        public Transform grabLocation;
        public Vector3 rayHeightoffset;
        public float climb_raylength;
        public bool isAbletoClimb;
        public float climbResetValue;
        public bool canClimb;
        public bool isClimbing;
        [SerializeField] FreeClimbScript climbingScript;
        public bool canAutoClimb;
        public float climbingAnimationRate;
        public EnergyScript energyScript;
        [SerializeField] float sprintEnergyConsum;
        [SerializeField] float climbEnergyConsum;
        public AudioClip climbrocks;

        [Tooltip("Should horizontal rotation be inverted")]
        public bool InvertXAxis = false;
        [Tooltip("Should vertical rotation be inverted")]
        public bool InvertYAxis = false;
        [Header("ControllerCostimization")]
        public float controllerHeight;
        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }
        public ChracterState chracterState;

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
            _animator = GetComponent<Animator>();
        }
        float newDis;
        private void Start()
        {
            //DOTween.SetTweensCapacity(10000, 10000);
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            
            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            LoadPlayerData();


        }

        private void Update()
        {
            if (GameManager.gameManagerInstance != null && GameManager.gameManagerInstance.gamePause)
            {
                Cursor.visible = true;
                _input.cursorLocked = false;
                return;
            }
            _hasAnimator = TryGetComponent(out _animator);
            JumpAndGravity();
            GroundedCheck();
            Move();
            Glide();
            CheckIfEnableIK();
            AimingAnimation();
            WallCheck();
            VFXParent();
            VisualEffects();
            CheckIfShoot();
            NewPlayerShoot();
            EnableCursorLock();
            //Debug.Log("its is gliding" + _verticalVelocity);
        }

        

        public void EnableCursorLock()
        {
            if (GameManager.gameManagerInstance != null && !GameManager.gameManagerInstance.gamePause)
            {
                _input.cursorLocked = true;
                Cursor.visible = false;
            }
        }
        

        float moveX;
        float moveY;
        private void AimingAnimation()
        {
            if (_hasAnimator)
            {

                if(_input.aim && Grounded)
                {
                    moveX = Mathf.Lerp(moveX, _input.move.x, climbingAnimationRate * Time.deltaTime);
                    moveY = Mathf.Lerp(moveY, _input.move.y, climbingAnimationRate * Time.deltaTime);
                    _animator.SetBool(_animIDAim, true);
                    _animator.SetLayerWeight(1, 1);
                }
                else
                {
                    _animator.SetBool(_animIDAim, false);
                    _animator.SetLayerWeight(1, 0);
                }
            }
            _animator.SetFloat(_animIDMoveX, moveX);
            _animator.SetFloat(_animIDMoveY, moveY);
        }

        private void LateUpdate()
        {
            if(GameManager.gameManagerInstance!=null && GameManager.gameManagerInstance.gamePause)
            {
                return;
            }
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDGlide = Animator.StringToHash("Gliding");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDAim = Animator.StringToHash("Aiming");
            _animIDMoveX = Animator.StringToHash("MoveX");
            _animIDMoveY = Animator.StringToHash("MoveY");
            _animIDClimb = Animator.StringToHash("Climb");
            _animIDClimbX = Animator.StringToHash("ClimbX");
            _animIDClimbY = Animator.StringToHash("ClimbY");
            _animIDVault = Animator.StringToHash("vault");
            _animIDPunch = Animator.StringToHash("Punch");
            _animIDThrow = Animator.StringToHash("shoot");

        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }
        public bool isStillMoving;
        public float distanceBetweenwall;
        public float negativeDistanceBetweenwall;
        public float wallLerpSpeed;
        public Transform backwardPosition;
        private void WallCheck()
        {
            _animator.SetBool(_animIDVault, false);
            RaycastHit wallInfo = new RaycastHit();
            if(isAbletoClimb)
            {
                if (Physics.Raycast(transform.position + rayHeightoffset, transform.forward, out wallInfo, climb_raylength, groundLayer))
                {
                    canClimb = true;
                    //Debug.Log("Distance " + Vector3.Distance(wallInfo.point, transform.position));
                    if (canClimb && (_input.interact || canAutoClimb) && !energyScript.energyReset)
                    {
                        if (Vector3.Distance(wallInfo.point, transform.position) >= distanceBetweenwall && !climbingScript.isVaulting)
                        {
                            isStillMoving = true;
                            transform.position = Vector3.Lerp(transform.position, new Vector3(grabLocation.position.x, transform.position.y, grabLocation.position.z), wallLerpSpeed * Time.deltaTime);
                        }
                        else if (Vector3.Distance(wallInfo.point, transform.position) < negativeDistanceBetweenwall && !climbingScript.isVaulting)
                        {
                            isStillMoving = true;
                            transform.position = Vector3.Lerp(transform.position, backwardPosition.position, wallLerpSpeed * Time.deltaTime);
                        }
                        else
                        {
                            isStillMoving = false;
                        }
                        if (!isAbletoClimb||!canClimb)
                        {
                            DicideToClimb(false, wallInfo);
                            return;
                        }
                        else
                        {
                            DicideToClimb(true, wallInfo);
                            if(climbingScript.isVaulting)
                            {
                                _animator.SetBool(_animIDVault, true);
                            }
                        }
                        energyScript.ConsumeEnergyOverTime(climbEnergyConsum);
                    }
                    else
                    {
                        DicideToClimb(false, wallInfo);
                    }
                    if (!isAbletoClimb || !canClimb)
                    {
                        DicideToClimb(false, wallInfo);
                        return;
                    }

                }
                else
                {
                    DicideToClimb(false, wallInfo);
                    canClimb = false;
                }
                if (!isAbletoClimb)
                {
                    DicideToClimb(false, wallInfo);
                    return;
                }
            }
            else
            {
                DicideToClimb(false, wallInfo);
                canClimb = false;
            }
        }
        public float climbX_Value = 0;
        public float climbY_Value = 0;
        private void DicideToClimb(bool isBoolean,RaycastHit hit)
        {
            
            isClimbing = isBoolean;
            climbingScript.enabled = isBoolean;
            _animator.SetBool(_animIDClimb, isBoolean);
            canEnableAdvanceIK = !isBoolean;
            IKscript.enabled = !isBoolean;

            if(isBoolean == true)
            {
                climbingScript.InitForClimb(hit);
                if(climbingScript.isAbleToMove)
                {
                    //Debug.Log("Check if the animation");
                    climbX_Value = Mathf.Lerp(climbX_Value, _input.move.x, climbingAnimationRate*Time.deltaTime);
                    climbY_Value = Mathf.Lerp(climbY_Value, _input.move.y, climbingAnimationRate * Time.deltaTime);
                }
                if (!climbingScript.isAbleToMove)
                {
                    //Debug.Log("Check if the animation Fail");
                    /*_animator.SetFloat(_animIDClimbX, 0);
                    _animator.SetFloat(_animIDClimbY, 0);*/
                    climbX_Value = Mathf.Lerp(climbX_Value, 0, climbingAnimationRate * Time.deltaTime);
                    climbY_Value = Mathf.Lerp(climbY_Value, 0, climbingAnimationRate * Time.deltaTime);
                }
                _controller.height = controllerHeight;
            }
            
            else
            {
                _controller.height = 1.8f;
            }
            _animator.SetFloat(_animIDClimbX, climbX_Value);
            _animator.SetFloat(_animIDClimbY, climbY_Value);

        }
        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * Sensitivity * (InvertXAxis?-1:1);
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * Sensitivity * (InvertYAxis ? -1 : 1);
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }
        public bool canMove=true;

        public void checkIfMove(float time)
        {
            canMove = false;
            Invoke("EnableMove", time);
        }

        public void EnableMove()
        {
            canMove = true;
        }
        private void Move()
        {
            if (isDashing)
                return;
            if (isClimbing)            
                return;
            if (!canMove)
                return;
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed;//= _input.sprint ? SprintSpeed : MoveSpeed;
            if(_input.aim && Grounded)
            {
                targetSpeed = WalkSpeed;
            }
            else if (_input.sprint && Grounded && !energyScript.energyReset)
            {
                targetSpeed = SprintSpeed;
                followCamera.m_Lens.FieldOfView = Mathf.Lerp(followCamera.m_Lens.FieldOfView, runCameraFOV, Time.deltaTime * duration_BetweenFOV);
                energyScript.ConsumeEnergyOverTime(sprintEnergyConsum);
            }
            else if(_input.sprint && !Grounded)
            {
                targetSpeed = airSprintSpeed;
            }
            else if (_input.sprint && !Grounded && isGliding)
            {
                targetSpeed = glideSpeed;
                
            }
            else
            {
                targetSpeed = MoveSpeed;
                followCamera.m_Lens.FieldOfView = Mathf.Lerp(followCamera.m_Lens.FieldOfView, normalCameraFOV, Time.deltaTime * duration_BetweenFOV);
            }

            /*if (_hasAnimator)
            {
                _animator.SetFloat(_animIDMoveX, _input.move.x);
                _animator.SetFloat(_animIDMoveY, _input.move.y);
                _animator.SetBool(_animIDAim, true);
            }*/


            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);
                if (!_input.aim)
                {
                    // rotate to face input direction relative to camera position
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }
            }

            //followCamera.m_Lens.FieldOfView = Mathf.Lerp(followCamera.m_Lens.FieldOfView, normalCameraFOV, Time.deltaTime * 10f);
            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity + (canGlide ? glideForce : 0) , 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }


        public Transform vfx_Parent;
        public Vector3 vfx_Parent_Offset;
        public void VFXParent()
        {
            vfx_Parent.position = Vector3.Lerp(vfx_Parent.position, transform.position + vfx_Parent_Offset, SpeedChangeRate);
        }
        
        private void VisualEffects()
        {
            if(isDashing)
            {
                foreach (var item in handRocks)
                {
                    item.gameObject.SetActive(false);
                }
                /*l_GlidingFire_Effect.gameObject.SetActive(false);
                r_GlidingFire_Effect.gameObject.SetActive(false);*/
            }
            else
            {
                if (_input.sprint && Grounded && (_input.move.x != 0 || _input.move.y != 0) && !energyScript.energyReset)
                {
                    runningWindEffect.gameObject.SetActive(true);
                    if (runningWindEffect.isPaused || runningWindEffect.isStopped)
                    {
                        runningWindEffect.Play();
                    }
                }
                else
                {
                    if (runningWindEffect.isPlaying)
                    {
                        runningWindEffect.Pause();
                        runningWindEffect.gameObject.SetActive(false);
                    }
                }
                if (isClimbing)
                {
                    foreach (var item in handRocks)
                    {
                        item.transform.DOScale(Vector3.one, 1.5f).SetEase(Ease.OutExpo);
                    }
                }
                else
                {
                    foreach (var item in handRocks)
                    {
                        item.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InOutExpo);
                        //item.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InOutExpo);
                    }
                    if (!isDashing)
                    {
                        if (isGliding)
                        {
                            if (isDashing)
                                return;
                            l_GlidingFire_Effect.enabled = true;
                            r_GlidingFire_Effect.enabled = true;
                        }
                        else
                        {
                            l_GlidingFire_Effect.enabled = false;
                            r_GlidingFire_Effect.enabled = false;
                        }
                    }
                }
            }
        }

        public void startCLimb(RaycastHit hit)
        {

        }

        private void CheckIfEnableIK()
        {
            if(_input.move.x ==0 && _input.move.y==0)
            {
                EnableAdvanceIK = true;
            }
            else
            {
                EnableAdvanceIK = false;
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                canDoubleJump = false;
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    //ObjectSpawer.Instance.GetObject(ObjectSpawer.ObjectType.JumpEffect, transform.position, Quaternion.Euler(90,0,0));
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * (isGliding?alteredGravity: Gravity));

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if(_verticalVelocity>=setMaxVerticalVelocity)
            {
                if (_verticalVelocity < _terminalVelocity && !isClimbing)
                {
                    _verticalVelocity += (isGliding ? alteredGravity : Gravity) * Time.deltaTime;
                }
            }
            //Debug.Log("Vertical velocity " + _verticalVelocity);
            
        }
        private void Glide()
        {
            if(!Grounded && (!Physics.Raycast(transform.position, Vector3.down, GroundedRadius * 10, groundLayer)) )
            {
                
                if (_input.sprint && canGlide && !isDashing && !energyScript.energyReset)
                {
                    glideAudioSource.enabled = true;
                    isGliding = true;
                    followCamera.m_Lens.FieldOfView = Mathf.Lerp(followCamera.m_Lens.FieldOfView, glideCameraFOV, Time.deltaTime * duration_BetweenFOV);
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDGlide, true);
                    }
                    energyScript.ConsumeEnergyOverTime(glideEnergyConsum);
                }
                else
                {
                    glideAudioSource.enabled = false;
                    isGliding = false;
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDGlide, false);
                    }
                }
            }
            else
            {
                glideAudioSource.enabled = false;
                isGliding = false;
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDGlide, false);
                }
            }
        }
        [Header("Shoot")]
        public Transform aimTarget;
        public Transform aimInialize;
        public GameObject playerBulletPrefab;
        public float playershootCounter;
        public float playershootBuffer;
        public float bulletspeed;
        public GameObject leftFireEffect;
        public float invokeFireAttack;
        public bool canShoot;
        public float shootEnergyConsumption;
        public AudioClip shootAudioClip;

        [Header("Punch")]
        public BoxCollider handcollider;
        public float invokePunch;
        public float disablePunch;
        public AudioClip punchAudioClip;
        private Vector3 PlayerShoot()
        {
            Vector3 getDirection = (aimTarget.transform.position - aimInialize.transform.position).normalized;
            return getDirection;
        }

        public void CheckIfShoot()
        {
            if (playershootCounter < playershootBuffer)
            {
                playershootCounter += Time.deltaTime;
                canShoot = false;
            }
            else
            {
                canShoot = true;
            }
        }
        private void NewPlayerShoot()
        {
            if (_input.aim && Input.GetKeyDown(KeyCode.Mouse0) && Grounded && canShoot && !energyScript.energyReset)
            {
                _animator.SetTrigger(_animIDThrow);
                leftFireEffect.SetActive(true);
                Invoke("ThrowFire", invokeFireAttack);
                //ObjectSpawer.Instance.GetObject(ObjectSpawer.ObjectType.PlayerBullet, aimInialize.position, Quaternion.LookRotation(aimTarget.position - aimInialize.position, Vector3.up));
                playershootCounter = 0;
            }
            else if(Input.GetKeyDown(KeyCode.Mouse0) /*_input.shoot*/ && Grounded && !isClimbing && !_input.aim)
            {
                _animator.SetTrigger(_animIDPunch);
                Punch();
            }
        }
        private void ThrowFire()
        {
            leftFireEffect.SetActive(false);
            playerAudio.PlayOneShot(shootAudioClip, 0.5f);
            ObjectSpawer.Instance.GetObject(ObjectSpawer.ObjectType.PlayerBullet, aimInialize.position, Quaternion.LookRotation(aimTarget.position - aimInialize.position, Vector3.up));
            energyScript.ConsumeEnergy(shootEnergyConsumption);
        }
        private void Punch()
        {
            foreach (var item in handRocks)
            {
                item.transform.DOScale(Vector3.one, 0.05f).SetEase(Ease.OutExpo);
            }
            Invoke("EnablePunch", invokePunch);
            playerAudio.PlayOneShot(punchAudioClip, 0.5f);
        }
        public void EnablePunch()
        {
            handcollider.enabled = true;
            Invoke("DisablePunch", disablePunch);
        }

        private void DisablePunch()
        {
            handcollider.enabled = false;
            foreach (var item in handRocks)
            {
                item.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InExpo);
            }
        }

        private Vector3 GetDirection(Transform forwardT)
        {
            Vector3 direction = new Vector3();
            
            if (_input.move.x == 0 && _input.move.y == 0)
            {
                direction = forwardT.forward;
            }
            else
            {
                direction = forwardT.forward * _input.move.y + forwardT.right * _input.move.x;
            }
            return direction.normalized;
        }
        /*public void NewEnableDoubleJump()
        {
            canDoubleJump = true;
            Debug.Log("Check if DoubleJump");
        }*/


        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);

            Gizmos.DrawRay(transform.position,transform.position +  Vector3.down * GroundedRadius * 10);
            Gizmos.DrawRay(transform.position + rayHeightoffset, transform.forward * climb_raylength);

        }
        
        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    playerAudio.PlayOneShot(FootstepAudioClips[index]);
                    //AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }
        private void OnClimbstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                playerAudio.PlayOneShot(climbrocks);
                //AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                playerAudio.PlayOneShot(LandingAudioClip);
                //AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (_animator && EnableAdvanceIK && canEnableAdvanceIK)
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, ikWeight);
                _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, ikWeight);

                _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, ikWeight);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, ikWeight);

                Vector3 rayDir = Vector3.down;
                // for left foot
                AdjustFootIK(AvatarIKGoal.LeftFoot, rayDir);
                //for the right foot
                AdjustFootIK(AvatarIKGoal.RightFoot, rayDir);
            }
        }
        private void AdjustFootIK(AvatarIKGoal ikGoal, Vector3 rayDir)
        {
            Vector3 rayStartPos = _animator.GetIKPosition(ikGoal) + Vector3.up;
            // raycast origin starts from the foot location + offset of 1 unit in up dir   
            bool isGround = Physics.Raycast(rayStartPos, rayDir, out RaycastHit hitInfo, 2f, groundLayer);
            // check for ground detection   
            if (isGround) // touching ground   
            {
                Vector3 hitPos = hitInfo.point;
                _animator.SetIKPosition(ikGoal, hitPos);
            }
        }

        public void SetSensitivity(float newSensitivity)
        {
            Sensitivity = newSensitivity;
        }

        public void DropFromClimb()
        {
            isAbletoClimb = false;
            canClimb = false;
            isClimbing = false;
            _animator.SetBool(_animIDClimb, false);
            //_animator.SetBool(_animIDGrounded, true);
            Invoke("setClimbValue", climbResetValue);
        }

        private void setClimbValue()
        {
            //Debug.Log("Reset Value");
            isAbletoClimb = true;
        }
        public void LoadPlayerData()
        {
            if (PlayerPrefs.HasKey("SettingsControlsInvertX"))
            {
                if (PlayerPrefs.GetInt("SettingsControlsInvertX") == 1) InvertXAxis = true;
                else InvertXAxis = false;
            }
            if (PlayerPrefs.HasKey("SettingsControlsInvertY"))
            {
                if (PlayerPrefs.GetInt("SettingsControlsInvertY") == 1) InvertYAxis = true;
                else InvertYAxis = false;
            }
        }
    }

    public enum ChracterState
    {
        Walk,
        Aim,
        Sprint,
        Climbing,
        Gliding,
        Dash
    }
}