﻿using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Collections.Generic;
using System.IO;

public class LightController : MonoBehaviour {

	private byte[] ledMode={6,6,6,6,6,6,6,6};
	
	private SerialPort led;
	public bool[] isTouched;
	//public int[] ledMode;
	string portName;
	string macPort = "/dev/cu.usbmodem1421";
	string windowsPort = "COM1";
	bool portExists = false;
	
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
			Debug.Log("Your computer has no serial ports! You need a new one ):<");
		}
		
		InvokeRepeating ("sendLEDStatus",0, 0.2f);
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
		byte[] data=new byte[8];
		for (int i=0; i<8; i++) {
			//index
			data[i]=(byte)i;
			data[i]=(byte)((data[i]<<4)&0xF0);

			//mode
			data[i]+= ledMode[i];
			Debug.Log(ledMode[i]);
		}
		if(!led.IsOpen){
			led.Open();
		}
		try {
			led.Write(data,0,8);
		} catch (Exception e) {
			Debug.LogError ("Cannot  write port! Error: " + e);
		}
		
		
	}

}