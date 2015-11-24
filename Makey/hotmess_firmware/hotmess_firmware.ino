/*
 ************************************************
 ************** MAKEY MAKEY *********************
 ************************************************
 
 /////////////////////////////////////////////////
 /////////////HOW TO EDIT THE KEYS ///////////////
 /////////////////////////////////////////////////
 - Edit keys in the settings.h file
 - that file should be open in a tab above (in Arduino IDE)
 - more instructions are in that file
 
 ////////////////////////////////////////////////
 //////// MaKey MaKey FIRMWARE v1.4 /////////////
 ////////////////////////////////////////////////
 by: Eric Rosenbaum, Jay Silver, and Jim Lindblom
 MIT Media Lab & Sparkfun
 start date: 2/16/2012 
 current release: 7/5/2012
 */

/////////////////////////
// DEBUG DEFINITIONS ////               
/////////////////////////
//#define DEBUG
//#define DEBUG2 
//#define DEBUG3 
//#define DEBUG_TIMING
//#define DEBUG_TIMING2

////////////////////////
// DEFINED CONSTANTS////
////////////////////////

#define BUFFER_LENGTH    3     // 3 bytes gives us 24 samples
#define NUM_INPUTS       18    // 6 on the front + 12 on the back
#define NUM_OUTPUTKEYS   18   // 
//#define TARGET_LOOP_TIME 694   // (1/60 seconds) / 24 samples = 694 microseconds per sample 
//#define TARGET_LOOP_TIME 758  // (1/55 seconds) / 24 samples = 758 microseconds per sample 
#define TARGET_LOOP_TIME 744  // (1/56 seconds) / 24 samples = 744 microseconds per sample 

#include "settings.h"
#include "dance.h"
/////////////////////////
// STRUCT ///////////////
/////////////////////////
typedef struct {
  byte pinNumber;
  int keyCode;
  byte measurementBuffer[BUFFER_LENGTH]; 
  boolean oldestMeasurement;
  byte bufferSum;
  boolean pressed;
  boolean prevPressed;
} MakeyMakeyInput;

MakeyMakeyInput inputs[NUM_INPUTS];
MakeyMakeyInput outputKeys[NUM_OUTPUTKEYS];

///////////////////////////////////
// VARIABLES //////////////////////
///////////////////////////////////
int bufferIndex = 0;
byte byteCounter = 0;
byte bitCounter = 0;

int pressThreshold;
int releaseThreshold;
boolean inputChanged;

// Pin Numbers
// input pin numbers for kickstarter production board
int pinNumbers[NUM_INPUTS] = {
  12, 8, 13, 15, 7, 6,     // top of makey makey board
  5, 4, 3, 2, 1, 0,        // left side of female header, KEBYBOARD
  23, 22, 21, 20, 19, 18   // right side of female header, MOUSE
};

// timing
int loopTime = 0;
int prevTime = 0;
int loopCounter = 0;


///////////////////////////
// FUNCTIONS //////////////
///////////////////////////
void updateMeasurementBuffers();
void updateBufferSums();
void updateBufferIndex();
void updateInputStates();
void updateOutputStates();
void addDelay();
void cycleLEDs();
void updateOutLEDs();

////////////////////
// MAIN LOOP ///////
////////////////////
void loop() 
{
  updateMeasurementBuffers();
  updateBufferSums();
  updateBufferIndex();
  updateInputStates();
  updateOutputStates();
  cycleLEDs();
  updateOutLEDs();
  addDelay();
  //delay(20);
}

//////////////////////////
// INITIALIZE ARDUINO
//////////////////////////
void initializeArduino() {
#ifdef DEBUG
  Serial.begin(9600);  // Serial for debugging
#endif

  /* Set up input pins 
   DEactivate the internal pull-ups, since we're using external resistors */
  for (int i=0; i<NUM_INPUTS; i++)
  {
    pinMode(pinNumbers[i], INPUT);
    digitalWrite(pinNumbers[i], LOW);
  }

  pinMode(inputLED_a, INPUT);
  pinMode(inputLED_b, INPUT);
  pinMode(inputLED_c, INPUT);
  digitalWrite(inputLED_a, LOW);
  digitalWrite(inputLED_b, LOW);
  digitalWrite(inputLED_c, LOW);

  pinMode(outputK, OUTPUT);
  digitalWrite(outputK, LOW);

//#ifdef DEBUG
  delay(4000); // allow us time to reprogram in case things are freaking out
//#endif

  //Keyboard.begin();
}

///////////////////////////
// INITIALIZE INPUTS
///////////////////////////
void initializeInputs() {

  float thresholdPerc = SWITCH_THRESHOLD_OFFSET_PERC;
  float thresholdCenterBias = SWITCH_THRESHOLD_CENTER_BIAS/50.0;
  float pressThresholdAmount = (BUFFER_LENGTH * 8) * (thresholdPerc / 100.0);
  float thresholdCenter = ( (BUFFER_LENGTH * 8) / 2.0 ) * (thresholdCenterBias);
  pressThreshold = int(thresholdCenter + pressThresholdAmount);
  releaseThreshold = int(thresholdCenter - pressThresholdAmount);

#ifdef DEBUG
  Serial.println(pressThreshold);
  Serial.println(releaseThreshold);
#endif

  for (int i=0; i<NUM_INPUTS; i++) {
    inputs[i].pinNumber = pinNumbers[i];
    inputs[i].keyCode = keyCodes[i];
    outputKeys[i].keyCode=keyCodes[i];

    for (int j=0; j<BUFFER_LENGTH; j++) {
      inputs[i].measurementBuffer[j] = 0;
    }
    inputs[i].oldestMeasurement = 0;
    inputs[i].bufferSum = 0;
    inputs[i].pressed = false;
    inputs[i].prevPressed = false;
#ifdef DEBUG
    Serial.println(i);
#endif
  }
  
  /*
  for (int inputGroup=0; inputGroup<6; inputGroup++) {
    int inputOffset = inputGroup * 3;
    // The outputs will be based on the first of each group of three keys.
    // For example, if keyCodes[0] is 'a' then the outputs will be:
    // 'a' if just inputs[0] is held
    // 'b' if just inputs[1] is held
    // 'c' if just inputs[2] is held
    // 'd' if inputs[0] and inputs[1] are held
    // 'e' if inputs[0] and inputs[2] are held
    // 'f' if inputs[1] and inputs[2] are held
    // 'g' if inputs[0] and inputs[1] and inputs[2] are held
    // Unfortunately we have two special cases left over since the sequence from input 'v' exceeds 'z' by two characters.
    // It seems the hardware is outputting multiple keys as Shift+[ to get '{' ('z'+1) and Shift+'\' to get '|' ('z'+2).
    // To combat this, we'll replace 'z'+1 with '8' and 'z'+2 with '9' and translate these special cases in decompression.
    for (int outputOffset=0; outputOffset<7; outputOffset++) {
      outputKeys[(inputGroup*7)+outputOffset].keyCode = keyCodes[inputOffset] + outputOffset;
      if (outputKeys[(inputGroup*7)+outputOffset].keyCode == 'z'+1)
        outputKeys[(inputGroup*7)+outputOffset].keyCode = '8';
      if (outputKeys[(inputGroup*7)+outputOffset].keyCode == 'z'+2)
        outputKeys[(inputGroup*7)+outputOffset].keyCode = '9';
    }
  }
  */
}


//////////////////////////////
// UPDATE MEASUREMENT BUFFERS
//////////////////////////////
void updateMeasurementBuffers() {

  for (int i=0; i<NUM_INPUTS; i++) {

    // store the oldest measurement, which is the one at the current index,
    // before we update it to the new one 
    // we use oldest measurement in updateBufferSums
    byte currentByte = inputs[i].measurementBuffer[byteCounter];
    inputs[i].oldestMeasurement = (currentByte >> bitCounter) & 0x01; 

    // make the new measurement
    boolean newMeasurement = digitalRead(inputs[i].pinNumber);

    // invert so that true means the switch is closed
    newMeasurement = !newMeasurement; 

    // store it    
    if (newMeasurement) {
      currentByte |= (1<<bitCounter);
    } 
    else {
      currentByte &= ~(1<<bitCounter);
    }
    inputs[i].measurementBuffer[byteCounter] = currentByte;
  }
}

///////////////////////////
// UPDATE BUFFER SUMS
///////////////////////////
void updateBufferSums() {

  // the bufferSum is a running tally of the entire measurementBuffer
  // add the new measurement and subtract the old one

  for (int i=0; i<NUM_INPUTS; i++) {
    byte currentByte = inputs[i].measurementBuffer[byteCounter];
    boolean currentMeasurement = (currentByte >> bitCounter) & 0x01; 
    if (currentMeasurement) {
      inputs[i].bufferSum++;
    }
    if (inputs[i].oldestMeasurement) {
      inputs[i].bufferSum--;
    }
  }  
}

///////////////////////////
// UPDATE BUFFER INDEX
///////////////////////////
void updateBufferIndex() {
  bitCounter++;
  if (bitCounter == 8) {
    bitCounter = 0;
    byteCounter++;
    if (byteCounter == BUFFER_LENGTH) {
      byteCounter = 0;
    }
  }
}

///////////////////////////
// UPDATE INPUT STATES
///////////////////////////
void updateInputStates() {
  inputChanged = false;
  for (int i=0; i<NUM_INPUTS; i++) {
    inputs[i].prevPressed = inputs[i].pressed; // store previous pressed state
    if (inputs[i].pressed) {
      if (inputs[i].bufferSum < releaseThreshold) {  
        inputChanged = true;
        inputs[i].pressed = false;
        //Keyboard.release(inputs[i].keyCode);
      }
    } 
    else if (!inputs[i].pressed) {
      if (inputs[i].bufferSum > pressThreshold) {  // input becomes pressed
        inputChanged = true;
        inputs[i].pressed = true; 
        //Keyboard.press(inputs[i].keyCode);
      }
    }
  }
}

void updateOutputStates() {
  // Translate real inputs into the 'compressed' key combos.
  // First keep track of the previous states, so we'll be able to tell which ones are changing this round.
  // Also assume any key is NOT currently held until we detect the related key combo is held in real inputs.
  byte outputArray[3];
  outputArray[0]=0;
  outputArray[1]=0;
  outputArray[2]=0;
  int arrayIndex=0;
  for (int i=0; i<NUM_OUTPUTKEYS; i++) {
  	if(i>7 && i<16)arrayIndex=1;
        if(i>15)arrayIndex=2;
    outputKeys[i].prevPressed = outputKeys[i].pressed;
    if(inputs[i].pressed)
    {
        
    	outputKeys[i].pressed = true;
        
    	outputArray[arrayIndex]<<=1;
    	outputArray[arrayIndex]|=0x01;
    }
    else
    {			  
    	outputKeys[i].pressed = false;
        
    	outputArray[arrayIndex]<<=1;
    	//outputArray[arrayIndex]|=0x00;
    }
    
    /*
    Serial.print(outputArray[0], HEX);
    Serial.print(outputArray[1], HEX);
    Serial.println(outputArray[2], HEX);
    */
    //Serial.write(outputArray[0]);
    
    
  }
  
    Serial.print(outputArray[0], HEX);
    Serial.print('\t');
    Serial.print(outputArray[1], HEX);
    Serial.print('\t');
    Serial.println(outputArray[2], HEX);
  
  //Serial.write(outputArray,3);
  Serial.flush();
  //key
    for (int i=0; i<NUM_OUTPUTKEYS; i++) {
    if (outputKeys[i].keyCode!=0 && outputKeys[i].prevPressed && !outputKeys[i].pressed) {
      Keyboard.release(outputKeys[i].keyCode);
    }
  }
  
  // Enable any presses that are now freshly valid.
  for (int i=0; i<NUM_OUTPUTKEYS; i++) {
    if (outputKeys[i].keyCode!=0 && !outputKeys[i].prevPressed && outputKeys[i].pressed) {
      Keyboard.press(outputKeys[i].keyCode);
    }
  }
  

}

void updateOutLEDs()
{
  boolean keyPressed = 0;
  for (int i=0; i<NUM_INPUTS; i++)
  {
    if (inputs[i].pressed)
    {
      keyPressed = 1;
    }
  }

  if (keyPressed)
  {
    digitalWrite(outputK, HIGH);
    TXLED1;
  }
  else
  {
    digitalWrite(outputK, LOW);
    TXLED0;
  }
}

///////////////////////////
// ADD DELAY
///////////////////////////
void addDelay() {

  loopTime = micros() - prevTime;
  if (loopTime < TARGET_LOOP_TIME) {
    int wait = TARGET_LOOP_TIME - loopTime;
    delayMicroseconds(wait);
    
  }

  prevTime = micros();

#ifdef DEBUG_TIMING
  if (loopCounter == 0) {
    int t = micros()-prevTime;
    Serial.println(t);
  }
  loopCounter++;
  loopCounter %= 999;
#endif
}

///////////////////////////
// CYCLE LEDS
///////////////////////////
void cycleLEDs() {
  pinMode(inputLED_a, INPUT);
  pinMode(inputLED_b, INPUT);
  pinMode(inputLED_c, INPUT);
  digitalWrite(inputLED_a, LOW);
  digitalWrite(inputLED_b, LOW);
  digitalWrite(inputLED_c, LOW);

  ledCycleCounter++;
  ledCycleCounter %= 6;

  if ((ledCycleCounter == 0) && inputs[0].pressed) {
    pinMode(inputLED_a, INPUT);
    digitalWrite(inputLED_a, HIGH);
    pinMode(inputLED_b, OUTPUT);
    digitalWrite(inputLED_b, HIGH);
    pinMode(inputLED_c, OUTPUT);
    digitalWrite(inputLED_c, LOW);
  }
  if ((ledCycleCounter == 1) && inputs[1].pressed) {
    pinMode(inputLED_a, OUTPUT);
    digitalWrite(inputLED_a, HIGH);
    pinMode(inputLED_b, OUTPUT);
    digitalWrite(inputLED_b, LOW);
    pinMode(inputLED_c, INPUT);
    digitalWrite(inputLED_c, LOW);

  }
  if ((ledCycleCounter == 2) && inputs[2].pressed) {
    pinMode(inputLED_a, OUTPUT);
    digitalWrite(inputLED_a, LOW);
    pinMode(inputLED_b, OUTPUT);
    digitalWrite(inputLED_b, HIGH);
    pinMode(inputLED_c, INPUT);
    digitalWrite(inputLED_c, LOW);
  }
  if ((ledCycleCounter == 3) && inputs[3].pressed) {
    pinMode(inputLED_a, INPUT);
    digitalWrite(inputLED_a, LOW);
    pinMode(inputLED_b, OUTPUT);
    digitalWrite(inputLED_b, LOW);
    pinMode(inputLED_c, OUTPUT);
    digitalWrite(inputLED_c, HIGH);
  }
  if ((ledCycleCounter == 4) && inputs[4].pressed) {
    pinMode(inputLED_a, OUTPUT);
    digitalWrite(inputLED_a, LOW);
    pinMode(inputLED_b, INPUT);
    digitalWrite(inputLED_b, LOW);
    pinMode(inputLED_c, OUTPUT);
    digitalWrite(inputLED_c, HIGH);
  }
  if ((ledCycleCounter == 5) && inputs[5].pressed) {
    pinMode(inputLED_a, OUTPUT);
    digitalWrite(inputLED_a, HIGH);
    pinMode(inputLED_b, INPUT);
    digitalWrite(inputLED_b, LOW);
    pinMode(inputLED_c, OUTPUT);
    digitalWrite(inputLED_c, LOW);
  }
}
