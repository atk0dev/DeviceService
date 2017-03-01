/*
   Web client sketch for IDE v1.0.1 and w5100/w5200
   Uses POST method.
   Posted November 2012 by SurferTim
*/

#include <SPI.h>
#include <Ethernet.h>
#include <OneWire.h>

byte mac[] = {  
  0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };

//Change to your server domain
char serverName[] = "devicedatareceiver.azurewebsites.net";

// change to your server's port
int serverPort = 80;

// change to the page on that server
char pageName[] = "/api/data";

EthernetClient client;
int totalCount = 0; 

// set this to the number of milliseconds delay
// this is 30 seconds
#define delayMillis 30000UL

unsigned long thisMillis = 0;
unsigned long lastMillis = 0;

String dataToSend;

OneWire ds(2);

void setup() {
  Serial.begin(9600);

  // disable SD SPI
  pinMode(4,OUTPUT);
  digitalWrite(4,HIGH);

  Serial.print(F("Starting ethernet..."));
  if(!Ethernet.begin(mac)) Serial.println(F("failed"));
  else Serial.println(Ethernet.localIP());

  delay(2000);
  Serial.println(F("Ready"));

}

void loop()
{
  // If using a static IP, comment out the next line
  Ethernet.maintain();

  thisMillis = millis();

  if(thisMillis - lastMillis > delayMillis)
  {
    lastMillis = thisMillis;

// --------------------------

    byte i;
    byte present = 0;
    byte type_s;
    byte data[12];
    byte addr[8];
    float celsius, fahrenheit;

    if (!ds.search(addr)) 
    {
      Serial.println("No more addresses.");
      Serial.println();
      ds.reset_search();
      delay(250);
      return;
    }

    Serial.print("ROM =");
    for( i = 0; i < 8; i++) 
    {
      Serial.write(' ');
      Serial.print(addr[i], HEX);
    }

    if (OneWire::crc8(addr, 7) != addr[7]) 
    {
      Serial.println("CRC is not valid!");
      return;
    }

    switch (addr[0]) 
    {
      case 0x10:
        Serial.println(" Chip = DS18S20"); // или более старый DS1820
        type_s = 1;
        break;
      
      case 0x28:
        Serial.println(" Chip = DS18B20");
        type_s = 0;
        break;

      case 0x22:
        Serial.println(" Chip = DS1822");
        type_s = 0;
        break;

      default:
        Serial.println("Device is not a DS18x20 family device.");
        return;
    }

    ds.reset();
    ds.select(addr);
    ds.write(0x44);
    delay(1000);

    present = ds.reset();

    ds.select(addr);
    ds.write(0xBE);
    Serial.print(" Data = ");
    Serial.print(present, HEX);
    Serial.print(" ");
    for ( i = 0; i < 9; i++) 
    {
      data[i] = ds.read();
      Serial.print(data[i], HEX);
      Serial.print(" ");
    }

    Serial.print(" CRC=");
    Serial.print(OneWire::crc8(data, 8), HEX);
    Serial.println();

    int16_t raw = (data[1] << 8) | data[0];
    if (type_s) 
    {
      raw = raw << 3; 
      if (data[7] == 0x10) 
      {
        raw = (raw & 0xFFF0) + 12 - data[6];
      }
    } 
    else 
    {
      byte cfg = (data[4] & 0x60);
      if (cfg == 0x00) raw = raw & ~7; // разрешение 9 бит, 93.75 мс
      else if (cfg == 0x20) raw = raw & ~3; // разрешение 10 бит, 187.5 мс
      else if (cfg == 0x40) raw = raw & ~1; // разрешение 11 бит, 375 мс
      //// разрешение по умолчанию равно 12 бит, время преобразования - 750 мс
    }
    
    celsius = (float)raw / 16.0;
    Serial.print(" Temperature = ");
    Serial.print(celsius);
    Serial.print(" Celsius.");    
    
    Serial.println();
   
// --------------------------
    dataToSend = "DeviceId=" + String(1) + "&Title=" + "temp" + "&Value=" + String(celsius);
    
    Serial.print("Sending data: ");
    Serial.println(dataToSend);

    if(!postPage(serverName,serverPort,pageName,string2char(dataToSend))) Serial.print(F("Fail "));
    else Serial.print(F("Pass "));
    totalCount++;
    Serial.println(totalCount,DEC);
  }    
}


byte postPage(char* domainBuffer,int thisPort,char* page,char* thisData)
{
  int inChar;
  char outBuf[64];

  Serial.print(F("connecting..."));
  
  if(client.connect(domainBuffer,thisPort) == 1)
  {
    Serial.println(F("connected"));

    // send the header
    sprintf(outBuf,"POST %s HTTP/1.1",page);
    client.println(outBuf);
    sprintf(outBuf,"Host: %s",domainBuffer);
    client.println(outBuf);
    client.println(F("Connection: close\r\nContent-Type: application/x-www-form-urlencoded"));
    sprintf(outBuf,"Content-Length: %u\r\n",strlen(thisData));
    client.println(outBuf);

    // send the body (variables)
    Serial.print(F("request data: "));
    Serial.println(thisData);
    client.print(thisData);
  } 
  else
  {
    Serial.println(F("failed"));
    return 0;
  }

  int connectLoop = 0;

  while(client.connected())
  {
    while(client.available())
    {
      inChar = client.read();
      Serial.write(inChar);
      connectLoop = 0;
    }

    delay(1);
    connectLoop++;
    if(connectLoop > 10000)
    {
      Serial.println();
      Serial.println(F("Timeout"));
      client.stop();
    }
  }

  Serial.println();
  Serial.println(F("disconnecting."));
  client.stop();
  return 1;
}

char* string2char(String str){
    if(str.length()!=0){
        char *p = const_cast<char*>(str.c_str());
        return p;
    }
}
