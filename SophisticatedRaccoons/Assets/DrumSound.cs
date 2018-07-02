using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumSound : MonoBehaviour {


	[FMODUnity.EventRef] public string[] drumSounds;
	public int randIndex = 0;
	// Use this for initialization
	void Start () {

		randIndex = Random.Range(0, drumSounds.Length);
		FMODUnity.RuntimeManager.PlayOneShotAttached(drumSounds[randIndex], this.gameObject);

		
	}
}
