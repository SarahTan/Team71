﻿using UnityEngine;
using System.Collections;
using System;

public class StateMachine : MonoBehaviour {

	SoundManager soundMgr;
	LightController lightCtrl;
	SerialInputController serialInput;
	int currentState = 0;
	int numStates = 5;
	State[] states;
	float stateLength = 8f;
	float remixLength = 21f;

	int numButtons = 8;
	bool[] readyToTurnOff;
	bool[] buttonTimerInProgress;
	bool[] buttonIsStepped;

	// Use this for initialization
	void Start () {
		readyToTurnOff = new bool[numButtons];
		for (int i = 0; i < numButtons; i++) {
			readyToTurnOff[i] = true;
		}

		buttonTimerInProgress = new bool[numButtons];
		buttonIsStepped = new bool[numButtons];

		soundMgr = GameObject.Find ("Sound Manager").GetComponent<SoundManager>();
		lightCtrl = GameObject.Find ("Light Controller").GetComponent<LightController> ();
		serialInput = GameObject.Find ("MK").GetComponent<SerialInputController> ();

		states = new State[numStates];
		for (int i = 0; i < numStates; i++) {
			states[i] = new State(i);
		}
		FlashAllUnplayedButtons ();
	}
	
	// Update is called once per frame
	void Update () {

	}


	// Because the input from the Makey Makey doesn't correspond to how we number the buttons on the mat
	int ButtonMapping (int input) {
		int button = 0;

		switch (input) {	
		// Floor buttons
		case 11:
			button = 0;
			break;

		case 10:
			button = 1;
			break;

		case 2:
			button = 2;
			break;

		case 9:
			button = 3;
			break;

		case 8:
			button = 4;
			break;
		
		case 3:
			button = 5;
			break;

		case 17:
			button = 6;
			break;

		case 16:
			button = 7;
			break;

		// Orb buttons
		case 0:
			button = 8;
			break;

		case 15:
			button = 9;
			break;

		case 14:
			button = 10;
			break;

		case 13:
			button = 11;
			break;

		case 12:
			button = 12;
			break;

		default:
			button = -1;
			break;
		}

		return button;
	}

	// Called at the start of each state, to flash any buttons which aren't already stepped on
	void FlashAllUnplayedButtons () {
		string str = "State " + currentState + ", unplayed buttons: ";
		for (int i = 0; i < buttonIsStepped.Length; i++) {
			if (buttonIsStepped[i]) {
				lightCtrl.TurnOnLED(i);
			} else if (!buttonIsStepped[i] && readyToTurnOff[i] && states[currentState].ButtonInState(i)) {
				lightCtrl.FlashLED (states[currentState].buttons[i]);
				string tempStr = i + ", ";
				str += tempStr;
			}
		}
		Debug.Log (str);
	}

	bool AllButtonsStepped () {
		foreach (bool steppedOn in buttonIsStepped) {
			if (!steppedOn) {
				return false;
			}
		}
		return true;
	}

	// Called in State0, when correct button is stepped on
	// This class handles all state transitions
	IEnumerator StateTimer () {
		for (int i = 1; i <= 3; i++) {
			if (buttonIsStepped[i]) {
				StartCoroutine(ButtonTimer(i));
				soundMgr.StartMusic(i);
			}
		}

		// State 1, 2, 3
		while (currentState < 3) {
			currentState++;
			FlashAllUnplayedButtons();
			yield return new WaitForSeconds(stateLength);
		}

		while (!(AllButtonsStepped () || Input.GetKey (KeyCode.R))) {
			yield return new WaitForSeconds (1f);
		}

		Debug.Log("state: remix");
		currentState++;
		soundMgr.PlayRemix();
		lightCtrl.RemixLED ();
		serialInput.remix (Convert.ToByte(1));
		
		yield return new WaitForSeconds(remixLength);
		ResetStates (true);
	}

	IEnumerator ButtonTimer (int button) {
		readyToTurnOff[button] = false;
		buttonTimerInProgress [button] = true;

		yield return new WaitForSeconds (stateLength);

		readyToTurnOff [button] = true;
		buttonTimerInProgress[button] = false;

		// If the button isn't being stepped on after time is up, just stop music
		if (!buttonIsStepped[button] && currentState != 4) {			
			soundMgr.StopMusic(button);

			if (states[currentState].ButtonInState(button)) {
				lightCtrl.FlashLED (button);
			} else {
				lightCtrl.TurnOffLED (button);
			}
		} 
	}

	// Also called by KeyboardController when Q/W is pressed
	public void ResetStates (bool changeSong) {
		soundMgr.StopRemix(changeSong);
		serialInput.remix (Convert.ToByte(2));
		currentState = 0;
		for (int i = 0; i < numButtons; i++) {
			lightCtrl.TurnOffLED(i);
			readyToTurnOff[i] = true;
		}
		StopAllCoroutines ();

		if (buttonIsStepped [0]) {
			soundMgr.StartMusic(0);
			lightCtrl.TurnOnLED(0);
			StartCoroutine (ButtonTimer(0));
			StartCoroutine(StateTimer());
		} else {
			FlashAllUnplayedButtons();
		}
	}

	// Called by SerialInputController
	public void StepOn (int input) {
		int button = ButtonMapping (input);

		if (button >= 0 && button <= 7) {
			buttonIsStepped [button] = true;

			if (currentState != 4) {
				soundMgr.PlayTrackFeedback ();

				// Check if button is in state and is not currently being stepped on
				if (states [currentState].ButtonInState (button) && buttonIsStepped [button]) {
				
					// Start timers
					if (!buttonTimerInProgress [button]) {
						StartCoroutine (ButtonTimer (button));
					}			
					if (currentState == 0) {
						StartCoroutine (StateTimer ());
					}
				
					soundMgr.StartMusic (button);
					lightCtrl.TurnOnLED (button);
				}
			}
		} else if (button >= 8 && button <= 12) {
			soundMgr.TriggerFilter(button);
		}
	}
	
	// Called by SerialInputController
	public void StepOff (int input) {
		int button = ButtonMapping (input);

		if (button >= 0 && button <= 7) {
			buttonIsStepped [button] = false;

			if (readyToTurnOff [button]) {
				soundMgr.StopMusic (button);
			}

			if (states [currentState].ButtonInState (button)) {
				//lightCtrl.FlashLED (button);
			} else {
				lightCtrl.TurnOffLED (button);
			}
		}
	}
	
}
