using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	int numTracks = 8;
	Transform tracksParent;
	AudioSource bgm;
	AudioSource feedback;
	AudioSource remix;
	AudioSource[] tracks;
	float maxVol = 1f;
	bool[] changingVol;

	string[] songs;
	int numSongs = 2;
	int currentSong;

	// Use this for initialization
	void Start () {
		changingVol = new bool[numTracks];

		songs = new string[numSongs];
		for (int i = 0; i < numSongs; i++) {
			songs[i] = "Music/Song" + (i+1);
		}

		bgm = transform.FindChild ("BGM").GetComponent<AudioSource> ();
		feedback = transform.FindChild ("Feedback").GetComponent<AudioSource> ();
		remix = transform.FindChild ("Remix").GetComponent<AudioSource> ();

		tracksParent = transform.FindChild ("Tracks");
		tracks = new AudioSource[numTracks];
		tracks = tracksParent.GetComponentsInChildren<AudioSource> ();

		currentSong = 1;
		ChangeSong (songs [currentSong]);
		//TestMusic ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void TriggerAudioEffect () {

	}

	public void PlayFeedback () {
		feedback.Play();
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
		for (int i = 0; i < numTracks; i++) {
			StartCoroutine (DecreaseVol (tracks[i], i));
		}
		remix.Play ();
	}

	public void StopRemix () {
		remix.Stop ();

		int nextSong = currentSong;

		// To use if we have more than 2 songs
//		while (numSongs != 1 && nextSong == currentSong) {
//			nextSong = Random.Range (0, numSongs);
//		}

		nextSong = (nextSong+1) % 2;

		ChangeSong (songs[nextSong]);
	}

	void TestMusic () {
		foreach (AudioSource track in tracks) {
			track.volume = 1f;
		}
	}

	void ChangeSong (string folder) {
		AudioClip[] clips = Resources.LoadAll<AudioClip> (folder);

		for (int i = 0; i < tracks.Length; i++) {
			tracks[i].clip = clips[i];
			tracks[i].volume = 0f;
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
