using UnityEngine;
using System.Collections;

public class AudioFilters : MonoBehaviour {

	GameObject[] sources = new GameObject[8];
	float effectDuration = 8f;

	// Use this for initialization
	void Start () {
		sources [0] = transform.GetChild (0).gameObject;
		Transform tracks = transform.GetChild (3);
		for (int i = 1; i < sources.Length; i++) {
			sources[i] = tracks.GetChild(i).gameObject;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Tigger (int effect) {
		TurnOff ();

		switch (effect) {
		case 0:
			Chorus ();
			break;

		case 1:
			Reverb ();
			break;

		case 2:
			Echo ();
			break;

		case 3:
			LPF ();
			break;

		case 4:
			HPF ();
			break;

		default:
			break;
		}

		Invoke ("TurnOff", effectDuration);
	}
	
	void Chorus () {

	}

	void Reverb () {
		foreach (GameObject source in sources) {
			source.GetComponent<AudioReverbFilter>().reverbPreset = AudioReverbPreset.Drugged;
		}
	}

	void Echo() {

	}

	void LPF () {

	}

	void HPF () {

	}

	void TurnOff () {
		foreach (GameObject source in sources) {
			source.GetComponent<AudioChorusFilter>().enabled = false;
			source.GetComponent<AudioEchoFilter>().enabled = false;
			source.GetComponent<AudioHighPassFilter>().enabled = false;
			source.GetComponent<AudioLowPassFilter>().enabled = false;
			source.GetComponent<AudioReverbFilter>().enabled = false;
		}
	}
}


/************************
 * 	CHEAT SHEET
 * 
 *	1. Chorus (varies)
 *	2. Reverb Drugged 
 * 	3. Echo (varies)
 * 	4. LPF (main, varies) + Reverb Concert Hall (secondary)
 * 	5. HPF (main, varies) + Reverb and Echo (varies)
 * 
 *********************** /