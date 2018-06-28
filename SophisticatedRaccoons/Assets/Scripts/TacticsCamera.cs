using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsCamera : MonoBehaviour {

    float lerpTime = 2f;
    bool rotationDone = false;

    public void ChangePlayer(bool p1)
    {
        StartCoroutine(ChangeCamera(p1));
    }

    IEnumerator ChangeCamera(bool playerOne)
    {
        float time = Time.time;
        float t;
        float smoothLerp;
        Vector3 oldAngle = transform.eulerAngles;
        Vector3 newAngle = oldAngle;
        newAngle.y = playerOne ? 0 : 180f;

        while (time+lerpTime > Time.time)
        {
            t = (Time.time-time)/lerpTime;
            smoothLerp = t*t * (3f - 2f*t);
            transform.eulerAngles = Vector3.Lerp(oldAngle, newAngle, smoothLerp);
            yield return null;
        }

        transform.eulerAngles = newAngle;

        yield break;
    }
    public void RotateLeft()
    {
        transform.Rotate(Vector3.up, 90, Space.Self);
    }

    public void RotateRight()
    {
        transform.Rotate(Vector3.up, -90, Space.Self);
    }
	
}
