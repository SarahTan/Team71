using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	int numTracks = 6;
	Transform tracksParent;
	AudioSource feedback;
	AudioSource[] tracks;
	float[] tracksMaxVol;

	// Use this for initialization
	void Start () {
		feedback = transform.FindChild ("Feedback").GetComponent<AudioSource> ();
		tracksParent = transform.FindChild ("Tracks");

		tracks = new AudioSource[numTracks];
		for (int i = 0; i < numTracks; i++) {
			tracks[i] = tracksParent.GetChild(i).GetComponent<AudioSource>();
		}

		// Set the play volume for each of the tracks
		tracksMaxVol = new float[numTracks];
		tracksMaxVol [0] = tracksMaxVol [1] = tracksMaxVol [2] = tracksMaxVol [4] = 1f;
		tracksMaxVol [3] = 0.75f;
		tracksMaxVol [5] = 0.5f;
	}
	
	// Update is called once per frame
	void Update () {
		int track = -1;

		// Using W, A, S, D, F, G
		if (Input.GetKeyDown ("1")) {
			track = 0;

		} else if (Input.GetKeyDown ("2")) {
			track = 1;
			
		} else if (Input.GetKeyDown ("3")) {
			track = 2;
			
		} else if (Input.GetKeyDown ("4")) {
			track = 3;
			
		} else if (Input.GetKeyDown ("5")) {
			track = 4;
			
		} else if (Input.GetKeyDown ("6")) {
			track = 5;	
		}

		if (track != -1 && tracks [track].volume == 0f) {
			feedback.Play();
			StartCoroutine (IncreaseVol (track));
		}
	}

	// CALL THIS FUNCTION
	public void SendInput (int input) {
		int track = -1;

		switch (input) {
		case 11:
			track = 0;
			break;

		case 10:
			track = 1;
			break;

		case 9:
			track = 2;
			break;
			
		case 8:
			track = 3;
			break;

		case 17:
			track = 4;
			break;
			
		case 16:
			track = 5;
			break;

		default:
			break;
		}

		if (track != -1 && tracks [track].volume == 0f) {
			feedback.Play();
			StartCoroutine (IncreaseVol (track));
		}
	}

	public void PlayFeedback () {
		feedback.Play();
	}

	public void StartMusic (int trackNum) {
		// Only create a thread to increase vol if it's at 0!
		if (tracks[trackNum].volume == 0) {
			StartCoroutine (IncreaseVol (trackNum));
		}
	}

	public void StopMusic (int trackNum) {
		// Only create a thread to decrease vol if it's at it's max!
		if (tracks [trackNum].volume == tracksMaxVol[trackNum]) {
			StartCoroutine (DecreaseVol (trackNum));
		}
	}

	public void PlayRemix () {

	}

	IEnumerator IncreaseVol (int trackNum) {
		while (tracks[trackNum].volume < tracksMaxVol[trackNum]) {
			tracks [trackNum].volume += 0.05f;
			yield return null;
		}
	}

	IEnumerator DecreaseVol (int trackNum) {
		yield return new WaitForSeconds (8f);

		while (tracks[trackNum].volume > 0) {
			tracks [trackNum].volume -= 0.05f;
			yield return null;
		}
	}

}
