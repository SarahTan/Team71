using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	int numTracks = 6;
	Transform tracksParent;
	AudioSource bgm;
	AudioSource feedback;
	AudioSource remix;
	AudioSource[] tracks;
	float maxVol = 1f;
	bool[] changingVol;

	string[] songs;
	int numSongs = 1;
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
		for (int i = 0; i < numTracks; i++) {
			tracks[i] = tracksParent.GetChild(i).GetComponent<AudioSource>();
		}
		currentSong = 0;
		ChangeSong (songs [currentSong]);
	}
	
	// Update is called once per frame
	void Update () {

	}

	// CALL THIS FUNCTION
	public void SendInput (int input) {
		int trackNum = -1;

		switch (input) {
		case 11:
			trackNum = 0;
			break;

		case 10:
			trackNum = 1;
			break;

		case 9:
			trackNum = 2;
			break;
			
		case 8:
			trackNum = 3;
			break;

		case 17:
			trackNum = 4;
			break;
			
		case 16:
			trackNum = 5;
			break;

		default:
			break;
		}

		if (trackNum != -1 && tracks [trackNum].volume == 0f) {
			feedback.Play();
			StartCoroutine (IncreaseVol (tracks[trackNum], trackNum));
		}
	}

	public void PlayFeedback () {
		feedback.Play();
	}

	public void StartMusic (int trackNum) {
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
		foreach (AudioSource audio in tracks) {
			audio.volume = 0f;
		}
		remix.Play ();
	}

	public void StopRemix () {
		remix.Stop ();

		int nextSong = currentSong;

		while (numSongs != 1 && nextSong == currentSong) {
			nextSong = Random.Range (0, numSongs);
		}

		ChangeSong (songs[nextSong]);
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
			audio.volume += 0.05f;
			yield return new WaitForFixedUpdate();
		}
		changingVol[trackNum] = false;
	}

	IEnumerator DecreaseVol (AudioSource audio, int trackNum) {
		while (changingVol[trackNum]) {
			yield return new WaitForFixedUpdate();
		}

		changingVol[trackNum] = true;
		while (audio.volume > 0) {
			audio.volume -= 0.05f;
			yield return new WaitForFixedUpdate();
		}
		changingVol[trackNum] = false;
	}

}
