using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Create CameraState/CameraSplitScreenState")]
public class SplitScreenState : CameraState {
	
	[SerializeField] 
	private LayerMask _collisionMask;
	
	[SerializeField] [Range(1.0f, 10.0f)]
	private float mouseSensitivityX = 10.0f;
	
	[SerializeField] [Range(1.0f, 10.0f)]
	private float mouseSensitivityY = 1.0f;

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
    
	[SerializeField] [Range(0.0f, 1.0f)] 
	private float _smoothCameraPosTime = 0.105f;
    
	[SerializeField] [Range(-5.0f, 5.0f)]
	private float splitScreenOffsetFactor = 2.0f;
    
	[SerializeField]
	private float smoothDampMinVal = 0.05f;
	
	[SerializeField]
	private float smoothDampMaxVal = 0.25f;
	
	[SerializeField] [Range(1.0f, 20.0f)] 
	private float isometricDistance = 8.0f;
    
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
	private float _cameraCollisionRadius = .5f;
	private float _smoothDollyTime;
	private float splitScreenWidth = -.497f;

	private void Awake() {
		_fpsCameraPos = Vector3.up * headHeight;
	}
	
	public override void Enter() {
		_lerpOffset = _camera3rdPersonOffset;
		_cameraPos = thisTransform.position;
		Vector3 initialCameraRotation = CameraTransform.rotation.eulerAngles;
		mouseMovement.x = initialCameraRotation.y;
		mouseMovement.y = initialCameraRotation.x;
		depthMaskPlanePos = DepthMaskPlane.localPosition;
		depthMaskPlanePos.x = splitScreenWidth;
		DepthMaskPlane.localPosition = depthMaskPlanePos;
		
		// split rotation is kept vertical on the screen
		Vector3 screenAngle = Vector3.ProjectOnPlane(isRightMostPlayer ? Vector3.up : Vector3.down, CameraTransform.transform.forward);
		DepthMaskHolder.rotation = Quaternion.LookRotation(CameraTransform.forward, screenAngle);
	}

	public override void Run() {
		Input();
		
		// both cameras are free
		CameraTransform.rotation = Quaternion.Euler(mouseMovement.y, mouseMovement.x, 0.0f);
		
		if (firstPerson) {
			CameraTransform.localPosition = _fpsCameraPos;
			return;
		}
		
		_cameraPos = Vector3.SmoothDamp(_cameraPos, thisTransform.position, 
			ref _smoothDampCurrentVelocityLateral, _smoothCameraPosTime);

		_abovePlayer = _cameraPos + Vector3.up * headHeight;
		_offsetTarget = _abovePlayer + CameraTransform.rotation * _camera3rdPersonOffset;
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

		_smoothDollyTime = _hit.collider ? smoothDampMinVal : smoothDampMaxVal;
		_lerpOffset = Vector3.SmoothDamp(_lerpOffset, _offset, ref _smoothDampCurrentVelocity, _smoothDollyTime);

		splitScreenOffset = Vector3.ProjectOnPlane( 
			isRightMostPlayer
				? -CameraTransform.transform.right * splitScreenOffsetFactor
				: CameraTransform.transform.right * splitScreenOffsetFactor, 
			Vector3.up);
		
		CameraTransform.position = splitScreenOffset + _abovePlayer + CameraTransform.rotation * _lerpOffset;
		
		if (Vector3.Distance(PlayerThis.position, PlayerOther.position) < isometricDistance)
			stateMachine.TransitionTo<TransitionToIsometric>();
	}

	public override void Exit() {
		
	}
	
	private void Input() {
		
		mouseMovement.x += PlayerController.GetRightStickVector().x * rightStickSpeedX * Time.deltaTime;// TODO UnityEngine.Input.GetAxisRaw(MouseX) * mouseSensitivityX;
		mouseMovement.y -= PlayerController.GetRightStickVector().y * rightStickSpeedY * Time.deltaTime;// TODO UnityEngine.Input.GetAxisRaw(MouseY) * mouseSensitivityY;
		mouseMovement.y = Mathf.Clamp(mouseMovement.y, 1 - lookOffset , 179 - lookOffset);

		// mouseMovement.y = Mathf.Clamp(mouseMovement.y, clampLookupMax - LookOffset, clampLookupMin - LookOffset);
	}
}
