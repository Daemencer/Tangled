using UnityEngine;
using System.Collections;

public class SoundPlayer : MonoBehaviour
{
	static long i = 0;
	public AudioSource source;

	public bool onStart;
	public bool pauseOnGamePause = false;
	string registerName;
	bool soundRegistered = false;

	void Start ()
	{
		RegisterSoundIfNotDone();
		if (onStart)
			PlaySound();
	}

	public void PlaySound(AudioClip clip = null)
	{
		RegisterSoundIfNotDone();
		if (clip)
			source.clip = clip;
		SoundManager.Instance.PlaySound(registerName);
	}

	public void StopSound()
	{
		RegisterSoundIfNotDone();
		SoundManager.Instance.StopSound(registerName);
	}

	void RegisterSoundIfNotDone()
	{
		if (!soundRegistered)
		{
			soundRegistered = true;
			registerName = "SoundPlayerSound" + i;
			SoundManager.Instance.RegisterSound(registerName, source, pauseOnGamePause);
			i++;
		}
	}
}
