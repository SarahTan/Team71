using UnityEngine;
using System.Collections;
using System;

public class State {

	public int stateNum;					// What state number this is
	public int numButtons;			// Total number of buttons in this state
	public int[] buttons;			// The values of the buttons in this state

	public State (int num) {
		stateNum = num;

		switch (stateNum) {
		case 0:
			buttons =  new int[]{0};
			numButtons = 1;
			break;
		
		case 1:
			buttons = new int[]{0, 1, 2, 3};
			numButtons = 2;
			break;

		case 2:
			buttons = new int[]{0, 1, 2, 3, 4, 5};
			numButtons = 3;
			break;

		case 3:
			buttons = new int[]{0, 1, 2, 3, 4, 5, 6, 7};
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
}
