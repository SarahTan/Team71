using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Collections.Generic;
using System.IO;

public class SerialInputController : MonoBehaviour {

	public StateMachine stateMachine;
	public bool[] isTouched;

	public int[] ledMode;

	private SerialPort led;
	private SerialPort sp;
	string portName;
	string macPort = "/dev/cu.usbmodem1411";
	string windowsPort = "COM1";
	bool portExists = false;
	int [] touchBuffer;
	int bufferSize = 2;

	// Use this for initialization
	void Start () {
		//	For checking if the port even exists before trying to open it
		string[] ports;
		if (Application.platform == RuntimePlatform.WindowsEditor) {
			portName = windowsPort;
			ports = SerialPort.GetPortNames ();
		} else {
			portName = macPort;
			ports = GetPortNames();
		}

		if (ports.Length > 0) {
			foreach (String port in ports) {
				Debug.Log ("Found port " + port);
			
				if (port == portName) {
					portExists = true;
					sp = new SerialPort (portName, 9600);
				
					try {
						sp.Open ();	
						sp.ReadTimeout = 20;
						Debug.Log ("Successfully opened port " + portName);
					} catch (Exception e) {
						Debug.LogError ("Cannot open port '" + portName + "'! Error: " + e);
					}
				
					break;
				}
			}
		} else {
			Debug.Log("Your computer has no serial ports! You need a new one ):<");
		}

		isTouched = new bool[18];
		touchBuffer = new int[18];

		for (int i = 0; i < 18; i++) {
			touchBuffer[i] = 0;
			isTouched[i] = false;
		}	
		/////led

	}

	
	// For OSX, because it's annoying.
	string[] GetPortNames () {
		int p = (int)Environment.OSVersion.Platform;
		string[] ports = null;

		// Check if it's a Unix system
		if (p == 4 || p == 128 || p == 6) {
			ports = Directory.GetFiles ("/dev/");
		}

		return ports;
	}

	void OnApplicationQuit() {
		if (portExists) {
			sp.Close ();
		}
	}

	void updateData(bool[] newData) {
		//Debug.Log (newData.Length);

		//18input
		for (int i = 0; i < 18; i++) {
			if (newData[i] != isTouched[i]) {
				touchBuffer[i]++;
				if (touchBuffer[i] >= bufferSize) {
					touchBuffer[i] = 0;
					isTouched[i] = newData[i];
				}
			} else {
				touchBuffer[i] = 0;		
			}
		}

		//debug
		string dbg = "";
		for (int i = 0; i < 18; i++) {
			if (isTouched[i]) {
				// SEND INPUT TO STATE MACHINE HERE!


			}
			dbg += (isTouched[i] ? '1':'0');
		}
		Debug.Log (dbg);
	}

	void touchController(string[] s) {
		byte[] data = new byte[3];
		for (int i = 0; i < 3; i++) {
			data[i] = Convert.ToByte(s[i], 16);
		}

		//convert 3 byte data to 18 bool
		bool[] status = new bool[18];
		for (int j = 0; j < 2; j++) {
			for (int i = 0; i < 8; i++) {
				status [j * 8 + i] = ((data [j] >> i) & 0x01) == 1 ? true : false;
			}
		}
		status [16] = ((data [2] >> 0) & 0x01) == 1 ? true : false;
		status [17] = ((data [2] >> 1) & 0x01) == 1 ? true : false;

		updateData (status);
		//debug
		
		 string dbg = "";
		for (int i = 0; i < 18; i++) {
			dbg += (status[i] ? '1':'0');
		}
		Debug.Log (dbg);	
	}	
	
	void FixedUpdate () {
		if (portExists) {
			if (!sp.IsOpen) {
				sp.Open ();
			}

			try {
				string received = sp.ReadLine ();
				Debug.Log(received);
				string[] words = sp.ReadLine ().Split ('\t', '\r');
				//Debug.Log(sp.ReadLine());

				if (words.Length == 4) {
					touchController (words);//the last word is return
				}
			} catch (Exception e) {
				if (Application.platform != RuntimePlatform.WindowsEditor) {
					Debug.LogError ("Cannot read from port '" + portName + "'! Error: " + e);
				}
			}

			sp.Close ();
		}		
	}
}
