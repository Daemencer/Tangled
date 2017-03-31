using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;


[System.Serializable]
public class FloatEvent : UnityEvent<float>
{
}


/*
 Improvement ideas:
  - Can easily manage Looping Sounds and playing modes
  - Can also affords to change the system to hash sets so that we skip the annoying string name part
  - Add a fading effect to musics on scene changes
  - Register a persistant sound manager on scene load using the unity event SceneManager.onSceneLevelLoaded
 */
public class SoundManager : MonoSingleton<SoundManager>
{
	public AudioSource musicSource;

	[Tooltip("Change the music volume at runtime. Set by the user save at the game start.")]
	[SerializeField]
	[Range(0.0f, 100.0f)]
	private float musicVolume = 100.0f;

	[Tooltip("Change the sfx volume at runtime. Set by the user save at the game start.")]
	[SerializeField]
	[Range(0.0f, 100.0f)]
	private float sfxVolume = 100.0f;

	public FloatEvent OnMusicVolumeChanged = new FloatEvent();
	public FloatEvent OnSFXVolumeChanged = new FloatEvent();

	// Properties
	public float MusicVolume
	{
		get { return musicVolume; }
		set { musicVolume = value; OnMusicVolumeChanged.Invoke(musicVolume); }
	}

	public float SFXVolume
	{
		get { return sfxVolume; }
		set { sfxVolume = value; OnSFXVolumeChanged.Invoke(sfxVolume); }
	}

	// could become HashSet so that each GAO plays its sound by simply calling the AudioSource itself as the array index
	private Dictionary<string, AudioSource> sfx;

	UnityEvent OnGamePause = new UnityEvent();
	UnityEvent OnGamePlay = new UnityEvent();

	bool gamePaused = false;


	void Awake()
	{
		sfx = new Dictionary<string, AudioSource>();
	}


	void Start()
	{
		OnMusicVolumeChanged.AddListener(UpdateMusicVolume);
		OnSFXVolumeChanged.AddListener(UpdateSFXVolume);
	}

	public void OnEnable()
	{
		SceneManager.sceneUnloaded += OnSceneUnloaded;
	}
	public void OnDisable()
	{
		SceneManager.sceneUnloaded -= OnSceneUnloaded;
	}

	void OnSceneUnloaded(Scene scene)
	{
		CleanAll();
	}

	void Update()
	{
		if (Time.timeScale == 0 && !gamePaused)
		{
			OnGamePause.Invoke();
			gamePaused = true;
		}
		else if (Time.timeScale != 0 && gamePaused)
		{
			OnGamePlay.Invoke();
			gamePaused = false;
		}
	}

	void UpdateMusicVolume(float volume)
	{
		musicSource.volume = volume / 100f;
	}

	void UpdateSFXVolume(float volume)
	{
		foreach (var source in sfx.Select(x => x.Value))
			source.volume = volume / 100f;
	}

	// Register an SFX sound into the library
	public void RegisterSound(string name, AudioSource source, bool pauseWhenGamePause = false)
	{
		if (pauseWhenGamePause)
		{
			OnGamePause.AddListener(() =>
			{
				if (source.isPlaying)
					source.Pause();
			});

			OnGamePlay.AddListener(() =>
			{
				source.UnPause();
			});
		}

		if (!sfx.ContainsKey(name))
			sfx.Add(name, source);
	}

	public void CleanAll()
	{
		sfx.Clear();
		OnGamePause.RemoveAllListeners();
		OnGamePlay.RemoveAllListeners();
	}

	public void PlaySound(string name)
	{
		if (sfx.Count > 0 && sfx != null)
		{
			AudioSource source = sfx[name];
			// sets the audio source volume to the manager's volume
			source.volume = sfxVolume / 100.0f;
			// play the sound
			source.Play();
		}
	}

	public void StopSound(string name)
	{
		AudioSource source = sfx[name];
		// play the sound
		source.Stop();
	}

	/// <summary>
	/// Replace the current music by the selected AudioClip. 
	/// If it is the same clip, the music just continue.
	/// </summary>
	public void PlayMusic(AudioClip clip)
	{
		if (musicSource.clip != clip)
		{
			musicSource.Stop();
			musicSource.clip = clip;
			if (clip)
				musicSource.Play();
		}
	}

	/// <summary>
	/// Play the current music of the music audio source.
	/// </summary>
	public void PlayMusic()
	{
		musicSource.Play();
	}

	/// <summary>
	/// Stop the current music of the music audio source.
	/// </summary>
	public void StopMusic()
	{
		musicSource.Stop();
	}
}