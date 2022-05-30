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
    //cc animation
    public Animator anim;


    private Collider[] _OverlapCollidersNonAlloc = new Collider[10];
    private GameObject[] players;
    private GameObject otherPlayer;
    private CapsuleCollider _collider;
    private StateMachine stateMachine;
    private Transform _camera;
    private Vector3 aimingDirection = Vector3.forward;
    private Vector3 _debugCollider;
    private Vector3 _planeNormal;
    private Vector3 _point1;
    private Vector3 _point2;
    private Vector2 joyStickLeftInput;
    private Vector2 joyStickRightInput;
    private bool _grounded;
    private float _colliderRadius;

    protected bool alive = true;

    [HideInInspector] public Vector3 _velocity;
    [HideInInspector] public Vector3 _jumpVector;
    [HideInInspector] public Vector3 _inputMovement;
    [HideInInspector] public float airControl = 1.0f;
    [HideInInspector] public bool _jumped;
    public Color _debugColor = new Color(10, 20, 30);
    public bool _pressedJump;
    public bool _releasedJump;

    [SerializeField] public List<State> states;

    [FormerlySerializedAs("_gravity")]
    [SerializeField]
    [Range(0.0f, 20.0f)]
    [Tooltip("Set default gravity in:\nEdit > Project Settings > Physics > Gravity")]
    public float _defaultGravity;

    [SerializeField] [Range(0.0f, 4.0f)] public float jumpFallVelocityMultiplier = 2.0f;

    [Space(10)]
    [Header("Character Design")]
    [SerializeField]
    [Range(0.0f, 15.0f)]
    private float _acceleration = 3.0f;

    [SerializeField]
    [Range(0.0f, 10.0f)]
    [Tooltip("The deceleration when no input")]
    private float _deceleration = 1.5f;

    [SerializeField]
    [Range(0.0f, 10.0f)]
    [Tooltip("Extra force when character is turning the opposite way")]
    private float _turnSpeedModifier = 2.0f;

    [SerializeField]
    [Range(0.0f, 20.0f)]
    [Tooltip("Max speed")]
    private float _terminalVelocity = 12.0f;
    private float _defaultTerminalVelocity;

    [SerializeField]
    [Range(0.0f, 20.0f)]
    [Tooltip("Set before hitting [\u25BA]\nOnly changed during start")]
    public float _jumpForce = 10.0f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    [Tooltip("Force to overcome friction from a standstill")]
    private float _staticFrictionCoefficient = 0.5f;    // TODO check where this should be used! To check when the velocity is greater than what static friction? 

    [SerializeField]
    [Range(0.0f, 1.0f)]
    [Tooltip("Force applied when moving\n(60-70% of static friction usually)")]
    private float _kineticFrictionCoefficient = 0.2f;   // TODO rename to _dynamicFrictionCoefficient ? 

    [SerializeField]
    [Range(0.0f, 1.0f)]
    [Tooltip("Force affecting velocity")]
    private float _airResistanceCoefficient = .5f;

    [Space(10)]
    [Header("Character Controller Implementation Details")]
    [SerializeField]
    [Tooltip("What LayerMask(s) the character should collide with")]
    private LayerMask _collisionMask;

    [SerializeField]
    [Range(0.0f, 0.15f)]
    [Tooltip("The distance the character should stop before a collider")]
    private float _skinWidth = 0.1f;

    [SerializeField]
    [Range(0.0f, 0.2f)]
    [Tooltip("The distance the character should count as being grounded")]
    private float _groundCheckDistance = 0.15f;

    [SerializeField] private GameObject visuals;

    private Vector2 reference;
    private Vector2 inputVectorUnsmoothed;

    private void Awake()
    {
        stateMachine = new StateMachine(this, states);
        _collider = GetComponent<CapsuleCollider>();
        _camera = GetComponentInChildren<Camera>().transform;
    }

    private void Start()
    {
        _defaultTerminalVelocity = _terminalVelocity;
        _jumpVector = new Vector3(0.0f, _jumpForce);
        _defaultGravity = -Physics.gravity.y;
        _colliderRadius = _collider.radius;
        _point1 = _collider.center + Vector3.up * (_collider.height / 2 - _colliderRadius);
        _point2 = _collider.center + Vector3.down * (_collider.height / 2 - _colliderRadius);
        players = GameObject.FindGameObjectsWithTag("Player");
        otherPlayer = players[0] != gameObject ? players[0] : players[1];
    }



    private void Update()
    {
        joyStickRightInput = Vector2.SmoothDamp(joyStickRightInput, inputVectorUnsmoothed, ref reference, .05f, 100.0f);

        if (alive)
        {
            Grounded();
            ApplyJoystickMovement();
            stateMachine.Run();

            if (anim != null)
            {
                Vector3 rot = transform.rotation.eulerAngles;
                Vector3 rotatedVelocity = Quaternion.Euler(rot.x, -rot.y, rot.z) * _velocity;

                anim.SetFloat("Speed", rotatedVelocity.x);
                anim.SetFloat("Direction", rotatedVelocity.z);
            }
        }
        else
        {
            inputVectorUnsmoothed = Vector2.zero;
            joyStickLeftInput = Vector2.zero;
            reference = Vector2.zero;
            _inputMovement = Vector3.zero;
            _velocity = Vector3.zero;
        }
    }

    public void JumpButton(InputAction.CallbackContext context)
    {

        if (context.started && _grounded)
            _jumped = true;
        else
            _pressedJump = false;

        if (context.canceled)
            _releasedJump = true;
    }

    public void JoystickLeft(InputAction.CallbackContext context)
    {
        if (!alive) return;

        joyStickLeftInput = context.ReadValue<Vector2>();
    }

    public void JoystickRight(InputAction.CallbackContext context)
    {
        inputVectorUnsmoothed = context.ReadValue<Vector2>();
    }

    public Vector2 GetRightStickVector()
    {
        return joyStickRightInput;
    }


    private void ApplyJoystickMovement()
    {

        _inputMovement.x = joyStickLeftInput.x;
        _inputMovement.y = 0.0f;
        _inputMovement.z = joyStickLeftInput.y;

        if (_inputMovement.magnitude > 1.0f) _inputMovement.Normalize();

        _inputMovement = InputToCameraProjection(_inputMovement);
        _inputMovement *= _acceleration * Time.deltaTime;
    }

    private void AimDirection()
    {
        transform.LookAt(transform.position + aimingDirection);
    }

    private Vector3 InputToCameraProjection(Vector3 input)
    {

        if (_camera == null)
            return input;

        Vector3 cameraRotation = _camera.transform.rotation.eulerAngles;
        cameraRotation.x = Mathf.Min(cameraRotation.x, _planeNormal.y);
        input = Quaternion.Euler(cameraRotation) * input;
        return Vector3.ProjectOnPlane(input, _planeNormal).normalized;
    }

    public void Accelerate(Vector3 input)
    {
        _velocity += input *
                     ((Vector3.Dot(input, _velocity) < 0.0f ? _turnSpeedModifier : 1.0f) *
                      _acceleration);
        _velocity = Vector3.ClampMagnitude(_velocity, _terminalVelocity);
    }

    public void Decelerate()
    {

        Vector3 projection = Vector3.ProjectOnPlane(_velocity, Vector3.up);
        if (_deceleration * Time.deltaTime > projection.magnitude)
        {
            _velocity.x = 0.0f;
            _velocity.z = 0.0f;
        }
        else
        {
            _velocity -= projection * (_deceleration * Time.deltaTime);
        }
    }

    public void ApplyAirFriction() => _velocity *= Mathf.Pow(1.0f - _airResistanceCoefficient, Time.deltaTime);

    public void UpdateVelocity()
    {

        if (_velocity.magnitude < float.Epsilon)
        {
            _velocity = Vector3.zero;
            return;
        }

        RaycastHit hit;
        int iterations = 0;
        do
        {
            hit = CapsuleCasts(_velocity);

            if (!hit.collider)
                continue;

            float skinWidth = _skinWidth / Vector3.Dot(_velocity.normalized, hit.normal);
            float distanceToSkinWidth = hit.distance + skinWidth;

            if (distanceToSkinWidth > _velocity.magnitude * Time.deltaTime)
                return;

            if (distanceToSkinWidth > 0.0f)
                transform.position += distanceToSkinWidth * _velocity.normalized;

            Vector3 normalForce = Normal.Force(_velocity, hit.normal);
            _velocity += normalForce;
            ApplyFriction(normalForce);

        } while (hit.collider && iterations++ < 10);

        if (iterations > 9)
            Debug.Log("UpdateVelocity " + iterations);
    }

    public void ResolveOverlap()
    {
        int exit = 0;
        int count = Physics.OverlapCapsuleNonAlloc(
            transform.position + _point1,
            transform.position + _point2,
            _collider.radius,
            _OverlapCollidersNonAlloc,
            _collisionMask);

        while (count > 0 && exit++ < 10)
        {
            for (int i = 0; i < count; i++)
            {
                if (Physics.ComputePenetration(
                        _collider,
                        _collider.transform.position,
                        _collider.transform.rotation,
                        _OverlapCollidersNonAlloc[i],
                        _OverlapCollidersNonAlloc[i].gameObject.transform.position,
                        _OverlapCollidersNonAlloc[i].gameObject.transform.rotation,
                        out var direction,
                        out var distance))
                {

                    Vector3 separationVector = direction * distance;
                    transform.position += separationVector + separationVector.normalized * _skinWidth;
                    _velocity += Normal.Force(_velocity, direction);
                }
            }

            count = Physics.OverlapCapsuleNonAlloc(
                transform.position + _point1,
                transform.position + _point2,
                _collider.radius,
                _OverlapCollidersNonAlloc,
                _collisionMask);
            exit++;
        }
    }

    private void ApplyFriction(Vector3 normalForce)
    {

        if (_velocity.magnitude < normalForce.magnitude * _kineticFrictionCoefficient)
        {
            _velocity = Vector3.zero;
        }
        else
        {
            _velocity -= _kineticFrictionCoefficient * normalForce.magnitude * _velocity.normalized;
        }
    }

    public bool Grounded()
    {
        _grounded = Physics.SphereCast(transform.position + _point2, _colliderRadius, Vector3.down,
                        out var hit, _groundCheckDistance + _skinWidth, _collisionMask);

        _planeNormal = _grounded ? hit.normal : Vector3.up;

        return _grounded;
    }

    private RaycastHit CapsuleCasts(Vector3 direction)
    {
        Physics.CapsuleCast(transform.position + _point1,
            transform.position + _point2,
            _colliderRadius,
            direction,
            out var hit,
            float.PositiveInfinity,
            _collisionMask);
        return hit;
    }

    private void OnDrawGizmos()
    {
        _debugCollider = transform.position + _velocity * Time.deltaTime + Vector3.up * .5f;

        Gizmos.color = _debugColor;

        Gizmos.DrawWireSphere(_debugCollider, .5f);

        Gizmos.DrawLine(_debugCollider + Vector3.back * .5f, _debugCollider + Vector3.up + Vector3.back * .5f);

        Gizmos.DrawLine(_debugCollider + Vector3.forward * .5f, _debugCollider + Vector3.up + Vector3.forward * .5f);

        Gizmos.DrawLine(_debugCollider + Vector3.right * .5f, _debugCollider + Vector3.up + Vector3.right * .5f);

        Gizmos.DrawLine(_debugCollider + Vector3.left * .5f, _debugCollider + Vector3.up + Vector3.left * .5f);

        Gizmos.DrawWireSphere(_debugCollider + Vector3.up, .5f);
    }

    public Vector3 Velocity()
    {
        return _velocity;
    }

    public void SetVelocity(Vector3 velocity)
    {
        _velocity = velocity;
    }

    public void AddVelocity(Vector3 velocity)
    {
        _velocity += velocity;
    }

    public bool IsGrounded()
    {
        return _grounded;
    }

    public bool ReleasedJump() => _releasedJump;
    public bool PressedJump() => _pressedJump;
    public Vector2 GetRightJoystickInput() { return joyStickRightInput; }

    public void Die()
    {
        if (alive)
            StartCoroutine(ReturnToOtherPlayer());
        alive = false;
    }

    private IEnumerator ReturnToOtherPlayer()
    {
        float t = 0.0f;
        MovementSpeedReduction(false);
        Quaternion startRot = transform.rotation;
        Quaternion endRot = quaternion.Euler(-90, 0, 0) * startRot;
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
    }

    public void SetTerminalVelocity(float value) => _terminalVelocity = value;
    public float GetTerminalVelocity() { return _terminalVelocity; }
    public void SetDefaultVelocity() => _terminalVelocity = _defaultTerminalVelocity;
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
}