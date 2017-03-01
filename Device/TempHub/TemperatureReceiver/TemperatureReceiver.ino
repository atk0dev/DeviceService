#include <SPI.h>
#include <Ethernet.h>
#include <OneWire.h>

byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };

OneWire  ds(2);  // on pin 2 (a 4.7K resistor is necessary)

char dataToSend[100] = {0};
char serverName[] = "devicedatareceiver.azurewebsites.net";
int serverPort = 80;
char pageName[] = "/api/data";

EthernetClient client;

void setup(void) {
  Serial.begin(9600);

  pinMode(4, OUTPUT);
  digitalWrite(4, HIGH);

  Serial.print(F("Starting ethernet..."));
  if (!Ethernet.begin(mac)) Serial.println(F("failed"));
  else Serial.println(Ethernet.localIP());

  delay(2000);
  Serial.println(F("Ready"));

}

void loop(void) {
  
  Ethernet.maintain();

  Serial.println();
  Serial.println("--== L O O P ==--");

  ReadTempData(dataToSend, 100);
  Serial.println();
  Serial.println(dataToSend);

  if (!PostPage(serverName, serverPort, pageName, dataToSend))
  {
	  Serial.print(F("Fail "));
  } 
  else
  {
	  Serial.print(F("Pass "));
  }
 
  Serial.println();
  Serial.println("--==  E N D  ==--");
  Serial.println();
  Serial.println();
 
}

byte PostPage(char* domainBuffer, int thisPort, char* page, char* thisData)
{
	int inChar;
	char outBuf[64];

	Serial.print(F("connecting..."));

	if (client.connect(domainBuffer, thisPort) == 1)
	{
		Serial.println(F("connected"));

		// send the header
		sprintf(outBuf, "POST %s HTTP/1.1", page);
		client.println(outBuf);
		sprintf(outBuf, "Host: %s", domainBuffer);
		client.println(outBuf);
		client.println(F("Connection: close\r\nContent-Type: application/x-www-form-urlencoded"));
		sprintf(outBuf, "Content-Length: %u\r\n", strlen(thisData));
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

	while (client.connected())
	{
		while (client.available())
		{
			inChar = client.read();
			Serial.write(inChar);
			connectLoop = 0;
		}

		delay(1);
		connectLoop++;
		if (connectLoop > 10000)
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


void ReadTempData(char *buf, int len)
{   
  byte i;
  byte present = 0;
  byte type_s;
  byte data[12];
  byte addr[8];
  float celsius;

  sprintf(buf,"");
  
  if ( !ds.search(addr)) {
    Serial.println("No more addresses.");
    Serial.println();
    ds.reset_search();
    delay(250);
    sprintf(buf,"");
    return;
  }

  sprintf(buf + strlen(buf),"DeviceId=");
  sprintf(buf + strlen(buf), "%d", 1);
  sprintf(buf + strlen(buf),"&Title=");

  Serial.print("ROM =");
  for( i = 0; i < 8; i++) {
    Serial.write(' ');
    Serial.print(addr[i], HEX);
    sprintf(buf + strlen(buf), "%02X",  addr[i]);
  }

  if (OneWire::crc8(addr, 7) != addr[7]) {
      Serial.println("CRC is not valid!");
      return;
  }
  Serial.println();

  // the first ROM byte indicates which chip
  switch (addr[0]) {
    case 0x10:
      Serial.println("  Chip = DS18S20");  // or old DS1820
      type_s = 1;
      break;
    case 0x28:
      Serial.println("  Chip = DS18B20");
      type_s = 0;
      break;
    case 0x22:
      Serial.println("  Chip = DS1822");
      type_s = 0;
      break;
    default:
      Serial.println("Device is not a DS18x20 family device.");
      return;
  } 

  ds.reset();
  ds.select(addr);
  ds.write(0x44, 1);        // start conversion, with parasite power on at the end
  
  delay(1000);     // maybe 750ms is enough, maybe not
  // we might do a ds.depower() here, but the reset will take care of it.
  
  present = ds.reset();
  ds.select(addr);    
  ds.write(0xBE);         // Read Scratchpad

  Serial.print("  Data = ");
  Serial.print(present, HEX);
  Serial.print(" ");
  for ( i = 0; i < 9; i++) {           // we need 9 bytes
    data[i] = ds.read();
    Serial.print(data[i], HEX);
    Serial.print(" ");    
  }
  Serial.print(" CRC=");
  Serial.print(OneWire::crc8(data, 8), HEX);
  Serial.println();

  // Convert the data to actual temperature
  int16_t raw = (data[1] << 8) | data[0];
  if (type_s) {
    raw = raw << 3; // 9 bit resolution default
    if (data[7] == 0x10) {
      // "count remain" gives full 12 bit resolution
      raw = (raw & 0xFFF0) + 12 - data[6];
    }
  } else {
    byte cfg = (data[4] & 0x60);
    // at lower res, the low bits are undefined, so let's zero them
    if (cfg == 0x00) raw = raw & ~7;  // 9 bit resolution, 93.75 ms
    else if (cfg == 0x20) raw = raw & ~3; // 10 bit res, 187.5 ms
    else if (cfg == 0x40) raw = raw & ~1; // 11 bit res, 375 ms
    //// default is 12 bit resolution, 750 ms conversion time
  }
  celsius = (float)raw / 16.0;
  Serial.print("  Temperature = ");
  Serial.print(celsius);
  Serial.print(" Celsius");

  sprintf(buf + strlen(buf),"&Value=");
  sprintf(buf + strlen(buf), "%.2f", celsius);

  //  dataToSend = "DeviceId=" + String(1) + "&Title=" + "temp" + "&Value=" + String(celsius);
}
