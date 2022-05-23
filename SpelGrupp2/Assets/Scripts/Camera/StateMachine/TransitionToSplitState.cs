using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create CameraState/TransitionToSplitState")]
public class TransitionToSplitState : CameraBaseState {

	[SerializeField] 
	private LayerMask _collisionMask;
	
	// [SerializeField] [Range(1.0f, 10.0f)]
	// private float mouseSensitivityX = 10.0f;
	//
	// [SerializeField] [Range(1.0f, 10.0f)]
	// private float mouseSensitivityY = 5.0f;

	[SerializeField] [Range(1.0f, 150.0f)] 
	private float rightStickSpeedX = 100.0f;
	
	[SerializeField] [Range(1.0f, 150.0f)] 
	private float rightStickSpeedY = 85.0f;

	[SerializeField] [Range(0.0f, 2.0f)]
	private float headHeight = 1.6f;

	[SerializeField]
	private bool firstPerson; 
    
	[SerializeField]                      
	private bool isRightMostPlayer;    
    
	[SerializeField] 
	private Vector3 _camera3rdPersonOffset;
    
	// [SerializeField] [Range(0.0f, 1.0f)] 
	// private float _smoothCameraPosTime = 0.105f;
    
	[SerializeField] [Range(-5.0f, 5.0f)]
	private float splitScreenOffsetFactor = 2.0f;
    
	[SerializeField]
	private float smoothDampMinVal = 0.05f;
	
	[SerializeField]
	private float smoothDampMaxVal = 0.25f;

	private RaycastHit _hit;
	private Vector3 _fpsCameraPos;
	private Vector3 _cameraPos;
	private Vector3 _abovePlayer;
	private Vector3 _smoothDampCurrentVelocityLateral;
	private Vector3 _offsetTarget;
	private Vector3 _offsetDirection;
	private Vector3 _offset;
	private Vector3 _lerpOffset;
	private Vector3 splitScreenOffset;
	private Vector3 _smoothDampCurrentVelocity;
	private Vector3 depthMaskPlanePos;
	// private float _cameraCollisionRadius = .5f;
	private float _smoothDollyTime;
	private float splitScreenWidth = -.497f;
	private readonly Quaternion _ninetyDegrees = Quaternion.Euler(0.0f, 90.0f, 0.0f);
	private float percentage = 0.0f;

	private void Awake() {
		_fpsCameraPos = Vector3.up * headHeight;
	}
	
	public override void Enter() {
		percentage = 0.0f;
		Vector3 initialCameraRotation = CameraTransform.rotation.eulerAngles;
		mouseMovement.x = initialCameraRotation.y;
		mouseMovement.y = initialCameraRotation.x;
		depthMaskPlanePos = DepthMaskPlane.localPosition;
		depthMaskPlanePos.x = splitScreenWidth;
		DepthMaskPlane.localPosition = depthMaskPlanePos;
		_cameraPos = CameraTransform.position;
	}

	public override void Run() {
		Input();
		
		CameraTransform.rotation = Quaternion.Euler(mouseMovement.y, mouseMovement.x, 0.0f);
		
		if (firstPerson) {
			CameraTransform.localPosition = _fpsCameraPos;
			return;
		}
	    
		percentage += Time.deltaTime;

		// split rotation is Linearly interpolated to vertical
		Vector3 angle = (PlayerOther.position - PlayerThis.position).normalized;
		Vector3 quarterAngle = _ninetyDegrees * angle;
		Vector3 screenAngle = Vector3.ProjectOnPlane(quarterAngle, -CameraTransform.transform.forward);
		Vector3 verticalAngle = Vector3.ProjectOnPlane(isRightMostPlayer ? Vector3.up : Vector3.down, CameraTransform.forward);
		DepthMaskHolder.rotation = 
				Quaternion.Slerp(
					Quaternion.LookRotation(CameraTransform.forward, screenAngle), 
					Quaternion.LookRotation(CameraTransform.forward, verticalAngle), 
					percentage * 3.0f);
		
		_cameraPos = Vector3.Lerp(_cameraPos, thisTransform.position, percentage);

		_abovePlayer = _cameraPos + Vector3.up * headHeight;
		_offsetTarget = _abovePlayer + CameraTransform.rotation * _camera3rdPersonOffset;
		_offsetDirection = _offsetTarget - _abovePlayer;

		// Physics.SphereCast(_abovePlayer, 
		// 	_cameraCollisionRadius, 
		// 	_offsetDirection.normalized, 
		// 	out _hit, 
		// 	_offsetDirection.magnitude, 
		// 	_collisionMask);
		
		if (_hit.collider)
			_offset = _camera3rdPersonOffset.normalized * _hit.distance;
		else
			_offset = _camera3rdPersonOffset;

		_smoothDollyTime = _hit.collider ? smoothDampMinVal : smoothDampMaxVal;
		_lerpOffset = Vector3.Lerp(_lerpOffset, _offset, percentage);

		splitScreenOffset = Vector3.ProjectOnPlane( 
			isRightMostPlayer
				? -CameraTransform.transform.right * splitScreenOffsetFactor
				: CameraTransform.transform.right * splitScreenOffsetFactor, 
			Vector3.up);
		
		CameraTransform.position = splitScreenOffset + _abovePlayer + CameraTransform.rotation * _lerpOffset;

		if (percentage > 1.0f)
			stateMachine.TransitionTo<SplitScreenState>();
	}

	public override void Exit() {
		
	}
	
	private void Input() {
		
		mouseMovement.x += PlayerController.GetRightStickVector().x * rightStickSpeedX * Time.deltaTime;// TODO UnityEngine.Input.GetAxisRaw(MouseX) * mouseSensitivityX;
		mouseMovement.y -= PlayerController.GetRightStickVector().y * rightStickSpeedY * Time.deltaTime;// TODO UnityEngine.Input.GetAxisRaw(MouseY) * mouseSensitivityY;
		mouseMovement.y = Mathf.Clamp(mouseMovement.y, 1 - lookOffset , 179 - lookOffset);

		// mouseMovement.z = Mathf.Clamp(mouseMovement.z, clampLookupMax - LookOffset, clampLookupMin - LookOffset);
	}
}
