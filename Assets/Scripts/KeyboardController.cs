using UnityEngine;
using System.Collections;

public class KeyboardController : MonoBehaviour {

	StateMachine stateMachine;

	// Use this for initialization
	void Start () {
		stateMachine = gameObject.GetComponent<StateMachine> ();
	}
	
	// Update is called once per frame
	void Update () {
		int input = -1;
		bool stepDown = true;

		if (Input.GetKeyDown ("0")) {
			input = 11;
			
		} else if (Input.GetKeyDown ("1")) {
			input = 10;
			
		} else if (Input.GetKeyDown ("2")) {
			input = 2;
			
		} else if (Input.GetKeyDown ("3")) {
			input = 9;
			
		} else if (Input.GetKeyDown ("4")) {
			input = 8;
			
		} else if (Input.GetKeyDown ("5")) {
			input = 3;

		} else if (Input.GetKeyDown ("6")) {
			input = 17;

		} else if (Input.GetKeyDown ("7")) {
			input = 16;

		} else if (Input.GetKeyDown ("a")) {
			input = 0;
			
		} else if (Input.GetKeyDown ("s")) {
			input = 15;
			
		} else if (Input.GetKeyDown ("d")) {
			input = 14;
			
		} else if (Input.GetKeyDown ("f")) {
			input = 13;
			
		} else if (Input.GetKeyDown ("g")) {
			input = 12;
		}
		SendInput (input, stepDown);

		input = -1;
		stepDown = false;
		if (Input.GetKeyUp ("0")) {
			input = 11;

		} else if (Input.GetKeyUp ("1")) {
			input = 10;
			
		} else if (Input.GetKeyUp ("2")) {
			input = 2;
			
		} else if (Input.GetKeyUp ("3")) {
			input = 9;
			
		} else if (Input.GetKeyUp ("4")) {
			input = 8;
			
		} else if (Input.GetKeyUp ("5")) {
			input = 3;
			
		} else if (Input.GetKeyUp ("6")) {
			input = 17;
			
		} else if (Input.GetKeyUp ("7")) {
			input = 16;		
		}
		SendInput (input, stepDown);

	}

	void SendInput (int input, bool stepDown) {
		if (input != -1) {
			if (stepDown) {
				stateMachine.StepOn (input);
			} else {
				stateMachine.StepOff (input);
			}
		}
	}
}
