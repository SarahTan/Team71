using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	int numTracks = 8;
	Transform tracksParent;
	AudioSource bgm;
	AudioSource trackFeedback;
	AudioSource filterFeedback;
	AudioSource remix;
	AudioSource[] tracks;
	float maxVol = 1f;
	bool[] changingVol;
	
	AudioFilters filters;
	TextAsset[] filterParams;

	string[] songs;
	int numSongs = 3;
	int currentSong;

	// Use this for initialization
	void Start () {		
		filters = GameObject.Find ("Sound Manager").GetComponent<AudioFilters>();
		filterParams = new TextAsset[numSongs];
		filterParams = Resources.LoadAll<TextAsset> ("Filters");

		changingVol = new bool[numTracks];

		songs = new string[numSongs];
		for (int i = 0; i < numSongs; i++) {
			songs[i] = "Music/Song" + i;
		}

		bgm = transform.FindChild ("BGM").GetComponent<AudioSource> ();
		trackFeedback = transform.FindChild ("Track Feedback").GetComponent<AudioSource> ();
		filterFeedback = transform.FindChild ("Filter Feedback").GetComponent<AudioSource> ();
		remix = transform.FindChild ("Remix").GetComponent<AudioSource> ();

		tracksParent = transform.FindChild ("Tracks");
		tracks = new AudioSource[numTracks];
		tracks = tracksParent.GetComponentsInChildren<AudioSource> ();

		currentSong = 2;
		ChangeSong (songs [currentSong]);
		InvokeRepeating ("CheckNumTracksPlaying", 0f, 1f);
		//TestMusic ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	void CheckNumTracksPlaying () {
		int numTracksPlaying = 0;
		foreach (AudioSource track in tracks) {
			if (track.volume == maxVol) {
				numTracksPlaying++;
			}
		}

		if (numTracksPlaying <= 4) {
			trackFeedback.volume = 0.1f;
			filterFeedback.volume = 0.3f;

		} else if (numTracksPlaying <= 6) {			
			trackFeedback.volume = 0.15f;
			filterFeedback.volume = 0.4f;

		} else {
			trackFeedback.volume = 0.2f;
			filterFeedback.volume = 0.5f;
		}
	}

	public void PlayTrackFeedback () {
		trackFeedback.Play();
	}

	public void PlayFilterFeedback () {
		filterFeedback.Play ();
	}

	public void TriggerFilter (int orb) {
		filters.Trigger (orb);
	}

	public void StartMusic (int trackNum) {
		// For testing
//		if (tracks [trackNum].volume == 0f) {
//			tracks [trackNum].volume = 1f;
//		} else if (tracks [trackNum].volume == 1f) {
//			tracks [trackNum].volume = 0f;
//		}

		if (tracks [trackNum].volume == 0f) {
			StartCoroutine (IncreaseVol (tracks [trackNum], trackNum));
		}
	}

	public void StopMusic (int trackNum) {
		if (tracks[trackNum].volume == maxVol) {
			StartCoroutine (DecreaseVol (tracks[trackNum], trackNum));
		}
	}

	public void PlayRemix () {
		StopAllCoroutines ();
		for (int i = 0; i < numTracks; i++) {
			changingVol[i] = false;
		}

		bgm.Stop ();

		// Fade out so there's no awkward silence
		for (int i = 0; i < numTracks; i++) {
			StartCoroutine (DecreaseVol (tracks[i], i));
		}
		remix.Play ();
	}

	public void StopRemix (bool requestChange) {
		remix.Stop ();
		if (requestChange) {
			currentSong = (currentSong + 1) % numSongs;
			ChangeSong (songs [currentSong]);
		} else {
			foreach (AudioSource track in tracks) {
				track.Stop();
				track.volume = 0f;
				track.Play ();
			}
			bgm.Stop();
			bgm.Play();
		}
	}

	public void ChangeMaxVol (float newVol) {
		maxVol = newVol;
		for (int i = 0; i < numTracks; i++) {
			tracks[i].volume = newVol;
		}
		bgm.volume = newVol;
	}

	void TestMusic () {
		foreach (AudioSource track in tracks) {
			track.volume = 1f;
		}
	}

	void ChangeSong (string folder) {
		Debug.Log ("song " + currentSong);
		filters.TurnOff ();
		filters.LoadParams (filterParams[currentSong]);
		AudioClip[] clips = Resources.LoadAll<AudioClip> (folder);

		for (int i = 0; i < tracks.Length; i++) {
			tracks[i].Stop();
			tracks[i].volume = 0f;
			tracks[i].clip = clips[i];
			tracks[i].Play();
		}

		bgm.clip = clips [clips.Length-2];	// The second last track in the folder
		bgm.Play ();

		remix.clip = clips [clips.Length-1];	// The last track in the folder
	}

	IEnumerator IncreaseVol (AudioSource audio, int trackNum) {
		while (changingVol[trackNum]) {
			yield return new WaitForFixedUpdate();
		}

		changingVol [trackNum] = true;
		while (audio.volume < maxVol) {
			audio.volume += 0.01f;
			yield return new WaitForFixedUpdate();
		}
		changingVol[trackNum] = false;
	}

	IEnumerator DecreaseVol (AudioSource audio, int trackNum) {
		yield return null;
		while (changingVol[trackNum]) {
			yield return new WaitForFixedUpdate();
		}

		changingVol[trackNum] = true;
		while (audio.volume > 0) {
			audio.volume -= 0.01f;
			yield return new WaitForFixedUpdate();
		}
		changingVol[trackNum] = false;
	}

}
