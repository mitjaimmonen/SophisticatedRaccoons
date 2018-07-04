using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DrumSound : MonoBehaviour {


	[FMODUnity.EventRef] public string[] drumSounds;
	FMOD.Studio.EventInstance drumEI;
	public int randIndex = 0;
	Rigidbody rb;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		randIndex = Random.Range(0, drumSounds.Length);
		drumEI = FMODUnity.RuntimeManager.CreateInstance(drumSounds[randIndex]);
		drumEI.start();
		FMODUnity.RuntimeManager.AttachInstanceToGameObject(drumEI, gameObject.transform, rb);
		// FMODUnity.RuntimeManager.PlayOneShotAttached(drumSounds[randIndex], this.gameObject);
	}
	void OnDisable()
	{
		drumEI.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
	}

}
