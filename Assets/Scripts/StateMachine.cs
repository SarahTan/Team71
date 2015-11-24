using UnityEngine;
using System.Collections;

public class StateMachine : MonoBehaviour {

	SoundManager soundMgr;
	LightController lightCtrl;
	int currentState = 0;
	int numStates = 5;
	State[] states;
	float stateLength = 12f;
	float remixLength = 20f;

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
		// State 1, 2, 3
		while (currentState < 3) {
			currentState++;
			FlashAllUnsteppedButtons();
			yield return new WaitForSeconds(stateLength);
		}

		while (!(AllButtonsStepped () || Input.GetKey (KeyCode.R))) {
			yield return new WaitForSeconds (1f);
		}

		Debug.Log("state: remix");
		currentState++;
		soundMgr.PlayRemix();
		lightCtrl.RemixLED ();
		
		yield return new WaitForSeconds(remixLength);
		soundMgr.StopRemix ();
		currentState = 0;
		for (int i = 0; i < numButtons; i++) {
			lightCtrl.TurnOffLED(i);
		}
		FlashAllUnsteppedButtons();
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
		}

		if (states[currentState].ButtonInState(button)) {
			lightCtrl.FlashLED (button);
		} else {
			lightCtrl.TurnOffLED (button);
		}
	}
	
}
