using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Collections.Generic;
using System.IO;

public class LightController : MonoBehaviour {

	private byte[] ledMode = {0,0,0,0,0,0,0,0};
	
	private SerialPort led;
	public bool[] isTouched;
	//public int[] ledMode;
	string portName = "/dev/cu.usbmodem1421";
	bool portExists = false;
	
	// Use this for initialization
	void Start () {
		//	For checking if the port even exists before trying to open it
		string[] ports;
		if (IsOSX()) {
			ports = GetPortNames();

			if (ports.Length > 0) {
				foreach (String port in ports) {
					Debug.Log ("Found port " + port);
					
					if (port == portName) {
						portExists = true;
						led = new SerialPort (portName, 9600);
						
						try {
							led.Open ();	
							led.ReadTimeout = 20;
							Debug.Log ("Successfully opened ledport " + portName);
						} catch (Exception e) {
							Debug.LogError ("Cannot open ledport '" + portName + "'! Error: " + e);
						}
						
						break;
					}
				}
			} else {
				Debug.Log("Your computer has no serial ports!");
			}
		}
		
		InvokeRepeating ("sendLEDStatus", 0, 0.2f);
	}

	bool IsOSX () {
		if (Application.platform == RuntimePlatform.OSXEditor ||
		    Application.platform == RuntimePlatform.OSXPlayer ||
		    Application.platform == RuntimePlatform.OSXDashboardPlayer) {
			return true;
		}
		return false;
	}
	
	// For OSX, because it's annoying.
	string[] GetPortNames () {
		int p = (int)Environment.OSVersion.Platform;
		string[] ports=null;
		
		// Check if it's a Unix system
		if (p == 4 || p == 128 || p == 6) {
			ports = Directory.GetFiles ("/dev/");
		}
		
		return ports;
	}
	
	void OnApplicationQuit() {
		if (portExists) {
			led.Close ();
		}
	}

	void sendLEDStatus(){
		byte[] data = new byte[8];
		for (int i = 0; i < 8; i++) {
			//index
			data[i]=(byte)i;
			data[i]=(byte)((data[i]<<4)&0xF0);

			//mode
			data[i]+= ledMode[i];
//			Debug.Log(ledMode[i]);
		}

		if (portExists) {
			if (!led.IsOpen) {
				led.Open ();
			}

			try {
				led.Write (data, 0, 8);
			} catch (Exception e) {
				Debug.LogError ("Cannot write to port! Error: " + e);
			}		
		}
	}

	public void TurnOnLED (int button) {
		Debug.Log ("LED " + button + " turned on");
		if (button > 7) {
			Debug.Log ("TurnOn : wrong index for LED button "+button);
			return;
		}
		ledMode [button] = 1;
	}

	public void FlashLED (int button) {		
		Debug.Log ("LED " + button + " flashing");
		if (button > 7) {
			Debug.Log ("Flash : wrong index for LED button "+button);
			return;
		}
		ledMode [button] = 2;
	}

	public void TurnOffLED (int button) {		
		Debug.Log ("LED " + button + " turned off");
		if (button > 7) {
			Debug.Log ("TurnOff : wrong index for LED button "+button);
			return;
		}
		ledMode [button] = 0;
	}

	public void RemixLED () {
		Debug.Log ("LEDs remixing!");
		for(int i=0;i<8;i++)
		ledMode [i] = 6;
	}

}
