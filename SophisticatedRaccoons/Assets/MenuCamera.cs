using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour {

	public float lerpTime;

    public Vector3 posStart;
    public Vector3 eulerStart;
    public Vector3 posMiddleLerp;
    public Vector3 eulerMiddleLerp;
    public Vector3 posEnd;
    public Vector3 eulerEnd;
	

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
		Vector3 newEuler;
		while (time + lerpTime > Time.time)
		{
            t = (Time.time-time)/lerpTime;
            smoothLerp = t*t * (3f - 2f*t);
			Vector3 lerpstart = Vector3.Lerp(posEnd,posMiddleLerp, t);
			Vector3 lerpEnd = Vector3.Lerp(posMiddleLerp,posStart, t);
			newPos = Vector3.Lerp(lerpstart, lerpEnd, smoothLerp);
			transform.position = newPos;

			lerpstart = Vector3.Lerp(eulerEnd, eulerMiddleLerp, t);
			lerpEnd = Vector3.Lerp(eulerMiddleLerp, eulerStart, t);
			newEuler = Vector3.Lerp(lerpstart, lerpEnd, smoothLerp);
			transform.eulerAngles = newEuler;

			yield return null;
		}

		transform.position = posStart;
		transform.eulerAngles = eulerStart;
		yield break;
	}
	IEnumerator CrossLerpToEnd()
	{
		//Simultaneous lerp from start-to-middle & middle-to-end
		float time = Time.time;
		float t = 0, smoothLerp = 0;
		Vector3 newPos;
		Vector3 newEuler;
		while (time + lerpTime > Time.time)
		{
            t = (Time.time-time)/lerpTime;
            smoothLerp = t*t * (3f - 2f*t);
			Vector3 lerpstart = Vector3.Lerp(posStart,posMiddleLerp, t);
			Vector3 lerpEnd = Vector3.Lerp(posMiddleLerp,posEnd, t);
			newPos = Vector3.Lerp(lerpstart, lerpEnd, smoothLerp);
			transform.position = newPos;

			lerpstart = Vector3.Lerp(eulerStart, eulerMiddleLerp, t);
			lerpEnd = Vector3.Lerp(eulerMiddleLerp, eulerEnd, t);
			newEuler = Vector3.Lerp(lerpstart, lerpEnd, smoothLerp);
			transform.eulerAngles = newEuler;

			yield return null;
		}

		transform.position = posEnd;
		transform.eulerAngles = eulerEnd;
		yield break;
	}
}
