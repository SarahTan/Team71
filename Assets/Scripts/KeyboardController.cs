﻿using UnityEngine;
using System.Collections;

public class KeyboardController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		int input = -1;

		if (Input.GetKeyDown ("1")) {
			input = 0;
			
		} else if (Input.GetKeyDown ("2")) {
			input = 1;
			
		} else if (Input.GetKeyDown ("3")) {
			input = 2;
			
		} else if (Input.GetKeyDown ("4")) {
			input = 3;
			
		} else if (Input.GetKeyDown ("5")) {
			input = 4;
			
		} else if (Input.GetKeyDown ("6")) {
			input = 5;	

		} else if (Input.GetKeyDown ("7")) {
			input = 5;	

		} else if (Input.GetKeyDown ("8")) {
			input = 5;	

		} else if (Input.GetKeyUp ("1")) {
			input = 0;	
			
		} else if (Input.GetKeyUp ("2")) {
			input = 1;	
			
		} else if (Input.GetKeyUp ("3")) {
			input = 2;	
			
		} else if (Input.GetKeyUp ("4")) {
			input = 3;	
			
		} else if (Input.GetKeyUp ("5")) {
			input = 4;	
			
		} else if (Input.GetKeyUp ("6")) {
			input = 5;	
			
		} else if (Input.GetKeyUp ("7")) {
			input = 6;	
			
		} else if (Input.GetKeyUp ("8")) {
			input = 7;	
			
		}
	}
}