////constants
#define NUM_INPUTS  8
#define FLASHING_SPEED  400
#define DELAY_TIME    20
////variable
int ports[NUM_INPUTS]={
  2,3,4,5,6,7,8,9
};
int flashingSpeed[NUM_INPUTS];
bool LEDCurrentState[NUM_INPUTS];
byte flashMode[NUM_INPUTS];//0:off  1:on  2:flash
int flashingCount[NUM_INPUTS];
byte dataBuffer[16];
////functions
void initialPort();
void readSerialPort();
void processData(byte data);
void updateCount();
void updateStatus();

////start
void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  randomSeed(analogRead(0));
  initialPort();
}
void initialPort(){
 for(int i=0;i<NUM_INPUTS;i++){
    pinMode(ports[i], OUTPUT);
    digitalWrite(ports[i],true);
    LEDCurrentState[i]=false;
    flashingCount[i]=random(0,FLASHING_SPEED);
 } 
}
void loop() {
  readSerialPort();
  updateCount();
  updateStatus();
  delay(DELAY_TIME);
}

void readSerialPort(){
  int dataSize=Serial.available();
  if(dataSize>16)dataSize=16;
   //Serial.println(dataSize);
  for(int i=0;i<dataSize;i++){
    dataBuffer[i]=Serial.read();
     

    processData(dataBuffer[i]);
  }
  
  //Serial.flush();
}
void processData(byte data){
  if((data>>4)<NUM_INPUTS){
    if(flashMode[data>>4]!=(data&0x0F)){
        switch((data&0x0F)){
        case 0:
          LEDCurrentState[data>>4]=0;
          break;
        case 1:
          LEDCurrentState[data>>4]=1;
          break;
        case 2:
          LEDCurrentState[data>>4]=0;
          break;
      } 
      digitalWrite(ports[data>>4],!LEDCurrentState[data>>4]);
    }
    flashMode[data>>4]=(data&0x0F); //flashMode
    
  }
}
void updateCount(){
  for(int i=0;i<NUM_INPUTS;i++){
    flashingCount[i]+=DELAY_TIME;
  }
}
void updateStatus(){
  for(int i=0;i<NUM_INPUTS;i++){
     flashingSpeed[i]=(flashMode[i]&0x04)==0?FLASHING_SPEED:random(100,FLASHING_SPEED);
     if(flashingCount[i]>flashingSpeed[i]){
      flashingCount[i]=0;
     // Serial.print(i);
     // Serial.print(' ');
      //Serial.println(flashMode[i]&0x0F);
      switch(flashMode[i]&0x03){
        case 0:
          LEDCurrentState[i]=0;
          break;
        case 1:
          LEDCurrentState[i]=1;
          break;
        case 2:
          LEDCurrentState[i]=!LEDCurrentState[i];
          break;
      }
      
    }
    digitalWrite(ports[i],!LEDCurrentState[i]);
      
    //  Serial.print(' ');
  }
  Serial.println(LEDCurrentState[0]==true?'1':'0');
  //Serial.println("t");
}



