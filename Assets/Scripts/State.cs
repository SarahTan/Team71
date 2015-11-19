using UnityEngine;
using System.Collections;
using System;

public class State {

	public int num;					// What state number this is
	public int numButtons;			// Total number of buttons in this state
	public int[] buttons;			// The values of the buttons in this state
	public bool allButtonsStepped = false;
	
	bool[] buttonIsStepped;	// Whether the respective buttons have been stepped on

	public State (int stateNum) {
		num = stateNum;

		switch (num) {
		case 0:
			buttons =  new int[]{0};
			numButtons = 1;
			break;
		
		case 1:
			buttons = new int[]{0, 1};
			numButtons = 2;
			break;

		case 2:
			buttons = new int[]{0, 1, 2};
			numButtons = 3;
			break;

		case 3:
			buttons = new int[]{0, 1, 2, 3};
			numButtons = 4;
			break;

		case 4:
			buttons = null;
			numButtons = 0;
			break;

		default:
			//throw new ArgumentException("State num can only be from 0 - 3.");
			break;
		}

		buttonIsStepped = new bool[numButtons];
	}

	public bool ButtonIsStepped (int buttonStepped) {
		bool stepped = false;

		for (int i = 0; i < numButtons; i ++) {
			if (buttonStepped == buttons[i] && buttonIsStepped[i]) {
				stepped = true;
				break;
			}
		}
		return stepped;
	}

	public void StepOnButton (int buttonStepped) {
		for (int i = 0; i < numButtons; i ++) {
			if (buttonStepped == buttons[i]) {
				buttonIsStepped [i] = true;
				break;
			}
		}

		foreach (bool b in buttonIsStepped) {
			if (!b) {
				allButtonsStepped = false;
				break;
			}
		}
	}

	public void StepOffButton (int buttonStepped) {
		for (int i = 0; i < numButtons; i ++) {
			if (buttonStepped == buttons[i]) {
				buttonIsStepped [i] = false;
				allButtonsStepped = false;
				break;
			}
		}
	}

	// Check if the button which is stepped on is in the state
	public bool ButtonInState (int buttonStepped) {
		foreach (int button in buttons) {
			if (buttonStepped == button) {
				return true;
			}
		}
		return false;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
