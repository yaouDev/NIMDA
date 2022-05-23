using System;
using System.Collections.Generic;
using CallbackSystem;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerController))]
public class CameraController : MonoBehaviour {

	private CameraStateMachine stateMachine;
	[SerializeField] 
	public List<CameraState> states;
	
	private const string MouseX = "Mouse X";
	private const string MouseY = "Mouse Y";
	private const float LookOffset = 90;
	
	private RaycastHit _hit;
	private Vector3 _fpsCameraPos;
	private Vector3 _smoothDampCurrentVelocity;
	private Vector3 _smoothDampCurrentVelocityLateral;
	private Vector3 _offset;
	private Vector3 _abovePlayer;
	private Vector3 _offsetTarget;
	private Vector3 _offsetDirection;
	private Vector3 _lerpOffset;
	private Vector3 _cameraPos;
	private Vector2 _mouseMovement;
	private float _smoothDollyTime;
	private PlayerController playerController;

	[HideInInspector]
	public float clampLookupMax;
	[HideInInspector]
	public float clampLookupMin;
	[HideInInspector]
	public float smoothDampMinVal;
	[HideInInspector]
	public float smoothDampMaxVal;
	
	[SerializeField]
	private bool _firstPerson;

	[SerializeField] 
	private LayerMask _collisionMask;
	
	[SerializeField]
	private Transform _camera;
	
	[SerializeField] [Range(1.0f, 10.0f)]
	private float mouseSensitivityX = 1.0f;
	
	// [SerializeField] [Range(1.0f, 10.0f)]
	// private float mouseSensitivityY = 1.0f;
		
	[SerializeField] [Range(0.0f, 2.0f)]
	private float _cameraCollisionRadius;

	[SerializeField] [Range(0.0f, 2.0f)]
	private float _headHeight = 1.6f;

	[SerializeField] 
	private Vector3 _camera3rdPersonOffset;

	[SerializeField] [Range(0.0f, 1.0f)] 
	private float _smoothCameraPosTime = 0.05f;

	[SerializeField] private Transform playerThis;
	[SerializeField] private Transform playerOther;
	[SerializeField] private Transform depthMaskHolder;
	[SerializeField] private Transform depthMaskPlane;

	[SerializeField] public bool isPlayerOne;
	
	private void Awake() {
		
		playerController = GetComponent<PlayerController>();
		isPlayerOne = GetComponent<PlayerHealth>().IsPlayerOne();
		stateMachine = new CameraStateMachine(this, states);
		_fpsCameraPos = _camera.localPosition;
		_cameraPos = transform.position;
	}

	private void Update() {
		Input();
	}

	private void LateUpdate() {
		
		stateMachine.Run();
	}

	private void Input() {

		_mouseMovement.x += playerController.GetRightStickVector().x * mouseSensitivityX * 100;// TODO UnityEngine.Input.GetAxisRaw(MouseX) * mouseSensitivityX;
		_mouseMovement.y -= playerController.GetRightStickVector().y * mouseSensitivityX * 100;// TODO UnityEngine.Input.GetAxisRaw(MouseY) * mouseSensitivityY;
		_mouseMovement.y = Mathf.Clamp(_mouseMovement.y, clampLookupMax - LookOffset, clampLookupMin - LookOffset);
	}

	private bool _debugHit;
	private void MoveCamera() {
		
		_camera.rotation = Quaternion.Euler(_mouseMovement.y, _mouseMovement.x, 0.0f);
		
		if (_firstPerson) {
			_camera.localPosition = _fpsCameraPos;
			return;
		}
		
		_cameraPos = Vector3.SmoothDamp(_cameraPos, transform.position, 
				ref _smoothDampCurrentVelocityLateral, _smoothCameraPosTime);

		_abovePlayer = _cameraPos + Vector3.up * _headHeight;
		_offsetTarget = _abovePlayer + _camera.rotation * _camera3rdPersonOffset;
		_offsetDirection = _offsetTarget - _abovePlayer;

		Physics.SphereCast(_abovePlayer, 
			_cameraCollisionRadius, 
			_offsetDirection.normalized, 
			out _hit, 
			_offsetDirection.magnitude, 
			_collisionMask);
		
		if (_hit.collider)
			_offset = _camera3rdPersonOffset.normalized * _hit.distance;
		else
			_offset = _camera3rdPersonOffset;

		_debugHit = _hit.collider;
		
		_smoothDollyTime = _hit.collider ? smoothDampMinVal : smoothDampMaxVal;
		_lerpOffset = Vector3.SmoothDamp(_lerpOffset, _offset, ref _smoothDampCurrentVelocity, _smoothDollyTime);

		_camera.position = //cameraTarget + 
		                   _abovePlayer + _camera.rotation * _lerpOffset;
	}
	
	[FormerlySerializedAs("cameraTarget")] [SerializeField]
	private Vector3 splitScreenOffset;
	[SerializeField]
	private Vector2 topDownViewRotation;
	[SerializeField] 
	private Vector3 TopDownOffset;
	private Vector3 centroid;
	
	private void CentroidCamera() {

		// both cameras follow the centroid point between the players
		_camera.rotation = Quaternion.Euler(topDownViewRotation.x, topDownViewRotation.y, 0.0f);

		// TODO clamp centroid to magnitude
		
		// both cameras have the same rotation
		
		// the split is rotating freely between the players
		RotateScreenSplit();
		
		centroid = playerThis.position + (playerOther.position - playerThis.position) * .5f;
		Debug.DrawLine(centroid, playerThis.position);
		_cameraPos = centroid + Vector3.up * _headHeight;

		// TODO fade objects in front of camera
		// Physics.SphereCast(_abovePlayer, 
		// 	_cameraCollisionRadius, 
		// 	_offsetDirection.normalized, 
		// 	out _hit, 
		// 	_offsetDirection.magnitude, 
		// 	_collisionMask);

		_offset = TopDownOffset;

		_camera.position = centroid + _abovePlayer + _camera.rotation * _offset;
	}

	[SerializeField]
	private bool isRightMostPlayer;

	[SerializeField] [Range(-5.0f, 5.0f)]
	private float splitScreenOffsetFactor = 2.0f;
	private void SplitCamera() {
		// both cameras are free
		// split rotation is kept vertical on the screen
		
		// offset is set to side of screen the player is on — Set this _once_ in statemachine 
		Vector3 screenAngle = Vector3.ProjectOnPlane(isRightMostPlayer ? Vector3.up : Vector3.down, _camera.transform.forward);
		depthMaskHolder.rotation = Quaternion.LookRotation(_camera.forward, screenAngle);
		
		_camera.rotation = Quaternion.Euler(_mouseMovement.y, _mouseMovement.x, 0.0f);
		
		if (_firstPerson) {
			_camera.localPosition = _fpsCameraPos;
			return;
		}
		
		_cameraPos = Vector3.SmoothDamp(_cameraPos, transform.position, 
			ref _smoothDampCurrentVelocityLateral, _smoothCameraPosTime);

		_abovePlayer = _cameraPos + Vector3.up * _headHeight;
		_offsetTarget = _abovePlayer + _camera.rotation * _camera3rdPersonOffset;
		_offsetDirection = _offsetTarget - _abovePlayer;

		Physics.SphereCast(_abovePlayer, 
			_cameraCollisionRadius, 
			_offsetDirection.normalized, 
			out _hit, 
			_offsetDirection.magnitude, 
			_collisionMask);
		
		if (_hit.collider)
			_offset = _camera3rdPersonOffset.normalized * _hit.distance;
		else
			_offset = _camera3rdPersonOffset;

		_debugHit = _hit.collider;
		
		_smoothDollyTime = _hit.collider ? smoothDampMinVal : smoothDampMaxVal;
		_lerpOffset = Vector3.SmoothDamp(_lerpOffset, _offset, ref _smoothDampCurrentVelocity, _smoothDollyTime);

		splitScreenOffset = Vector3.ProjectOnPlane( 
			isRightMostPlayer
				? -_camera.transform.right * splitScreenOffsetFactor
				: _camera.transform.right * splitScreenOffsetFactor, 
			Vector3.up);
		
		_camera.position = splitScreenOffset + _abovePlayer + _camera.rotation * _lerpOffset;
	}

	private void TransitionToSplit() {
		
	}

	private void TransitionToCentroid() {
		
	}

	private readonly Quaternion _ninetyDegrees = Quaternion.Euler(0.0f, 90.0f, 0.0f);
	private void RotateScreenSplit() {
		Vector3 angle = (playerOther.position - playerThis.position).normalized;
		Vector3 quarterAngle = _ninetyDegrees * angle;
		Vector3 screenAngle = Vector3.ProjectOnPlane(quarterAngle, -_camera.transform.forward);
		depthMaskHolder.rotation = Quaternion.LookRotation(_camera.forward, screenAngle);
	}
	
	private void OnDrawGizmos() {

		Gizmos.color = _debugHit ? Color.white : Color.black;
		
		Gizmos.DrawWireSphere(_camera.position, _cameraCollisionRadius);
	
		Gizmos.color = Color.white;
	
		Gizmos.matrix = Matrix4x4.TRS( 
			_camera.position,
			_camera.rotation, 
			new Vector3(1.0f, 1.0f, 1.0f) );
	
		Gizmos.DrawFrustum(
			Vector3.zero,
			Camera.main.fieldOfView, 
			12.0f, 
			.3f, 
			Camera.main.aspect);
		
	}
	
	private void MoveCameraOld() {
		_camera.rotation = Quaternion.Euler(_mouseMovement.y, _mouseMovement.x, 0.0f);

		if (_firstPerson) {
			_camera.localPosition = _fpsCameraPos;
			return;
		}
		
		_cameraPos = Vector3.SmoothDamp(_cameraPos, transform.position, ref _smoothDampCurrentVelocityLateral, _smoothCameraPosTime);
		
		_abovePlayer = _cameraPos + Vector3.up * _headHeight;
		_offsetTarget = _abovePlayer + _camera.rotation * _camera3rdPersonOffset;
		_offsetDirection = _offsetTarget - _abovePlayer;

		Physics.SphereCast(_abovePlayer, 
			_cameraCollisionRadius, 
			_offsetDirection.normalized, 
			out _hit, 
			_offsetDirection.magnitude, 
			_collisionMask);
		
		if (_hit.collider)
			_offset = _camera3rdPersonOffset.normalized * _hit.distance;
		else
			_offset = _camera3rdPersonOffset;

		_debugHit = _hit.collider;
		
		_smoothDollyTime = _hit.collider ? smoothDampMinVal : smoothDampMaxVal;
		_lerpOffset = Vector3.SmoothDamp(_lerpOffset, _offset, ref _smoothDampCurrentVelocity, _smoothDollyTime);

		_camera.position = _abovePlayer + _camera.rotation * _lerpOffset;
	}

	public Transform GetPlayerThis() {
		return playerThis;
	}

	public Transform GetPlayerOther() {
		return playerOther;
	}

	public Transform GetDepthMaskHolder() {
		return depthMaskHolder;
	}

	public Transform GetCameraTransform() {
		return _camera;
	}

	public Transform GetDepthMaskPlane() {
		return depthMaskPlane;
	}

	public PlayerController GetPlayerController() {
		return playerController;
	}

	public bool IsPlayerOne() => isPlayerOne;
}