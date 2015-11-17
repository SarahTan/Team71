using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
public class SerialInputController : MonoBehaviour {
	public SoundManager soundMgr;
	public bool[] isTouched;

	private SerialPort sp = new SerialPort("/dev/cu.usbmodem1411",9600);
	//private SerialPort sp;

	char[] byteArray;
	int [] touchBuffer;
	int bufferSize = 2;

	// Use this for initialization
	void Start () {
		byteArray = new char[3];
		isTouched = new bool[18];
		touchBuffer = new int[18];
		for (int i = 0; i < 18; i++) {
			touchBuffer[i] = 0;
			isTouched[i] = false;
		}

		sp.Open ();	
		sp.ReadTimeout = 20;		
	}

	void OnApplicationQuit() {
		sp.Close ();
	}

	void updateData(bool[] newData) {
		//Debug.Log (newData.Length);

		//18input
		for (int i = 0; i < 18; i++) {
			if(newData[i] != isTouched[i]){
				touchBuffer[i]++;
				if(touchBuffer[i] >= bufferSize){
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
			if(isTouched[i]) {
				soundMgr.SendInput(i);
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
		/*
		 * string dbg="";
		for (int i=0; i<18; i++) {
			dbg+=(status[i]?'1':'0');
		}
				Debug.Log (dbg);
		 * */

	}
	
	
	void FixedUpdate () {
		if (!sp.IsOpen) {
			sp.Open ();
		}

		if (sp != null && sp.IsOpen) {
			try {
				string received=sp.ReadLine();
				//Debug.Log(received);
				string[] words=sp.ReadLine().Split('\t','\r');
				//Debug.Log(sp.ReadLine());

				if(words.Length==4) {
					touchController(words);//the last word is return
				}
			} catch (System.Exception) {
				
			}
			sp.Close();
			
		} else {
			sp.Open();
		}	
		
	}
}
