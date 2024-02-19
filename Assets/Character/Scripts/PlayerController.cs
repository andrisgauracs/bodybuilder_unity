using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    StateMachine stateMachine;
    Animator animator;
    Rigidbody characterController;
    bool isMoving = false;
    bool isGrounded = true;
    bool isJumping = false;
    bool isRunning = false;
    bool jumpInitiated = true;
    Vector3 currentMovement;
    Vector3 lastMovement;
    Vector2 currentMovementInput;
    Dictionary<string, int> animations;
    float turnVelocity;
    float targetAngle;
    float maxSlopeAngle = 40f;
    RaycastHit slopeHit;
    float playerHeight = 0.5f;
    PlayerInput input;

    [SerializeField] float jumpForce = 8f;
    [SerializeField] new Transform camera;
    [SerializeField] float speed;
    [SerializeField] float groundDrag;
    [SerializeField] float airResistence;
    [SerializeField] LayerMask ground;

    void Awake()
    {
        animations = new Dictionary<string, int>(){
            {"Idle", Animator.StringToHash("Idle")},
            {"Walk", Animator.StringToHash("Walk")},
            {"Jumping", Animator.StringToHash("Jumping")},
            {"Run", Animator.StringToHash("Run")},
        };

        animator = gameObject.GetComponent<Animator>();
        characterController = gameObject.GetComponent<Rigidbody>();
        characterController.freezeRotation = true;
        input = gameObject.GetComponent<PlayerInput>();

        stateMachine = new StateMachine();
        var idleState = new IdleState(this, animator);
        var walkingState = new WalkingState(this, animator);
        var runningState = new RunningState(this, animator);
        var jumpState = new JumpState(this, animator);

        At(idleState, walkingState, new FuncPredicate(() => isMoving));
        At(idleState, runningState, new FuncPredicate(() => isMoving && isRunning));
        At(walkingState, runningState, new FuncPredicate(() => isMoving && isRunning));
        At(runningState, walkingState, new FuncPredicate(() => isMoving && !isRunning));
        At(runningState, idleState, new FuncPredicate(() => !isMoving));
        At(walkingState, idleState, new FuncPredicate(() => !isMoving));
        At(jumpState, walkingState, new FuncPredicate(() => isGrounded && isMoving));
        At(jumpState, idleState, new FuncPredicate(() => isGrounded));
        Any(jumpState, new FuncPredicate(() => isJumping));

        stateMachine.SetState(idleState);
    }

    public int GetAnimation(string name) { return animations[name]; }

    void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    void Update()
    {
        GroundCheck();
        stateMachine.Update();
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
        HandleMovement();
        HandleRotation();
    }

    Vector3 GetMoveDirection() { return isMoving ? Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward : Vector3.zero; }

    public void HandleRotation()
    {
        Vector3 dir = isMoving ? currentMovement.normalized : lastMovement.normalized;
        targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, 0.1f);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    void HandleMovement()
    {
        Vector3 moveDirection = GetMoveDirection();
        float currentSpeed = isRunning ? speed * 2 : speed;
        if (OnSlope())
        {
            characterController.AddForce(GetSlopeDirection() * currentSpeed, ForceMode.Force);
        }
        characterController.AddForce(moveDirection.normalized * currentSpeed * (!isGrounded ? airResistence : 1f), ForceMode.Force);
        Vector3 groundVelocity = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z);
        if (groundVelocity.magnitude > currentSpeed)
        {
            Vector3 limitedVelocity = characterController.velocity.normalized * currentSpeed;
            characterController.velocity = new Vector3(limitedVelocity.x, characterController.velocity.y, limitedVelocity.z);
        }
    }

    public void Jump()
    {
        if (isJumping)
        {
            characterController.velocity = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z);
            characterController.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        isMoving = context.performed ? true : context.canceled ? false : false;
        if (isMoving) lastMovement = currentMovement;
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton() && isGrounded;
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton() && isGrounded)
        {
            isJumping = true;
            jumpInitiated = true;
            Jump();
            Invoke(nameof(ResetJump), 0.3f);
        }
    }
    void ResetJump()
    {
        isJumping = false;
    }

    void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position + new Vector3(0, playerHeight, 0), Vector3.down, playerHeight + 0.1f, ground);
        if (isGrounded && !isJumping) jumpInitiated = false;
        characterController.drag = isGrounded ? groundDrag : 0;
    }

    bool OnSlope()
    {
        if (Physics.Raycast(transform.position + new Vector3(0, playerHeight, 0), Vector3.down, out slopeHit, playerHeight + 0.1f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    Vector3 GetSlopeDirection()
    {
        return Vector3.ProjectOnPlane(GetMoveDirection(), slopeHit.normal).normalized;
    }
}
