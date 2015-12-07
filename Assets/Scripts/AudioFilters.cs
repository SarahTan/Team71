using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;

public class AudioFilters : MonoBehaviour {

	int numTracks = 9;
	float effectDuration = 60f;
	GameObject[] sources;

	Chorus[] chorusEffects;
	Echo[] echoEffects;
	LPF[] LPFEffects;
	HPF[] HPFEffects;

	// Use this for initialization
	void Awake () {
		sources = new GameObject[numTracks];
		chorusEffects = new Chorus[numTracks];
		echoEffects = new Echo[numTracks];
		LPFEffects = new LPF[numTracks];
		HPFEffects = new HPF[numTracks];
		
		Transform tracks = transform.GetChild (3);
		for (int i = 0; i < sources.Length-1; i++) {
			sources[i] = tracks.GetChild(i).gameObject;
		}		
		sources [sources.Length-1] = transform.GetChild (0).gameObject;		// BGM
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.Return)) {
			TurnOff();
		}
	}
	

	public void Trigger (int effect) {
		TurnOff ();

		switch (effect) {
		case 8:
			TriggerChorus ();
			break;

		case 9:
			TriggerReverb ();
			break;

		case 10:
			TriggerEcho ();
			break;

		case 11:
			TriggerLPF ();
			break;

		case 12:
			TriggerHPF ();
			break;

		default:
			break;
		}

		CancelInvoke ();
		Invoke ("TurnOff", effectDuration);
	}
	
	
	public void TurnOff () {
		foreach (GameObject source in sources) {
			source.GetComponent<AudioChorusFilter>().enabled = false;
			source.GetComponent<AudioEchoFilter>().enabled = false;
			source.GetComponent<AudioHighPassFilter>().enabled = false;
			source.GetComponent<AudioLowPassFilter>().enabled = false;
			source.GetComponent<AudioReverbFilter>().enabled = false;
		}
	}


	void TriggerChorus () {
		for (int i = 0; i < sources.Length; i++) {
			AudioChorusFilter chorus = sources[i].GetComponent<AudioChorusFilter>();
			chorus.enabled = true;
			chorus.dryMix = chorusEffects[i].drymix;
			chorus.wetMix1 = chorusEffects[i].wetmix1;
			chorus.wetMix2 = chorusEffects[i].wetmix2;
			chorus.wetMix3 = chorusEffects[i].wetmix3;
			chorus.delay = chorusEffects[i].delay;
			chorus.rate = chorusEffects[i].rate;
			chorus.depth = chorusEffects[i].depth;
		}
	}

	void TriggerReverb () {
		foreach (GameObject source in sources) {
			source.GetComponent<AudioReverbFilter>().enabled = true;
			source.GetComponent<AudioReverbFilter>().reverbPreset = AudioReverbPreset.Drugged;
		}
	}

	void TriggerEcho() {
		for (int i = 0; i < sources.Length; i++) {
			AudioEchoFilter echo = sources[i].GetComponent<AudioEchoFilter>();
			echo.enabled = true;
			echo.delay = echoEffects[i].delay;
			echo.decayRatio = echoEffects[i].decay;
			echo.dryMix = echoEffects[i].dry;
			echo.wetMix = echoEffects[i].wet;
		}
	}

	void TriggerLPF () {
		for (int i = 0; i < sources.Length; i++) {
			AudioLowPassFilter lpf = sources[i].GetComponent<AudioLowPassFilter>();
			lpf.enabled = true;
			lpf.cutoffFrequency = LPFEffects[i].cutoff;
			lpf.lowpassResonanceQ = LPFEffects[i].resonance;

			sources[i].GetComponent<AudioReverbFilter>().enabled = true;
			sources[i].GetComponent<AudioReverbFilter>().reverbPreset = LPFEffects[i].reverb;
		}
	}

	void TriggerHPF () {
		for (int i = 0; i < sources.Length; i++) {
			AudioHighPassFilter hpf = sources[i].GetComponent<AudioHighPassFilter>();
			hpf.enabled = true;
			hpf.cutoffFrequency = HPFEffects[i].cutoff;
			hpf.highpassResonanceQ = HPFEffects[i].resonance;
			
			sources[i].GetComponent<AudioReverbFilter>().enabled = true;
			sources[i].GetComponent<AudioReverbFilter>().reverbPreset = HPFEffects[i].reverb;

			if (HPFEffects[i].echo != null) {
				AudioEchoFilter echo = sources[i].GetComponent<AudioEchoFilter>();
				echo.enabled = true;
				echo.delay = HPFEffects[i].echo.delay;
				echo.decayRatio = HPFEffects[i].echo.decay;
				echo.dryMix = HPFEffects[i].echo.dry;
				echo.wetMix = HPFEffects[i].echo.wet;
			}
		}
	}

	public void LoadParams (TextAsset filterParams) {
		string[] line = filterParams.text.Split ('\n');
		int readLine = 0;

		do {
			string filterType = "";

			if (line[readLine].Contains ("Chorus")) {
				filterType = "Chorus";
			} else if (line[readLine].Contains ("Echo")) {
				filterType = "Echo";
			} else if (line[readLine].Contains ("LPF")) {
				filterType = "LPF";
			} else if (line[readLine].Contains ("HPF")) {
				filterType = "HPF";
			} else {
				break;
			}

			int track = 0;
			do {
				readLine++;

				if (line[readLine].Contains("#")) {
					ArrayList filter = new ArrayList(4);
					readLine++;

					while (readLine < line.Length && line[readLine].Contains(":")) {
						if (filterType == "HPF" && line[readLine].Split(':')[0].Trim() == "echo") {
						    
							if (line[readLine].Split(':')[1].Trim() == "1") {
								float[] temp = new float[4];
								readLine++;
								for (int i = 0; i < 4; i++) {
									readLine++;
									temp[i] = float.Parse (line[readLine].Split(':')[1]);
								}								
								filter.Add (new Echo(temp[0], temp[1], temp[2], temp[3]));

							} else {
								filter.Add (null);
							}
						} else {
							filter.Add (float.Parse (line[readLine].Split(':')[1]));
						}

						readLine++;
					}

					if (filterType == "Chorus" && filter.Count == 7) {
						chorusEffects[track] = new Chorus((float)filter[0], (float)filter[1],
						                                  (float)filter[2], (float)filter[3],
						                                  (float)filter[4], (float)filter[5],
						                                  (float)filter[6]);

					} else if (filterType == "Echo" && filter.Count == 4) {
						echoEffects[track] = new Echo((float)filter[0], (float)filter[1], 
						                              (float)filter[2], (float)filter[3]);

					} else if (filterType == "LPF" && filter.Count == 3) {
						LPFEffects[track] = new LPF((float)filter[0], (float)filter[1], 
						                            Convert.ToInt32 ((float)filter[2]));

					} else if (filterType == "HPF" && filter.Count == 4) {
						HPFEffects[track] = new HPF((float)filter[0], (float)filter[1],
						                            Convert.ToInt32 ((float)filter[2]),
						                            (Echo)filter[3]);

					} else {
						Debug.LogWarning("Error in parsing " + filterType + " filter");
					}					
					track++;
				}
			} while (!line[readLine].Contains ("*"));
		} while (readLine < line.Length);
	}

	class Chorus {
		public float drymix;
		public float wetmix1;
		public float wetmix2;
		public float wetmix3;
		public float delay;
		public float rate;
		public float depth;

		public Chorus (float dr, float w1, float w2, float w3, float d, float r, float h) {
			drymix = dr;
			wetmix1 = w1;
			wetmix2 = w2;
			wetmix3 = w3;
			delay = d;
			rate = r;
			depth = h;
		}
	}

	class Echo {
		public float delay;
		public float decay;
		public float wet;
		public float dry;

		public Echo (float d, float c, float w, float dr) {
			delay = d;
			decay = c;
			wet = w;
			dry = dr;
		}
	}

	class LPF {
		public float cutoff;
		public float resonance;
		public AudioReverbPreset reverb;

		public LPF (float c, float r, int v) {
			cutoff = c;
			resonance = r;
			if (v == 1) {
				reverb = AudioReverbPreset.Concerthall;
			} else {
				reverb = AudioReverbPreset.Off;
			}
		}
	}

	class HPF {
		public float cutoff;
		public float resonance;
		public AudioReverbPreset reverb;
		public Echo echo;
		
		public HPF (float c, float r, int v, Echo e) {
			cutoff = c;
			resonance = r;
			if (v == 1) {
				reverb = AudioReverbPreset.Concerthall;
			} else if (v == 2) {
				reverb = AudioReverbPreset.Drugged;
			} else {
				reverb = AudioReverbPreset.Off;
			}
			echo = e;
		}
	}
}


/************************
 * 	CHEAT SHEET
 * 
 * 	0. None
 *	1. Chorus (varies)
 *	2. Reverb Drugged 
 * 	3. Echo (varies)
 * 	4. LPF (main, varies) + Reverb Concert Hall (secondary)
 * 	5. HPF (main, varies) + Reverb and Echo (varies)
 * 
 ***********************/