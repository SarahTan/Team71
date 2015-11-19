using UnityEngine;
using System.Collections;

public class StateMachine : MonoBehaviour {

	SoundManager soundMgr;
	int currentState = 0;
	int numStates = 5;
	State[] states;
	float stateLength = 8f;
	float remixLength = 10f;

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

	// Called in State0, when correct button is stepped on
	// This class handles all state transitions
	IEnumerator Timer () {
		currentState++;
		while (currentState != 4) {
			yield return new WaitForSeconds(stateLength);
			currentState++;

			// reset music to 0 if buttons arent stepped on

			if (currentState == 4 && states[currentState].allButtonsStepped) {
				soundMgr.PlayRemix();
				// Call lightMgr here
			} else {
				currentState = 1;
			}
		}

		if (currentState == 4) {
			yield return new WaitForSeconds(remixLength);
			currentState = 0;
		}
	}

	// Called by SerialInputController
	public void StepOn (int input) {
		int button = ButtonMapping (input);
		soundMgr.PlayFeedback ();

		if (states[currentState].ButtonInState(button)) {
			states [currentState].StepOnButton(button);

			soundMgr.StartMusic(button);
			// Call LightMgr here

			if (currentState == 0) {
				StartCoroutine(Timer());
			}
		}
	}
	
	// Called by SerialInputController
	public void StepOff (int input) {
		int button = ButtonMapping (input);

		if (states[currentState].ButtonInState(button)) {
			states [currentState].StepOffButton(button);

			// Check if 8s is up here
			soundMgr.StopMusic(button);
			// Call lightMgr here
		}
	}
	
}
