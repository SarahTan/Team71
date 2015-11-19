using UnityEngine;
using System.Collections;

public class StateMachine : MonoBehaviour {

	SoundManager soundMgr;
	int currentState = 0;
	int numStates = 5;
	State[] states;
	float stateLength = 8f;
	float remixLength = 10f;

	bool[] readyToTurnOff = new bool[8];
	bool[] buttonTimerInProgress = new bool[8];

	// Use this for initialization
	void Start () {
		soundMgr = GameObject.Find ("Sound Manager").GetComponent<SoundManager>();

		states = new State[numStates];
		for (int i = 0; i < numStates; i++) {
			states[i] = new State(i);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	// Because the input from the Makey Makey doesn't correspond to how we number the buttons on the mat
	int ButtonMapping (int input) {
		int button = 0;

		switch (input) {			
		case 0:
			button = 0;
			break;

		case 1:
			button = 1;
			break;

		case 2:
			button = 2;
			break;

		case 3:
			button = 3;
			break;

		case 4:
			button = 4;
			break;
		
		case 5:
			button = 5;
			break;

		case 6:
			button = 6;
			break;

		case 7:
			button = 7;
			break;

		default:
			break;
		}

		return button;
	}

	// Called at the start of each state, to flash any buttons which aren't already stepped on
	void FlashAllUnsteppedButtons () {
		for (int i = 0; i < states[currentState].numButtons; i++) {
			if (!states[currentState].ButtonIsStepped (states[currentState].buttons[i])) {
				// Call lightMgr here to flash light "states[currentState].buttons[i]"
			}
		}
	}

	// Called in State0, when correct button is stepped on
	// This class handles all state transitions
	IEnumerator StateTimer () {
		currentState++;
		while (currentState != 4) {
			yield return new WaitForSeconds(stateLength);
			currentState++;		// STATE CHANGES HERE!!!

			FlashAllUnsteppedButtons();

			if (currentState == 4 && states[currentState].allButtonsStepped) {
				soundMgr.PlayRemix();
				// Call lightMgr here to go crazy and flash everything out of sync
			} else {
				currentState = 1;
			}
		}

		if (currentState == 4) {
			yield return new WaitForSeconds(remixLength);
			currentState = 0;
			FlashAllUnsteppedButtons();
		}
	}

	IEnumerator ButtonTimer (int button) {
		readyToTurnOff[button] = false;
		buttonTimerInProgress [button] = true;
		yield return new WaitForSeconds (stateLength);

		readyToTurnOff [button] = true;
		buttonTimerInProgress[button] = false;

		// If the button isn't being stepped on after time is up, just stop music
		if (!states [currentState].ButtonIsStepped (button)) {
			soundMgr.StopMusic(button);
			// Call lightMgr here to turn off light "button"
		}
	}

	// Called by SerialInputController
	public void StepOn (int input) {
		int button = ButtonMapping (input);

		if (currentState != 4) {
			soundMgr.PlayFeedback ();

			// Check if button is in state and is not currently being stepped on
			if (states [currentState].ButtonInState (button) &&
			    	!states [currentState].ButtonIsStepped (button)) {
				states [currentState].StepOnButton (button);
				
				// Start button and state timers if required
				if (!buttonTimerInProgress [button]) {
					StartCoroutine (ButtonTimer (button));
				}			
				if (currentState == 0) {
					StartCoroutine (StateTimer ());
				}
				
				soundMgr.StartMusic (button);
				// Call lightMgr here to turn on light "button"
			}
		}
	}
	
	// Called by SerialInputController
	public void StepOff (int input) {
		int button = ButtonMapping (input);

		if (states[currentState].ButtonInState(button)) {
			states [currentState].StepOffButton(button);

			if (readyToTurnOff[button]) {
				soundMgr.StopMusic(button);
				// Call lightMgr here to turn off light "button"
			} else {
				// Call lightMgr here to flash light "button"
			}
		}
	}
	
}
