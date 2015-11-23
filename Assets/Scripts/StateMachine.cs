using UnityEngine;
using System.Collections;

public class StateMachine : MonoBehaviour {

	SoundManager soundMgr;
	LightController lightCtrl;
	int currentState = 0;
	int numStates = 5;
	State[] states;
	float stateLength = 8f;
	float remixLength = 10f;

	int numButtons = 8;
	bool[] readyToTurnOff;
	bool[] buttonTimerInProgress;
	bool[] buttonIsStepped;

	// Use this for initialization
	void Start () {
		readyToTurnOff = new bool[numButtons];
		buttonTimerInProgress = new bool[numButtons];
		buttonIsStepped = new bool[numButtons];

		soundMgr = GameObject.Find ("Sound Manager").GetComponent<SoundManager>();
		lightCtrl = GameObject.Find ("Light Controller").GetComponent<LightController> ();

		states = new State[numStates];
		for (int i = 0; i < numStates; i++) {
			states[i] = new State(i);
		}
		FlashAllUnsteppedButtons ();
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
		string str = "State " + currentState + ", unstepped buttons: ";
		for (int i = 0; i < buttonIsStepped.Length; i++) {
			if (!buttonIsStepped[i] && states[currentState].ButtonInState(i)) {
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
		currentState++;

		while (currentState != 4) {
			yield return new WaitForSeconds(stateLength);
			currentState++;		// STATE CHANGES HERE!!!

			if (currentState != 4) {
				FlashAllUnsteppedButtons();
			} else {
				if (AllButtonsStepped ()) {
					Debug.Log("state: remix");
					soundMgr.PlayRemix();
					lightCtrl.RemixLED ();
				} else {
					currentState = 1;
					FlashAllUnsteppedButtons();
				}
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
		if (!buttonIsStepped[button]) {
			soundMgr.StopMusic(button);
			lightCtrl.TurnOffLED (button);
		}
	}

	// Called by SerialInputController
	public void StepOn (int input) {
		int button = ButtonMapping (input);
		buttonIsStepped[button] = true;

		if (currentState != 4) {
			soundMgr.PlayFeedback ();

			// Check if button is in state and is not currently being stepped on
			if (states [currentState].ButtonInState (button) && buttonIsStepped[button]) {
				
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
	}
	
	// Called by SerialInputController
	public void StepOff (int input) {
		int button = ButtonMapping (input);
		buttonIsStepped[button] = false;

		if (readyToTurnOff[button]) {
			soundMgr.StopMusic(button);
			lightCtrl.TurnOffLED (button);
		} else if (states[currentState].ButtonInState(button)) {
			lightCtrl.FlashLED (button);
		}
	}
	
}
