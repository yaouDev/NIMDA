using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create CameraState/CameraSplitScreenState")]
public class SplitScreenState : CameraState {
	
	[SerializeField] 
	private LayerMask collisionMask;

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

	private Vector2 topDownViewRotation = new Vector2(55, 45);
	
	private Vector3 topDownOffset = new Vector3(0.0f, 1.0f, -16.0f);
    
	[SerializeField] [Range(0.0f, 1.0f)] 
	private float _smoothCameraPosTime = 0.105f;
	
	private float splitScreenOffsetFactor = 8.0f;
    
	[SerializeField]
	private float smoothDampMinVal = 0.05f;
	
	[SerializeField]
	private float smoothDampMaxVal = 0.25f;
	
	[SerializeField] [Range(1.0f, 20.0f)] 
	private float isometricDistance = 8.0f;
    
	private RaycastHit _hit;
	private Vector3 _cameraPos;
	private Vector3 _abovePlayer;
	private Vector3 _smoothDampCurrentVelocityLateral;
	private Vector3 _offsetTarget;
	private Vector3 _lerpOffset;
	private Vector3 splitScreenOffset;
	private Vector3 _smoothDampCurrentVelocity;
	private Vector3 depthMaskPlanePos;
	private float _cameraCollisionRadius = .5f;
	private float _smoothDollyTime;
	private float splitScreenWidth = -.497f;

	private bool isRightMostPlayer;

	private void Awake() {
	}
	
	public override void Enter() {
		_cameraPos = thisTransform.position;
		Vector3 initialCameraRotation = CameraTransform.rotation.eulerAngles;
		mouseMovement.x = initialCameraRotation.y;
		mouseMovement.y = initialCameraRotation.x;
		depthMaskPlanePos = DepthMaskPlane.localPosition;
		depthMaskPlanePos.x = splitScreenWidth;
		DepthMaskPlane.localPosition = depthMaskPlanePos;
		
		// split rotation is kept vertical on the screen
		isRightMostPlayer = (Quaternion.Euler(0, -45, 0) * (PlayerThis.position - PlayerOther.position)).x > 0;
		Vector3 screenAngle = Vector3.ProjectOnPlane(isRightMostPlayer ? Vector3.up : Vector3.down, CameraTransform.transform.forward);
		DepthMaskHolder.rotation = Quaternion.LookRotation(CameraTransform.forward, screenAngle);
	}

	public override void Run() {
		
		CameraTransform.rotation = Quaternion.Euler(topDownViewRotation.x, topDownViewRotation.y, 0.0f);
		

		_cameraPos = Vector3.SmoothDamp(_cameraPos, thisTransform.position, 
			ref _smoothDampCurrentVelocityLateral, _smoothCameraPosTime);

		_abovePlayer = _cameraPos + Vector3.up * headHeight;

		splitScreenOffset = Vector3.ProjectOnPlane( 
			isRightMostPlayer
				? -CameraTransform.transform.right * splitScreenOffsetFactor
				: CameraTransform.transform.right * splitScreenOffsetFactor, 
			Vector3.up);
		
		CameraTransform.position = splitScreenOffset + _abovePlayer + CameraTransform.rotation * topDownOffset;

		FadeObstacles();
		
		if (Vector3.Distance(PlayerThis.position, PlayerOther.position) < isometricDistance)
			stateMachine.TransitionTo<TransitionToIsometric>();

		if (!isRightMostPlayer && (Quaternion.Euler(0, -45, 0) * (PlayerThis.position - PlayerOther.position)).x > 0 ||
		    isRightMostPlayer && (Quaternion.Euler(0, -45, 0) * (PlayerThis.position - PlayerOther.position)).x < 0 ) {
			stateMachine.TransitionTo<TransitionToOtherSide>();
		}
	}

	private RaycastHit[] hits = new RaycastHit[10];

	private void FadeObstacles() {
		Vector3 offsetDirection = -CameraTransform.forward;
		hits = new RaycastHit[10];
		
		Physics.SphereCastNonAlloc(
			PlayerThis.position,
			2.0f,
			offsetDirection.normalized,
			hits,
			//out  RaycastHit  hit, 
			25.0f, 
			collisionMask);

		//Vector3 offset;
		for (int i = 0; i < hits.Length; i++)
		{
			if (hits[i].collider)
			{
				float dot = Vector3.Dot(
					(PlayerThis.position - hits[i].transform.position).normalized,
					new Vector3(1.0f, 0.0f, 1.0f));
				
				TreeFader tf = hits[i].transform.GetComponent<TreeFader>();
				if (tf != null && dot > 0)
				{
					tf.FadeOut();
				}
			}
		}
	}

	public override void Exit() {
		
	}
}
