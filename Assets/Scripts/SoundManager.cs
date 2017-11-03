using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
	[SerializeField]
	private AudioSource _efxSource;                   //Drag a reference to the audio source which will play the sound effects.
	[SerializeField]
	private AudioSource _musicSource;                 //Drag a reference to the audio source which will play the music.

	[SerializeField]
	private List<AudioClip> _soundTracks;                  //Holds a list of songs to play during the game
	[SerializeField]
	private List<AudioClip> _soundEffects;

	public static SoundManager instance = null;     //Allows other scripts to call functions from SoundManager.             

	//Used to play single sound clip by their name.
	public void PlayEffect(string clipName)
	{
		// Find and select the appropirate clip
		AudioClip temp = _soundEffects.Find(obj => obj.name == clipName);

		//Set the clip of our efxSource audio source to the clip passed in as a parameter.
		_efxSource.clip = temp;

		//Play the clip.
		_efxSource.Play ();
	}


	//Used to play an audio clip
	public void PlayClip(AudioClip clip)
	{
		//Set the clip of our efxSource audio source to the clip passed in as a parameter.
		_efxSource.clip = clip;

		//Play the clip.
		_efxSource.Play ();
	}


	//Used to play a click sound. Called by buttons.
	public void PlayClick()
	{
		PlayEffect("click");
	}

	//On call, continuously selects and plays random tracks from the soundtrack
	public void PlayMusic()
	{
		_musicSource.clip = _soundTracks[Random.Range(0, _soundTracks.Count)];
		_musicSource.Play();
		Invoke("PlayMusic", _musicSource.clip.length);
	}
}
