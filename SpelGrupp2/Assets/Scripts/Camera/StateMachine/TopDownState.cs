using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.LWRP;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Create CameraState/CameraTopDownState")]
public class TopDownState : CameraState
{
	[SerializeField] 
	private LayerMask collisionMask;

	[SerializeField]
	private Vector2 topDownViewRotation;
	
	[SerializeField] 
	private Vector3 topDownOffset;
	
	[SerializeField]
	private float headHeight = 1.6f;
	
	[SerializeField] [Range(1.0f, 20.0f)]
	private float splitMagnitude = 6.0f;

	[SerializeField] [Range(1.0f, 20.0f)] 
	private float thirdPersonSplitDistance = 15.0f;

	private Vector3 cameraPosition;
	private Vector3 cameraShakeOffset;
	private static Vector3 sharedCameraShakeOffset;

	
	private Vector3 centroid;
	private Vector3 abovePlayer;
	private Vector3 stencilPlanePosition;
	private Vector3 depthMaskPlanePos;
	private readonly Quaternion _ninetyDegrees = Quaternion.Euler(0.0f, 90.0f, 0.0f);
	private readonly Quaternion fourtyFiveDegrees = quaternion.Euler(0.0f, 45.0f, 0.0f);

	private void Awake() {
		abovePlayer = Vector3.up * headHeight;
	}

	public override void Enter() {
		depthMaskPlanePos = DepthMaskPlane.localPosition;
		depthMaskPlanePos.x = -.5f;
		DepthMaskPlane.localPosition = depthMaskPlanePos;
	}

	private Vector3 lerpOffset;
	private float smoothDollyTime;
	private float smoothDampMinVal = .1f;
	private float smoothDampMaxVal = 1.0f;
	private Vector3 smoothDampCurrentVelocity;
	private RaycastHit[] hits;
	private float lateralSplitMagnitude = 4.5f;

	private static float s;
	private static float sharedTrauma;
	private float trauma;
	private float easedTrauma;
	private float cameraShakeFalloffSpeed = 2.0f;
	private float shakeSpeed = 10.0f;
	private float vibrationSpeed = 20.0f;
	private float rotationFactor = 5.0f;

	public void ShakeCamera(float magnitude) => trauma += magnitude;

	private void CameraShake(float distanceFraction)
	{
		// Debug Timer in place of event
		s += Time.deltaTime;
		if (s >= 4.0f)
		{
			s = 0;
			trauma = 2f;
		}
		
		// perlin within Range(-1, 1)
		float perlinNoiseX = (Mathf.PerlinNoise(0, Time.time * shakeSpeed) - .5f) * 2;
		float perlinNoiseY = (Mathf.PerlinNoise(.5f, Time.time * shakeSpeed) - .5f) * 2;
		// add perlin within Range(-.25, .25)
		perlinNoiseX += (Mathf.PerlinNoise(.25f, Time.time * vibrationSpeed) - .5f) * .5f;
		perlinNoiseY += (Mathf.PerlinNoise(.75f, Time.time * vibrationSpeed) - .5f) * .5f;

		// decrease trauma over time
		trauma = Mathf.Clamp(trauma -= Time.deltaTime * cameraShakeFalloffSpeed, 0.0f, 1.0f);
		sharedTrauma = Mathf.Clamp(sharedTrauma -= Time.deltaTime * cameraShakeFalloffSpeed, 0.0f, 1.0f);
		
		// share screenshake if players on same screen
		if (distanceFraction <= 1.0f && trauma > sharedTrauma) { sharedTrauma = trauma; }
		trauma = Mathf.Max(trauma, sharedTrauma);

		easedTrauma = Ease.EaseInExpo(trauma);
		
		cameraShakeOffset = 
			CameraTransform.rotation * 
			new Vector3(perlinNoiseX * easedTrauma, perlinNoiseY * easedTrauma, 0.0f);
		
		CameraTransform.rotation *= 
			Quaternion.Euler(
				perlinNoiseX * easedTrauma * rotationFactor, 
				perlinNoiseY * easedTrauma * rotationFactor, 
				Mathf.Lerp(perlinNoiseX, perlinNoiseY, .5f) * easedTrauma * rotationFactor);
	}

	public override void Run()
	{
		Input();
		
		// both cameras have the same rotation
		CameraTransform.rotation = Quaternion.Euler(topDownViewRotation.x, topDownViewRotation.y, 0.0f);

		// the split is rotating freely between the players
		RotateScreenSplit();
		
		Vector3 dist = Quaternion.Euler(0, -45, 0) * (PlayerOther.position - PlayerThis.position);
		dist.z *= 1.5f;
		
		// different splitMagnitude x/y axis on screen
		float inv = Mathf.InverseLerp(0.0f, 9.0f, Mathf.Abs(dist.z));
		float dynamicSplitMagnitude = Mathf.Lerp(splitMagnitude, lateralSplitMagnitude, Ease.EaseOutCirc(inv));
		
		// both cameras follow the centroid point between the players, split when necessary
		Vector3 centroidOffsetPosition = (PlayerOther.position - PlayerThis.position) * .5f;
		centroid = PlayerThis.position + Vector3.ClampMagnitude( centroidOffsetPosition, dynamicSplitMagnitude);
		Debug.DrawRay(centroid, Vector3.up * 10.0f);

		// Camera zoom
		float distanceFraction = Vector3.Distance(PlayerThis.position, PlayerOther.position) * .5f / dynamicSplitMagnitude;
		topDownOffset.z = Mathf.Lerp(-12.0f, -16.0f, distanceFraction);

		CameraShake(distanceFraction);

		FadeObstacles();


		
		cameraPosition = centroid + abovePlayer + CameraTransform.rotation * topDownOffset; // TODO in progress

		CameraTransform.position = cameraPosition + cameraShakeOffset; // TODO in progress // centroid + abovePlayer + CameraTransform.rotation * topDownOffset;
		
		LerpSplitScreenLineWidth(centroidOffsetPosition.magnitude, dynamicSplitMagnitude);

		//if (Vector3.Distance(PlayerThis.position, PlayerOther.position) > thirdPersonSplitDistance)
		//	stateMachine.TransitionTo<TransitionToSplitState>();
	}

	private void LerpSplitScreenLineWidth(float offsetMagnitude, float dynamicSplitMagnitude) {

		float t = Remap(offsetMagnitude, dynamicSplitMagnitude, dynamicSplitMagnitude + 1.0f, 0.0f, 1.0f); 
		float lineWidth = Mathf.Lerp(-.5f, -.497f, t);
		depthMaskPlanePos.x = lineWidth;
		DepthMaskPlane.localPosition = depthMaskPlanePos;
	}

	public static float Remap (float value, float fromMin, float fromMax, float toMin,  float toMax) {
		
		float fromAbs = value - fromMin;
		float fromMaxAbs = fromMax - fromMin;      
      
		float normal = fromAbs / fromMaxAbs;

		float toMaxAbs = toMax - toMin;
		float toAbs = toMaxAbs * normal;

		float to = toAbs + toMin;
		return to;
	}

	private void FadeObstacles() {
		Vector3 offsetDirection = -CameraTransform.forward;
		hits = new RaycastHit[10];
		
		Physics.SphereCastNonAlloc(
			PlayerThis.position + offsetDirection * 6,
			//(Vector3.Distance(PlayerOther.position, PlayerThis.position) < splitMagnitude * 2 ? centroidOffsetPosition : Vector3.zero) + thisTransform.position + abovePlayer, 
			2.0f,
			offsetDirection.normalized,
			hits,
			//out  RaycastHit  hit, 
			25.0f, 
			collisionMask);
		
		Debug.DrawRay(
			//centroidOffsetPosition + thisTransform.position + abovePlayer,
			PlayerThis.position,
			offsetDirection * 8.0f, 
			Color.magenta);
		
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
	
	protected void Input() {
		// _mouseMovement.x += playerController.GetCameraMovement().x * mouseSensitivityX * 100;// 0;// TODO UnityEngine.Input.GetAxisRaw(MouseX) * mouseSensitivityX;
		// Debug.Log(_mouseMovement.x);
		// _mouseMovement.z -= playerController.GetCameraMovement().z * mouseSensitivityX * 100;// TODO UnityEngine.Input.GetAxisRaw(MouseY) * mouseSensitivityY;
		// _mouseMovement.z = Mathf.Clamp(_mouseMovement.z, clampLookupMax - LookOffset, clampLookupMin - LookOffset);
	}
	
	private void RotateScreenSplit() {
		Vector3 angle = (PlayerOther.position - PlayerThis.position).normalized;
		Vector3 quarterAngle = _ninetyDegrees * angle;
		Vector3 screenAngle = Vector3.ProjectOnPlane(quarterAngle, -CameraTransform.transform.forward);
		DepthMaskHolder.rotation = Quaternion.LookRotation(CameraTransform.forward, screenAngle);
	}
}
