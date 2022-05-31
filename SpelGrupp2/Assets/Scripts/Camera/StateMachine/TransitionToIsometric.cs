using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Create CameraState/TransitionToIsometric")]
public class TransitionToIsometric : CameraBaseState
{
	private float headHeight = 1.6f;
	
	private Vector3 depthMaskPlanePos;
	private Vector3 centroid;
	private Vector3 abovePlayer;

	private Vector3 topDownOffset = new Vector3(0.0f, 1.0f, -20.0f);

	private Vector2 topDownViewRotation = new Vector2(55, 45);
	private readonly Quaternion _ninetyDegrees = Quaternion.Euler(0.0f, 90.0f, 0.0f);

	private float percentage = 0.0f;
	
	[SerializeField] 
	private LayerMask collisionMask;

	private Vector3 initialSplitScreenPosition;
	
	private float zoomedInDistance = -12.0f;
	private float zoomedOutDistance = -20.0f;
	private float splitMagnitude = 13.0f;
	private float lateralSplitMagnitude = 7.5f;
	
	private void Awake() {
		abovePlayer = Vector3.up * headHeight;
	}
	public override void Enter() {
		percentage = 0.0f;
	    depthMaskPlanePos = DepthMaskPlane.localPosition;
	    depthMaskPlanePos.x = -.5f;
	    DepthMaskPlane.localPosition = depthMaskPlanePos;
	    bool isRightMostPlayer = (Quaternion.Euler(0, -45, 0) * (PlayerThis.position - PlayerOther.position)).x > 0;
	    initialSplitScreenPosition = Vector3.ProjectOnPlane(isRightMostPlayer ? Vector3.up : Vector3.down, CameraTransform.transform.forward);
    }

    public override void Run() {
	    //Input();

		percentage += Time.deltaTime * 2.0f;
	    float easedPercentage = Ease.EaseInOutCubic(percentage);
	    
	    // both cameras have the same rotation (fixed?)
	    CameraTransform.rotation = Quaternion.Euler(topDownViewRotation.x, topDownViewRotation.y, 0.0f);
	    //CameraTransform.rotation = Quaternion.Slerp(CameraTransform.rotation,  Quaternion.Euler(topDownViewRotation.x, topDownViewRotation.y, 0.0f), percentage);

	    // the split is rotating freely between the players
	    RotateScreenSplit(easedPercentage);
		
	    // both cameras follow the centroid point between the players, split when necessary
	    Vector3 centroidOffsetPosition = (PlayerOther.position - PlayerThis.position) * .5f;
	    centroid = PlayerThis.position + centroidOffsetPosition;

	    float distanceInCameraViewSpace = (Quaternion.Euler(0, -45, 0) * (PlayerOther.position - PlayerThis.position)).z;
	    float inv = Mathf.InverseLerp(0.0f, 20.0f, Mathf.Abs(distanceInCameraViewSpace));
	    float dynamicSplitMagnitude = Mathf.Lerp(splitMagnitude, lateralSplitMagnitude, inv);
	    float distanceFraction = Vector3.Distance(PlayerThis.position, PlayerOther.position) * .5f / dynamicSplitMagnitude;
	    float zoom = Mathf.Lerp(zoomedInDistance, zoomedOutDistance, distanceFraction);
	    topDownOffset.z = Mathf.Lerp(zoomedOutDistance, zoom, easedPercentage);
	    CameraTransform.position = Vector3.Lerp(CameraTransform.position,  centroid + abovePlayer + CameraTransform.rotation * topDownOffset, easedPercentage);
	    
	    LerpSplitScreenLineWidth(easedPercentage);
		 
	    FadeObstacles();
		
	    if (percentage > 1.0f)
		    stateMachine.TransitionTo<TopDownState>();
    }
    
    private void RotateScreenSplit(float t) {
	    
	    Vector3 angle = (PlayerOther.position - PlayerThis.position).normalized;
	    Vector3 quarterAngle = _ninetyDegrees * angle;
	    Vector3 screenAngle = Vector3.Lerp(initialSplitScreenPosition ,Vector3.ProjectOnPlane(quarterAngle, -CameraTransform.transform.forward), t);
	    DepthMaskHolder.rotation = Quaternion.LookRotation(CameraTransform.forward, screenAngle);
    }
    
    private void LerpSplitScreenLineWidth(float percentage) {

	    float lineWidth = Mathf.Lerp(-.5f, -.497f, 1.0f - percentage);
	    depthMaskPlanePos.x = lineWidth;
	    DepthMaskPlane.localPosition = depthMaskPlanePos;
    }

    public static float Remap (float from, float fromMin, float fromMax, float toMin,  float toMax) {
		
	    float fromAbs = from - fromMin;
	    float fromMaxAbs = fromMax - fromMin;      
      
	    float normal = fromAbs / fromMaxAbs;

	    float toMaxAbs = toMax - toMin;
	    float toAbs = toMaxAbs * normal;

	    float to = toAbs + toMin;
	    return to;
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
    
    private void Input() {
	    // mouseMovement.x += UnityEngine.Input.GetAxisRaw(MouseX) * mouseSensitivityX;
	    // mouseMovement.z -= UnityEngine.Input.GetAxisRaw(MouseY) * mouseSensitivityY;
	    // mouseMovement.z = Mathf.Clamp(mouseMovement.z, Camera.clampLookupMax - lookOffset, Camera.clampLookupMin - lookOffset);
    }
}
