using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class MenuCamera : MonoBehaviour {

	public float lerpTime;
    public PostProcessingBehaviour menuPostProcessing;
    public float focusStart, focusEnd;


	public Transform cameraStart;
	public Transform cameraMiddle;
	public Transform cameraEnd;

	void Awake()
	{
		if (!cameraStart || !cameraMiddle || !cameraEnd)
			Debug.LogError("Camera transforms missing!");
		DepthOfFieldModel.Settings dof = menuPostProcessing.profile.depthOfField.settings;
		dof.focusDistance = focusStart;
		menuPostProcessing.profile.depthOfField.settings = dof;
	}
	

	public void ToTitleScreen()
	{
		StartCoroutine(CrossLerpToStart());

	}
	public void ToJoinScreen()
	{
		StartCoroutine(CrossLerpToEnd());
	}
	IEnumerator CrossLerpToStart()
	{
		//Simultaneous lerp from start-to-middle & middle-to-end

		float time = Time.time;
		float t = 0, smoothLerp = 0;
		Vector3 newPos;
		Quaternion newRot;
		DepthOfFieldModel.Settings dof = menuPostProcessing.profile.depthOfField.settings;
		while (time + lerpTime > Time.time)
		{
            t = (Time.time-time)/lerpTime;
            smoothLerp = t*t * (3f - 2f*t);
			Vector3 lerpstart = Vector3.Lerp(cameraEnd.position,cameraMiddle.position, t);
			Vector3 lerpEnd = Vector3.Lerp(cameraMiddle.position,cameraStart.position, t);
			newPos = Vector3.Lerp(lerpstart, lerpEnd, smoothLerp);
			transform.position = newPos;

			Quaternion slerpstart = Quaternion.Slerp(cameraEnd.rotation, cameraMiddle.rotation, t);
			Quaternion slerpEnd = Quaternion.Slerp(cameraMiddle.rotation, cameraStart.rotation, t);
			newRot = Quaternion.Slerp(slerpstart, slerpEnd, smoothLerp);
			transform.rotation = newRot;

			dof.focusDistance = Mathf.Lerp(focusEnd, focusStart, t);
			menuPostProcessing.profile.depthOfField.settings = dof;

			yield return null;
		}

		transform.position = cameraStart.position;
		transform.rotation = cameraStart.rotation;
		dof.focusDistance = focusStart;
		menuPostProcessing.profile.depthOfField.settings = dof;
		yield break;
	}
	IEnumerator CrossLerpToEnd()
	{
		//Simultaneous lerp from start-to-middle & middle-to-end
		float time = Time.time;
		float t = 0, smoothLerp = 0;
		Vector3 newPos;
		Quaternion newRot;
		DepthOfFieldModel.Settings dof = menuPostProcessing.profile.depthOfField.settings;
		while (time + lerpTime > Time.time)
		{
            t = (Time.time-time)/lerpTime;
            smoothLerp = t*t * (3f - 2f*t);
			Vector3 lerpstart = Vector3.Lerp(cameraStart.position,cameraMiddle.position, t);
			Vector3 lerpEnd = Vector3.Lerp(cameraMiddle.position,cameraEnd.position, t);
			newPos = Vector3.Lerp(lerpstart, lerpEnd, smoothLerp);
			transform.position = newPos;

			Quaternion slerpstart = Quaternion.Lerp(cameraStart.rotation, cameraMiddle.rotation, t);
			Quaternion slerpEnd = Quaternion.Lerp(cameraMiddle.rotation, cameraEnd.rotation, t);
			newRot = Quaternion.Lerp(slerpstart, slerpEnd, smoothLerp);
			transform.rotation = newRot;

			dof.focusDistance = Mathf.Lerp(focusStart, focusEnd, t);
			menuPostProcessing.profile.depthOfField.settings = dof;

			yield return null;
		}

		transform.position = cameraEnd.position;
		transform.rotation = cameraEnd.rotation;
		dof.focusDistance = focusEnd;
		menuPostProcessing.profile.depthOfField.settings = dof;
		yield break;
	}
}
