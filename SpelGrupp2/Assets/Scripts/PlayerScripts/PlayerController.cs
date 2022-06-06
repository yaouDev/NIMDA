using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private Animator anim; //cc animation

    private Collider[] overlapCollidersNonAlloc = new Collider[10];
    private GameObject[] players;
    private GameObject otherPlayer;
    private CapsuleCollider capsuleCollider;
    private StateMachine stateMachine;
    private Transform myCamera;
    private Vector3 aimingDirection = Vector3.forward;
    private Vector3 debugCollider;
    private Vector3 planeNormal;
    private Vector3 point1;
    private Vector3 point2;
    private Vector2 joyStickLeftInput;
    private Vector2 joyStickRightInput;
    private bool grounded;
    private float colliderRadius;
    bool movementSpeedUpgraded;

    protected bool alive = true;

    [FormerlySerializedAs("_velocity")] [HideInInspector] public Vector3 velocity;
    [FormerlySerializedAs("_inputMovement")] [HideInInspector] public Vector3 inputMovement;
    [HideInInspector] public float airControl = 1.0f;
    [FormerlySerializedAs("_debugColor")] public Color DebugColor = new Color(10, 20, 30);

    [FormerlySerializedAs("states")] [SerializeField] public List<State> States;

    [FormerlySerializedAs("PefaultGravity")]
    [FormerlySerializedAs("_defaultGravity")]
    [FormerlySerializedAs("_gravity")]
    [SerializeField]
    [Range(0.0f, 20.0f)]
    [Tooltip("Set default gravity in:\nEdit > Project Settings > Physics > Gravity")]
    public float DefaultGravity;

    [FormerlySerializedAs("jumpFallVelocityMultiplier")] [SerializeField] [Range(0.0f, 4.0f)] 
    public float JumpFallVelocityMultiplier = 2.0f;

    [FormerlySerializedAs("_acceleration")]
    [Space(10)]
    [Header("Character Design")]
    [SerializeField]
    [Range(0.0f, 15.0f)]
    private float acceleration = 3.0f;

    [FormerlySerializedAs("_deceleration")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    [Tooltip("The deceleration when no input")]
    private float deceleration = 1.5f;

    [FormerlySerializedAs("_turnSpeedModifier")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    [Tooltip("Extra force when character is turning the opposite way")]
    private float turnSpeedModifier = 2.0f;

    [FormerlySerializedAs("_terminalVelocity")]
    [SerializeField]
    [Range(0.0f, 20.0f)]
    [Tooltip("Max speed")]
    private float terminalVelocity = 12.0f;

    [FormerlySerializedAs("_upgradedTerminalVelocity")]
    [SerializeField]
    [Range(0.0f, 30.0f)]
    [Tooltip("Upgraded max speed")]
    private float upgradedTerminalVelocity = 18f;

    [FormerlySerializedAs("_jumpForce")]
    [SerializeField]
    [Range(0.0f, 20.0f)]
    [Tooltip("Set before hitting [\u25BA]\nOnly changed during start")]
    public float JumpForce = 10.0f;

    [FormerlySerializedAs("_kineticFrictionCoefficient")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    [Tooltip("Force applied when moving\n(60-70% of static friction usually)")]
    private float kineticFrictionCoefficient = 0.2f;   // TODO rename to _dynamicFrictionCoefficient ? 

    [FormerlySerializedAs("_airResistanceCoefficient")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    [Tooltip("Force affecting velocity")]
    private float airResistanceCoefficient = .5f;

    [FormerlySerializedAs("_collisionMask")]
    [Space(10)]
    [Header("Character Controller Implementation Details")]
    [SerializeField]
    [Tooltip("What LayerMask(s) the character should collide with")]
    private LayerMask collisionMask;

    [FormerlySerializedAs("_skinWidth")]
    [SerializeField]
    [Range(0.0f, 0.15f)]
    [Tooltip("The distance the character should stop before a collider")]
    private float skinWidth = 0.1f;

    [FormerlySerializedAs("_groundCheckDistance")]
    [SerializeField]
    [Range(0.0f, 0.2f)]
    [Tooltip("The distance the character should count as being grounded")]
    private float groundCheckDistance = 0.15f;

    [SerializeField] private GameObject visuals;

    private Vector2 reference;
    private Vector2 inputVectorUnSmoothed;

    private void Awake()
    {
        stateMachine = new StateMachine(this, States);
        capsuleCollider = GetComponent<CapsuleCollider>();
        myCamera = GetComponentInChildren<Camera>().transform;
    }

    private void Start()
    {
        upgradedTerminalVelocity = terminalVelocity * 2;
        DefaultGravity = -Physics.gravity.y;
        colliderRadius = capsuleCollider.radius;
        point1 = capsuleCollider.center + Vector3.up * (capsuleCollider.height / 2 - colliderRadius);
        point2 = capsuleCollider.center + Vector3.down * (capsuleCollider.height / 2 - colliderRadius);
        players = GameObject.FindGameObjectsWithTag("Player");
        otherPlayer = players[0] != gameObject ? players[0] : players[1];
    }

    private void Update()
    {
        joyStickRightInput = Vector2.SmoothDamp(joyStickRightInput, inputVectorUnSmoothed, ref reference, .05f, 100.0f);

        if (alive)
        {
            Grounded();
            ApplyJoystickMovement();
            stateMachine.Run();

            if (anim != null)
            {
                Vector3 rot = transform.rotation.eulerAngles;
                Vector3 rotatedVelocity = Quaternion.Euler(rot.x, -rot.y, rot.z) * velocity;

                anim.SetFloat("Speed", rotatedVelocity.x);
                anim.SetFloat("Direction", rotatedVelocity.z);
            }
        }
        else
        {
            inputVectorUnSmoothed = Vector2.zero;
            joyStickLeftInput = Vector2.zero;
            reference = Vector2.zero;
            inputMovement = Vector3.zero;
            velocity = Vector3.zero;
        }
    }

    public void JumpButton(InputAction.CallbackContext context) 
    {
        return;
    }

    public void JoystickLeft(InputAction.CallbackContext context)
    {
        if (!alive) return;

        joyStickLeftInput = context.ReadValue<Vector2>();
    }

    public void JoystickRight(InputAction.CallbackContext context)
    {
        inputVectorUnSmoothed = context.ReadValue<Vector2>();
    }

    public Vector2 GetRightStickVector()
    {
        return joyStickRightInput;
    }


    private void ApplyJoystickMovement()
    {

        inputMovement.x = joyStickLeftInput.x;
        inputMovement.y = 0.0f;
        inputMovement.z = joyStickLeftInput.y;

        if (inputMovement.magnitude > 1.0f) inputMovement.Normalize();

        inputMovement = InputToCameraProjection(inputMovement);
        inputMovement *= acceleration * Time.deltaTime;
    }

    private void AimDirection()
    {
        transform.LookAt(transform.position + aimingDirection);
    }

    private Vector3 InputToCameraProjection(Vector3 input)
    {

        if (myCamera == null)
            return input;

        Vector3 cameraRotation = myCamera.transform.rotation.eulerAngles;
        //cameraRotation.x = Mathf.Min(cameraRotation.x, _planeNormal.y);
        input = Quaternion.Euler(cameraRotation) * input;
        return Vector3.ProjectOnPlane(input, planeNormal).normalized;
    }

    public void Accelerate(Vector3 input)
    {
        velocity += input *
                     ((Vector3.Dot(input, velocity) < 0.0f ? turnSpeedModifier : 1.0f) *
                      acceleration);
        velocity = Vector3.ClampMagnitude(velocity, movementSpeedUpgraded ? upgradedTerminalVelocity : terminalVelocity);
    }

    public void Decelerate()
    {

        Vector3 projection = Vector3.ProjectOnPlane(velocity, Vector3.up);
        if (deceleration * Time.deltaTime > projection.magnitude)
        {
            velocity.x = 0.0f;
            velocity.z = 0.0f;
        }
        else
        {
            velocity -= projection * (deceleration * Time.deltaTime);
        }
    }

    public void ApplyAirFriction() => velocity *= Mathf.Pow(1.0f - airResistanceCoefficient, Time.deltaTime);

    public void UpdateVelocity()
    {

        if (velocity.magnitude < float.Epsilon)
        {
            velocity = Vector3.zero;
            return;
        }

        RaycastHit hit;
        int iterations = 0;
        do
        {
            hit = CapsuleCasts(velocity);

            if (!hit.collider)
                continue;

            float skinWidth = this.skinWidth / Vector3.Dot(velocity.normalized, hit.normal);
            float distanceToSkinWidth = hit.distance + skinWidth;

            if (distanceToSkinWidth > velocity.magnitude * Time.deltaTime)
                return;

            if (distanceToSkinWidth > 0.0f)
                transform.position += distanceToSkinWidth * velocity.normalized;

            Vector3 normalForce = Normal.Force(velocity, hit.normal);
            velocity += normalForce;
            ApplyFriction(normalForce);

        } while (hit.collider && iterations++ < 10);

        if (iterations > 9)
            Debug.Log("UpdateVelocity " + iterations);
    }

    public void ResolveOverlap()
    {
        int exit = 0;
        int count = Physics.OverlapCapsuleNonAlloc(
            transform.position + point1,
            transform.position + point2,
            capsuleCollider.radius,
            overlapCollidersNonAlloc,
            collisionMask);

        while (count > 0 && exit++ < 10)
        {
            for (int i = 0; i < count; i++)
            {
                if (Physics.ComputePenetration(
                        capsuleCollider,
                        capsuleCollider.transform.position,
                        capsuleCollider.transform.rotation,
                        overlapCollidersNonAlloc[i],
                        overlapCollidersNonAlloc[i].gameObject.transform.position,
                        overlapCollidersNonAlloc[i].gameObject.transform.rotation,
                        out var direction,
                        out var distance))
                {

                    Vector3 separationVector = direction * distance;
                    transform.position += separationVector + separationVector.normalized * skinWidth;
                    velocity += Normal.Force(velocity, direction);
                }
            }

            count = Physics.OverlapCapsuleNonAlloc(
                transform.position + point1,
                transform.position + point2,
                capsuleCollider.radius,
                overlapCollidersNonAlloc,
                collisionMask);
            exit++;
        }
    }

    private void ApplyFriction(Vector3 normalForce)
    {

        if (velocity.magnitude < normalForce.magnitude * kineticFrictionCoefficient)
        {
            velocity = Vector3.zero;
        }
        else
        {
            velocity -= kineticFrictionCoefficient * normalForce.magnitude * velocity.normalized;
        }
    }

    public bool Grounded()
    {
        grounded = Physics.SphereCast(transform.position + point2, colliderRadius, Vector3.down,
                        out var hit, groundCheckDistance + skinWidth, collisionMask);

        planeNormal = false ? hit.normal : Vector3.up;

        return grounded;
    }

    private RaycastHit CapsuleCasts(Vector3 direction)
    {
        Physics.CapsuleCast(transform.position + point1,
            transform.position + point2,
            colliderRadius,
            direction,
            out var hit,
            float.PositiveInfinity,
            collisionMask);
        return hit;
    }

    private void OnDrawGizmos()
    {
        debugCollider = transform.position + velocity * Time.deltaTime + Vector3.up * .5f;

        Gizmos.color = DebugColor;

        Gizmos.DrawWireSphere(debugCollider, .5f);

        Gizmos.DrawLine(debugCollider + Vector3.back * .5f, debugCollider + Vector3.up + Vector3.back * .5f);

        Gizmos.DrawLine(debugCollider + Vector3.forward * .5f, debugCollider + Vector3.up + Vector3.forward * .5f);

        Gizmos.DrawLine(debugCollider + Vector3.right * .5f, debugCollider + Vector3.up + Vector3.right * .5f);

        Gizmos.DrawLine(debugCollider + Vector3.left * .5f, debugCollider + Vector3.up + Vector3.left * .5f);

        Gizmos.DrawWireSphere(debugCollider + Vector3.up, .5f);
    }

    public Vector3 Velocity()
    {
        return velocity;
    }

    public void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    public void AddVelocity(Vector3 velocity)
    {
        this.velocity += velocity;
    }

    public bool IsGrounded()
    {
        return grounded;
    }
    
    public Vector2 GetRightJoystickInput() { return joyStickRightInput; }

    private bool dying = false;
    public void Die()
    {
        if (alive && !dying)
        {
            dying = true;
            alive = false;
            StartCoroutine(ReturnToOtherPlayer());
        }
    }

    private IEnumerator ReturnToOtherPlayer()
    {
        float t = 0.0f;
        MovementSpeedReduction(false);
        Quaternion startRot = Quaternion.Euler(0, 150, 0);
        Quaternion endRot = Quaternion.Euler(-90, 0, 0) * startRot;
        transform.position += Vector3.up * .5f;
        while (t <= 1.0f)
        {
            transform.rotation = Quaternion.Slerp(startRot, endRot, Ease.EaseOutCirc(t));
            t += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);
        visuals.SetActive(false);
        transform.rotation = startRot;

        t = 0.0f;
        Vector3 startPos = gameObject.transform.position;
        while (t <= 1.0f)
        {
            gameObject.transform.position = Vector3.LerpUnclamped(startPos, otherPlayer.transform.position + Vector3.left, Ease.EaseOutCirc(t));
            t += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(2);
        visuals.SetActive(true);
        alive = true;
        dying = false;
    }

    public void SetTerminalVelocity(float value) => terminalVelocity = value;
    public float GetTerminalVelocity() { return terminalVelocity; }
    public void SetDefaultMovementSpeed() => movementSpeedUpgraded = false;
    public void Respawn() => alive = true;

    [HideInInspector] public float movementSpeedReduced = 1f;
    public void MovementSpeedReduction(bool slow)
    {
        if (slow)
        {
            movementSpeedReduced = 0.5f;
        }
        else
        {
            movementSpeedReduced = 1f;
        }
    }
    public void MovementSpeedUpgrade() => movementSpeedUpgraded = true;

    public bool MovementSpeedUpgraded
    {
        get { return movementSpeedUpgraded; }
        set { movementSpeedUpgraded = value; }
    }
}