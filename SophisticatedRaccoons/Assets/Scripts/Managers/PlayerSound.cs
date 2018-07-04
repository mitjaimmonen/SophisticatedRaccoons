using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PlayerSound : MonoBehaviour {

	[FMODUnity.EventRef] public string chantSound;
	[FMODUnity.EventRef] public string getHitSound;
	[FMODUnity.EventRef] public string moveSound;
	[FMODUnity.EventRef] public string pushSound;
	[FMODUnity.EventRef] public string pushedOutSound;
	[FMODUnity.EventRef] public string sfxGetHitSound;
	[FMODUnity.EventRef] public string sfxHitEnemySound;

	public void PlayChantSound()
	{
		FMODUnity.RuntimeManager.PlayOneShotAttached(chantSound, this.gameObject);
	}

	public void PlayGetHitSound()
	{
		FMODUnity.RuntimeManager.PlayOneShotAttached(getHitSound, this.gameObject);
	}

	public void PlayMoveSound()
	{
		FMODUnity.RuntimeManager.PlayOneShotAttached(moveSound, this.gameObject);
	}

	public void PlayPushSound()
	{
		FMODUnity.RuntimeManager.PlayOneShotAttached(pushSound, this.gameObject);
	}
	public void PlayPushedOutSound()
	{
		FMODUnity.RuntimeManager.PlayOneShotAttached(pushedOutSound, this.gameObject);
	}
	public void PlaySfxGetHitSound()
	{
		FMODUnity.RuntimeManager.PlayOneShotAttached(sfxGetHitSound, this.gameObject);
	}
	public void PlaySfxHitEnemySound()
	{
		FMODUnity.RuntimeManager.PlayOneShotAttached(sfxHitEnemySound, this.gameObject);
	}
}
