using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{

    public bool _pressedJump;
    public bool _releasedJump;

    private Collider[] _OverlapCollidersNonAlloc = new Collider[10];
    private CapsuleCollider _collider;
    private Transform _camera;
    private Vector3 _planeNormal;
    [HideInInspector]
    public Vector3 _velocity;   // TODO FIXME HACK statemachine
    [HideInInspector]
    public Vector3 _jumpVector; // TODO FIXME HACK statemachine
    [HideInInspector]
    public Vector3 _inputMovement;  // TODO FIXME HACK statemachine
    private Vector3 _point1;
    private Vector3 _point2;
    [HideInInspector]
    public bool _jumped;    // TODO FIXME HACK statemachine
    private bool _grounded;
    private float _colliderRadius;
    [HideInInspector]
    public float airControl = 1.0f; // TODO ? used here?

    [FormerlySerializedAs("_gravity")]
    [SerializeField]
    [Range(0.0f, 20.0f)]
    [Tooltip("Set default gravity in:\nEdit > Project Settings > Physics > Gravity")]
    public float _defaultGravity;

    [SerializeField]
    [Range(0.0f, 4.0f)]
    public float jumpFallVelocityMultiplier = 2.0f;

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

    private StateMachine stateMachine;  
    [SerializeField]
    public List<State> states;  

    private Vector2 joyStickLeftInput;
    private Vector2 joyStickRightInput;
    private Vector3 aimingDirection = Vector3.forward;

    protected bool alive = true;
    public Crafting crafting;

    private GameObject[] players;
    private GameObject otherPlayer;

    //[SerializeField] private Camera cam;

    private void Awake()
    {
        stateMachine = new StateMachine(this, states); 
        _collider = GetComponent<CapsuleCollider>();
        _camera = GetComponentInChildren<Camera>().transform;
    }

    private void Start()
    {
        crafting = new Crafting();
        _jumpVector = new Vector3(0.0f, _jumpForce);
        _defaultGravity = -Physics.gravity.y;
        _colliderRadius = _collider.radius;
        _point1 = _collider.center + Vector3.up * (_collider.height / 2 - _colliderRadius);
        _point2 = _collider.center + Vector3.down * (_collider.height / 2 - _colliderRadius);
        players = GameObject.FindGameObjectsWithTag("Player");
        otherPlayer = players[0] != gameObject ? players[0] : players[1];
    }

    public Vector2 GetRightJoystickInput() { return joyStickRightInput; }

    public void Die()
    {
        gameObject.transform.position = otherPlayer.transform.position + Vector3.left;
        alive = false;
    }
    public void Respawn() => alive = true;

    private void Update()
    {
        if (alive)
        {
            Grounded();
            ApplyJoystickMovement();
            AimDirection();
            stateMachine.Run(); 
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
        joyStickRightInput = context.ReadValue<Vector2>();
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

        // normalize input for keyboard controls
        // check is needed so gamepad joystick doesn't get normalized to magnitude of 1
        if (_inputMovement.magnitude > 1.0f) _inputMovement.Normalize();
        _inputMovement = InputToCameraProjection(_inputMovement);
        _inputMovement *= _acceleration * Time.deltaTime;
    }
    /*  --PlayerAttack--
	private void ApplyJoystickFireDirection() {
		
		if (joyStickRightInput.magnitude > 0.1f) {
			aimingDirection.x = joyStickRightInput.x;
			aimingDirection.z = joyStickRightInput.y;
			aimingDirection.Normalize();
		}
	}
    */
    private void AimDirection()
    {
        transform.LookAt(transform.position + aimingDirection);
    }
    /*
	[SerializeField] private float laserDrainPerShot = .2f;
	public void Fire(InputAction.CallbackContext context) {
		if (!alive) return;	// TODO game jam code! 
			
		if (context.started)
		{
			AudioController.instance.TriggerTest();

			ShootLaser();
			StartCoroutine(AnimateLineRenderer(aimingDirection));
		}
	}

     --PlayerHealth--
	[SerializeField] 
	private BatteryUI health;
	
	private bool alive = true;
	[SerializeField] 
	private GameObject visuals;
    public void TakeDamage()
    {
	    Debug.Log("Took damage");
        health.TakeDamage();
    }

        --PlayerHealth--
    public void Die() {
		alive = false;
		visuals.SetActive(false);
	}

        --PlayerHealth--
	public void Respawn() {
		alive = true;
		visuals.SetActive(true);
	} */

    /* --PlayerAttack--
	private void ShootLaser() {
		
		health.LaserBatteryDrain();

		Physics.Raycast(transform.position + transform.forward + Vector3.up, transform.forward, out RaycastHit hitInfo, 30.0f, enemyLayerMask);
		//Debug.Log(hitInfo.collider.transform.name);
		if (hitInfo.collider != null) {
			EnemyHealth enemy = hitInfo.transform.GetComponent<EnemyHealth>();
			enemy.TakeDamage();
			Debug.Log(String.Format("Hit {0}", hitInfo.transform.name));
		}
	}
	    --PlayerAttack--
	private IEnumerator AnimateLineRenderer(Vector3 direction) {
		Vector3[] positions = {transform.position + Vector3.up, transform.position + Vector3.up + direction * 30.0f};
		lineRenderer.SetPositions(positions);
		float t = 0.0f;
		while (t < 1.0f) {
			float e = Mathf.Lerp(Ease.EaseOutQuint(t), Ease.EaseOutBounce(t), t);
			float lineWidth = Mathf.Lerp(.5f, .0f, e);
			lineRenderer.startWidth = lineWidth;
			lineRenderer.endWidth = lineWidth;
			Color color = Color.Lerp(Color.white, Color.red, Ease.EaseInQuart(t));
			lineRenderer.startColor = color;
			lineRenderer.endColor = color;
			t += Time.deltaTime * 3.0f;
			yield return null;
		}

		lineRenderer.startWidth = 0.0f;
		lineRenderer.endWidth = 0.0f;
	}
        --PlayerAttack--
	private void AnimateLaserSightLineRenderer(Vector3 dir)
	{
        Vector3[] positions = { transform.position + Vector3.up, transform.position + Vector3.up + dir * 30.0f };
        aimLineRenderer.SetPositions(positions);
        float lineWidth = 0.05f;
        aimLineRenderer.startWidth = lineWidth;
        aimLineRenderer.endWidth = lineWidth;
        Color color = new Color(1f, 0.2f, 0.2f);
        aimLineRenderer.startColor = color;
        aimLineRenderer.endColor = color;
    }
        --PlayerAttack--
	public void TargetMousePos(InputAction.CallbackContext context) {
		Vector3 mousePos = context.ReadValue<Vector2>();
		mousePos.z = 15.0f;
		Plane plane = new Plane(Vector3.up, transform.position + Vector3.up);
		Ray ray = cam.ScreenPointToRay(mousePos);
		
		if (plane.Raycast(ray, out float enter)) {
			Vector3 hitPoint = ray.GetPoint(enter);
			aimingDirection = hitPoint + Vector3.down - transform.position;
		}
	}
	
    */

    // private void Input() {

    // float right = 0;// TODO UnityEngine.Input.GetAxisRaw(Horizontal);
    // float forward = 0;// TODO UnityEngine.Input.GetAxisRaw(Vertical);
    // _inputMovement.x = right;
    // _inputMovement.y = 0.0f;
    // _inputMovement.z = forward;

    // if (_inputMovement.magnitude > 1.0f) _inputMovement.Normalize();	// if using keyboard
    // _inputMovement = InputToCameraProjection(_inputMovement);
    // _inputMovement *= _acceleration * Time.deltaTime;
    // if (!_pressedJump)
    // 	_pressedJump = false;// TODO UnityEngine.Input.GetButtonDown(Jump);
    // if (!_releasedJump)
    // 	_releasedJump = false; // TODO UnityEngine.Input.GetButtonUp(Jump);

    // if (_pressedJump && _grounded)
    // 	_jumped = true;
    // }

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

        //if (exit > 10) Debug.Log("EXITED");
    }

    private void ApplyFriction(Vector3 normalForce)
    {

        if (_velocity.magnitude < normalForce.magnitude * _kineticFrictionCoefficient)
        {
            _velocity = Vector3.zero;
        }
        else
        {
            _velocity -=
                _kineticFrictionCoefficient * normalForce.magnitude * _velocity.normalized;
        }
    }

    public bool Grounded()
    {
        _grounded =
                Physics.SphereCast(transform.position + _point2, _colliderRadius, Vector3.down,
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

    //???
    private Vector3 _debugCollider;
    public Color _debugColor = new Color(10, 20, 30);
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
}