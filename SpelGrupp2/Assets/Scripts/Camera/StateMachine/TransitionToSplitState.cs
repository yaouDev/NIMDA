using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create CameraState/TransitionToSplitState")]
public class TransitionToSplitState : CameraBaseState {

	[SerializeField] 
	private LayerMask collisionMask;
	
	[SerializeField] [Range(0.0f, 2.0f)]
	private float headHeight = 1.6f;
	
	private bool isRightMostPlayer;

	private Vector2 topDownViewRotation = new Vector2(55, 45);
	
	private Vector3 topDownOffset = new Vector3(0.0f, 1.0f, -20.0f);
    
	private float splitScreenOffsetFactor = 8.0f;
	
	private Vector3 _cameraPos;
	private Vector3 _abovePlayer;
	private Vector3 _smoothDampCurrentVelocityLateral;
	private Vector3 _lerpOffset;
	private Vector3 splitScreenOffset;
	private Vector3 _smoothDampCurrentVelocity;
	private Vector3 depthMaskPlanePos;
	private float _smoothDollyTime;
	private float splitScreenWidth = -.497f;
	private readonly Quaternion _ninetyDegrees = Quaternion.Euler(0.0f, 90.0f, 0.0f);
	private float percentage = 0.0f;
	private float splitMagnitude = 13.0f;
	private float lateralSplitMagnitude = 7.5f;
	private float zoomedInDistance = -12.0f;
	private float zoomedOutDistance = -20.0f;
	private void Awake() {
	}
	
	public override void Enter() {
		percentage = 0.0f;
		depthMaskPlanePos = DepthMaskPlane.localPosition;
		depthMaskPlanePos.x = splitScreenWidth;
		DepthMaskPlane.localPosition = depthMaskPlanePos;
		//_cameraPos = PlayerThis.position;
	}

	public override void Run() {
		CameraTransform.rotation = Quaternion.Euler(topDownViewRotation.x, topDownViewRotation.y, 0.0f);

		percentage += Time.deltaTime;
		float easedPercentage = Ease.EaseInOutCubic(percentage);
		
		// split rotation is Linearly interpolated to vertical
		isRightMostPlayer = (Quaternion.Euler(0, -45, 0) * (PlayerThis.position - PlayerOther.position)).x > 0;
		Vector3 angle = (PlayerOther.position - PlayerThis.position).normalized;
		Vector3 quarterAngle = _ninetyDegrees * angle;
		Vector3 screenAngle = Vector3.ProjectOnPlane(quarterAngle, -CameraTransform.transform.forward);
		Vector3 verticalAngle = Vector3.ProjectOnPlane(isRightMostPlayer ? Vector3.up : Vector3.down, CameraTransform.forward);
		DepthMaskHolder.rotation = 
				Quaternion.Slerp(
					Quaternion.LookRotation(CameraTransform.forward, screenAngle), 
					Quaternion.LookRotation(CameraTransform.forward, verticalAngle), 
					easedPercentage * 2.0f);

		//_cameraPos = PlayerThis.position;// Vector3.Lerp(_cameraPos, thisTransform.position, percentage);
		_abovePlayer = PlayerThis.position + Vector3.up * headHeight;

		// equation for centroid
		float distanceInCameraViewSpace = (Quaternion.Euler(0, -45, 0) * (PlayerOther.position - PlayerThis.position)).z;
		float inv = Mathf.InverseLerp(0.0f, 20.0f, Mathf.Abs(distanceInCameraViewSpace));
		float dynamicSplitMagnitude = Mathf.Lerp(splitMagnitude, lateralSplitMagnitude, inv);
		// both cameras follow the centroid point between the players, split when necessary
		Vector3 centroidOffsetPosition = (PlayerOther.position - PlayerThis.position) * .5f;
		Vector3 centroid = PlayerThis.position + Vector3.ClampMagnitude( centroidOffsetPosition, dynamicSplitMagnitude);

		FadeObstacles();
		
		splitScreenOffset = Vector3.ProjectOnPlane( 
			isRightMostPlayer
				? -CameraTransform.transform.right * splitScreenOffsetFactor
				: CameraTransform.transform.right * splitScreenOffsetFactor, 
			Vector3.up);
		
		Vector3 lerpOffset = Vector3.Lerp(centroidOffsetPosition, splitScreenOffset, easedPercentage) + _abovePlayer;
		
		// Camera zoom
		float distanceFraction = Vector3.Distance(PlayerThis.position, PlayerOther.position) * .5f / dynamicSplitMagnitude;
		topDownOffset.z = Mathf.Lerp(zoomedInDistance, zoomedOutDistance, distanceFraction);
		
		// Camera Position
		CameraTransform.position = lerpOffset + CameraTransform.rotation * topDownOffset;

		LerpSplitScreenLineWidth(easedPercentage);
		
		if (percentage > 1.0f)
			stateMachine.TransitionTo<SplitScreenState>();
	}
	
	private void LerpSplitScreenLineWidth(float t) {

		float lineWidth = Mathf.Lerp(-.5f, -.497f, t);
		depthMaskPlanePos.x = lineWidth;
		DepthMaskPlane.localPosition = depthMaskPlanePos;
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
