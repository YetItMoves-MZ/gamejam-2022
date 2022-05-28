using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    #region serializable fields

    [Header("Components")]
    [Tooltip("The character controller component")]
    [SerializeField] private CharacterController controller;
    [Tooltip("The player's camera (main camera)")]
    [SerializeField] private Transform playerCamera;

    [Header("Crouching Variables")]
    [Tooltip("The height of the character while crouching")]
    [SerializeField] private float heightChange = 0.5f;

    [Header("Dashing Variables")]
    [Tooltip("Dashing speed")]
    [SerializeField] private float dashingModifier;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool IsGrounded = true;
    [Tooltip("Useful for rough ground. If you are not sure, keep on 0")]
    public float GroundedOffset = 0.0f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.5f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Header("Movement Variables")]
    [Tooltip("Movement speed")]
    [SerializeField] private float MovementSpeed;
    [Tooltip("Acceleration and deceleration")]
    [SerializeField] private float SpeedChangeRate = 10.0f;

    [Header("Y movement parameters")]
    [SerializeField] private float JumpHeight;
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    [SerializeField] private float JumpTimeout = 0.50f;
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    [SerializeField] private float FallTimeout = 0.15f;
    [Tooltip("Maximum falling speed (should allways be a negative number)")]
    [SerializeField] private float TerminalVelocity = -53.0f;


    [Header("Animation Variables")]
    [Tooltip("When to start landing animation")]
    [SerializeField] private float AnimationGroundOffset = -0.8f;

    [Header("Scripts")]
    [Tooltip("The UnlockHandler Script located in the UnlockHandler Object")]
    [SerializeField] private UnlocksHandler UnlockHandler;
    [Tooltip("The PlayerInteraction script located in the player game object")]
    [SerializeField] private PlayerInteraction playerInteraction;

    #endregion

    #region properties

    #region components
    private Animator _playerAnim;

    private Animator PlayerAnim
    {
        get
        {
            if (this._playerAnim == null)
                this._playerAnim = GetComponentInChildren<Animator>();

            return this._playerAnim;
        }
    }

    private CapsuleCollider _cpslCollider;

    private CapsuleCollider CpslCollider
    {
        get
        {
            if (this._cpslCollider == null)
                this._cpslCollider = GetComponent<CapsuleCollider>();
            return this._cpslCollider;
        }
    }
    #endregion

    #region booleans

    private bool IsDashing
    {
        get
        {
            return Input.GetAxis("Dash") > 0;
        }
    }

    private bool IsMoving
    {
        get
        {
            return Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        }
    }

    public bool IsCrouching { get; set; }

    public bool IsRolling { get; set; }
    public bool IsJumping
    {
        get
        {
            return Input.GetAxis("Jump") > 0;
        }
    }

    #endregion

    #region starting properties
    private float CapsuleColliderStartingHeight { get; set; }
    private float OriginalStepOffset { get; set; }
    private float PlayerStartHeight { get; set; }
    private float ColliderStartHeight { get; set; }

    #endregion

    #region Timeout delta
    private float FallTimeoutDelta { get; set; }
    private float JumpTimeoutDelta { get; set; }
    #endregion

    #region movemment changes
    private float VerticalVelocity { get; set; }
    private float TargetRotation { get; set; }
    private Vector3 TargetDirection { get; set; }
    #endregion
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        this.OriginalStepOffset = controller.stepOffset;
        this.PlayerStartHeight = controller.height;
        this.ColliderStartHeight = controller.center.y;
        this.CapsuleColliderStartingHeight = this.CpslCollider.height;

        // Locking cursor to not show it while moving.
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerInteraction.IsPlayerDead)
        {
            if (UnlockHandler.IsPowerActive[(int)UnlocksHandler.EPowers.Movement])
            {
                this.HandleMovement();
            }
            if (UnlockHandler.IsPowerActive[(int)UnlocksHandler.EPowers.Crawl])
            {
                this.HandleCrouchInput();
            }

            this.HandleJump();

            this.GroundedCheck();
        }
    }


    private void HandleMovement()
    {
        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = 0;
        if (!this.IsMoving)
        {
            targetSpeed = 0.0f;
            PlayerAnim.SetFloat("Speed", 0);
        }
        else
        {
            if (this.IsDashing && UnlockHandler.IsPowerActive[(int)UnlocksHandler.EPowers.Dash])
            {
                targetSpeed = this.dashingModifier * this.MovementSpeed;
                PlayerAnim.SetFloat("Speed", 6);
            }
            else
            {
                targetSpeed = this.MovementSpeed;
                PlayerAnim.SetFloat("Speed", 2);
            }
        }



        if (!this.IsMoving) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float speed;

        //not useing analog at the moment so i commented that for now. TODO: insert analog movement later... maybe?
        //float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
        float inputMagnitude = 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * this.SpeedChangeRate);

            // round speed to 3 decimal places
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        // normalise input direction
        Vector3 inputDirection = new Vector3(horizontal, 0.0f, vertical).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude



        TargetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
        //float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, TargetRotation, ref RotationVelocity, rotationSmoothTime);

        // rotate to face input direction relative to camera position
        // transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);



        // rotate to face camera direction
        // if (Input.GetAxisRaw("Camera Unlocked") == 0)
        //{
        transform.rotation = new Quaternion(0.0f, playerCamera.rotation.y, 0.0f, playerCamera.rotation.w);
        //}
        TargetDirection = Quaternion.Euler(0.0f, TargetRotation, 0.0f) * Vector3.forward;

        // move the player
        if (!this.IsRolling)
            controller.Move(TargetDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, VerticalVelocity, 0.0f) * Time.deltaTime);

        // update animator if using character
        // if (_hasAnimator)
        // {
        //     _animator.SetFloat(_animIDSpeed, _animationBlend);
        //     _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        // }
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        IsGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        if (IsGrounded)
            PlayerAnim.SetBool("isGrounded", true);
        else
            PlayerAnim.SetBool("isGrounded", false);

        Vector3 landingSphereAnim = new Vector3(transform.position.x, transform.position.y - AnimationGroundOffset, transform.position.z);
        if (Physics.CheckSphere(landingSphereAnim, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore))
            PlayerAnim.SetBool("isGrounded", true);

        // update animator if using character
        // if (_hasAnimator)
        // {
        //     _animator.SetBool(_animIDGrounded, Grounded);
        // }
    }

    private void HandleJump()
    {
        if (IsGrounded)
        {
            // reset the fall timeout timer
            FallTimeoutDelta = FallTimeout;

            this.PlayerAnim.SetBool("isJumping", false);
            this.PlayerAnim.SetBool("isFalling", false);

            // update animator if using character
            // if (_hasAnimator)
            // {
            //     _animator.SetBool(_animIDJump, false);
            //     _animator.SetBool(_animIDFreeFall, false);
            // }

            // stop our velocity dropping infinitely when grounded
            if (VerticalVelocity < 0.0f)
            {
                VerticalVelocity = -2f;
            }

            // Jump        

            if (IsJumping && JumpTimeoutDelta <= 0.0f && UnlockHandler.IsPowerActive[(int)UnlocksHandler.EPowers.Jump])
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                VerticalVelocity = Mathf.Sqrt(this.JumpHeight * -2f * Physics.gravity.y);

                PlayerAnim.SetBool("isJumping", true);
                PlayerAnim.SetBool("isGrounded", false);
                //PlayerAnim.SetBool("isGrounded", false);

                // update animator if using character
                // if (_hasAnimator)
                // {
                //     _animator.SetBool(_animIDJump, true);
                // }
            }

            // jump timeout
            if (JumpTimeoutDelta >= 0.0f)
            {
                JumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            JumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (FallTimeoutDelta >= 0.0f)
            {
                FallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                PlayerAnim.SetBool("isFalling", true);
                // update animator if using character
                // if (_hasAnimator)
                // {
                //     _animator.SetBool(_animIDFreeFall, true);
                // }
            }
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (VerticalVelocity > TerminalVelocity)
        {
            VerticalVelocity += Physics.gravity.y * Time.deltaTime;
        }
    }

    private void HandleCrouchInput()
    {
        if (Input.GetButtonDown("Crouch") && !this.IsCrouching)
        {
            // Debug.Log("Crouch on");
            this.PlayerAnim.SetBool("Crouching", true);
            this.IsCrouching = true;
            this.controller.height = this.PlayerStartHeight * 0.5f;
            this.controller.center = new Vector3(this.controller.center.x, this.heightChange, this.controller.center.z);
            this.CpslCollider.height = this.CapsuleColliderStartingHeight / 2;

        }
        else if (Input.GetButtonDown("Crouch") && this.IsCrouching)
        {
            // Debug.Log("Crouch off");
            this.PlayerAnim.SetBool("Crouching", false);
            this.IsCrouching = false;
            this.controller.height = this.PlayerStartHeight;
            this.controller.center = new Vector3(controller.center.x, this.ColliderStartHeight, controller.center.z);
            this.CpslCollider.height = this.CapsuleColliderStartingHeight;
        }
    }

}