using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	int numTracks = 6;
	Transform tracksParent;
	AudioSource feedback;
	AudioSource[] tracks;
	float[] tracksVol;

	// Use this for initialization
	void Start () {
		feedback = transform.FindChild ("Feedback").GetComponent<AudioSource> ();
		tracksParent = transform.FindChild ("Tracks");

		tracks = new AudioSource[numTracks];
		for (int i = 0; i < numTracks; i++) {
			tracks[i] = tracksParent.GetChild(i).GetComponent<AudioSource>();
		}

		// Set the play volume for each of the tracks
		tracksVol = new float[numTracks];
		tracksVol [0] = tracksVol [1] = tracksVol [2] = tracksVol [4] = 1f;
		tracksVol [3] = 0.75f;
		tracksVol [5] = 0.5f;
	}
	
	// Update is called once per frame
	void Update () {
		// Using W, A, S, D, F, G
		if (Input.GetKeyDown ("w")) {
			StartCoroutine (IncreaseVol (0));

		} else if (Input.GetKeyDown ("a")) {
			StartCoroutine (IncreaseVol (1));
			
		} else if (Input.GetKeyDown ("s")) {
			StartCoroutine (IncreaseVol (2));
			
		} else if (Input.GetKeyDown ("d")) {
			StartCoroutine (IncreaseVol (3));
			
		} else if (Input.GetKeyDown ("f")) {
			StartCoroutine (IncreaseVol (4));
			
		} else if (Input.GetKeyDown ("g")) {
			StartCoroutine (IncreaseVol (5));			
		}
	}

	// CALL THIS FUNCTION
	public void SendInput (int track) {
		switch (track) {
		case 11:
			StartCoroutine (IncreaseVol (0));
			break;

		case 10:
			StartCoroutine (IncreaseVol (1));
			break;

		case 9:
			StartCoroutine (IncreaseVol (2));
			break;
			
		case 8:
			StartCoroutine (IncreaseVol (3));
			break;
		case 17:
			StartCoroutine (IncreaseVol (4));
			break;
			
		case 16:
			StartCoroutine (IncreaseVol (5));
			break;

		default:
			break;
		}
	}


	IEnumerator IncreaseVol (int trackNum) {
		feedback.Play();

		if (tracks [trackNum].volume == 0f) {
			Debug.Log ("Track " + trackNum + " starting");
			while (tracks[trackNum].volume < tracksVol[trackNum]) {
				tracks [trackNum].volume += 0.05f;
				yield return null;
			}
			StartCoroutine (DecreaseVol (trackNum));
		} else {
			Debug.Log ("Track " + trackNum + " already playing");
		}

	}

	IEnumerator DecreaseVol (int trackNum) {
		yield return new WaitForSeconds (8f);
		Debug.Log ("Track " + trackNum + " ending");

		while (tracks[trackNum].volume > 0) {
			tracks [trackNum].volume -= 0.05f;
			yield return null;
		}
	}

}
