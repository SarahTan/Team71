using UnityEngine;
using System.Collections;

public class KeyboardController : MonoBehaviour {

	StateMachine stateMachine;

	int input = -1;
	bool stepDown = true;

	// Use this for initialization
	void Start () {
		stateMachine = gameObject.GetComponent<StateMachine> ();
	}
	
	// Update is called once per frame
	void Update () {
		input = -1;

		if (Input.GetKeyDown ("1")) {
			input = 0;
			stepDown = true;
			
		} else if (Input.GetKeyDown ("2")) {
			input = 1;
			stepDown = true;
			
		} else if (Input.GetKeyDown ("3")) {
			input = 2;
			stepDown = true;
			
		} else if (Input.GetKeyDown ("4")) {
			input = 3;
			stepDown = true;
			
		} else if (Input.GetKeyDown ("5")) {
			input = 4;
			stepDown = true;
			
		} else if (Input.GetKeyDown ("6")) {
			input = 5;	
			stepDown = true;

		} else if (Input.GetKeyDown ("7")) {
			input = 5;	
			stepDown = true;

		} else if (Input.GetKeyDown ("8")) {
			input = 5;	
			stepDown = true;

		} else if (Input.GetKeyUp ("1")) {
			input = 0;
			stepDown = false;
			
		} else if (Input.GetKeyUp ("2")) {
			input = 1;	
			stepDown = false;
			
		} else if (Input.GetKeyUp ("3")) {
			input = 2;	
			stepDown = false;
			
		} else if (Input.GetKeyUp ("4")) {
			input = 3;	
			stepDown = false;
			
		} else if (Input.GetKeyUp ("5")) {
			input = 4;	
			stepDown = false;
			
		} else if (Input.GetKeyUp ("6")) {
			input = 5;	
			stepDown = false;
			
		} else if (Input.GetKeyUp ("7")) {
			input = 6;	
			stepDown = false;
			
		} else if (Input.GetKeyUp ("8")) {
			input = 7;	
			stepDown = false;			
		}

		if (input != -1) {
			if (stepDown) {
				stateMachine.StepOn (input);
			} else {
				stateMachine.StepOff (input);
			}
		}
	}
}
