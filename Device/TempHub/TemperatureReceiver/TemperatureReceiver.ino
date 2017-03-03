#include <string.h> 
#include <stdio.h>
#include <stdlib.h>
#include <SPI.h>
#include <Ethernet.h>
#include <OneWire.h>


#ifndef _MY_TYPES_H
#define _MY_TYPES_H
#include "mytypes.h"
#endif

#ifndef _MY_GENERICLIST_H
#define _MY_GENERICLIST_H
#include "genericlist.h"
#endif

byte mac[] = { 0x1E, 0xA5, 0xBE, 0xAF, 0x9E, 0xE3 };

unsigned long lastConnectionTime = 0;             // last time you connected to the server, in milliseconds
const unsigned long postingInterval = 10L * 1000L; // delay between updates, in milliseconds
												   // the "L" is needed to use long type numbers

char *device_names[3];
list devices;

OneWire  ds(2);  // on pin 2 (a 4.7K resistor is necessary)

char serverName[] = "devicedatareceiver.azurewebsites.net";
int serverPort = 80;
char pageName[] = "/api/data";

EthernetClient client;

char dataToSend[200];

void print_data()
{
	for (device * a = (device *)list_begin(&devices); a; a = (device *)list_next(&a->header))
	{
		printf("device: %s - %f \n", a->name, a->value);
	}
}

device* find_device_by_name(char* deviceName)
{
	device* found = 0;

	for (device * a = (device *)list_begin(&devices); a; a = (device *)list_next(&a->header))
	{
		if (strcmp(a->name, deviceName) == 0)
		{
			found = a;
			break;
		}
	}

	return found;
}

device * read_device_data()
{

	byte i;
	byte present = 0;
	byte type_s;
	byte data[12];
	byte addr[8];
	float celsius;

	char buffer[8] = "";
	char *pbuffer = buffer;

	if (!ds.search(addr)) {
		Serial.println("No more addresses.");
		Serial.println();
		ds.reset_search();
		delay(250);
		return;
	}

	Serial.print("ROM =");
	for (i = 0; i < 8; i++) {
		Serial.write(' ');
		Serial.print(addr[i], HEX);

		sprintf(buffer, "%02X", (unsigned char)addr[i]);
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
	for (i = 0; i < 9; i++) {           // we need 9 bytes
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
	}
	else {
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



	int rand_i = rand() % 3;
	char* name = pbuffer;

	device * a = (device *)malloc(sizeof(device));
	a->value = celsius;
	a->name = name;

	return a;
}

void buildDataString()
{
	device* a;
	char buffer[20];
	int i = 1;
	strcpy(dataToSend, "");
	strcat(dataToSend, "DeviceId=");

	itoa(1, buffer, 10);

	strcat(dataToSend, buffer);

	while (!list_empty(&devices))
	{
		a = (device *)list_pop_front(&devices);
		printf("%d\n", a->value);
		printf("empty %d\n", list_empty(&devices));

		strcat(dataToSend, "&Title");
		itoa(i, buffer, 10);
		strcat(dataToSend, buffer);
		strcat(dataToSend, "=");

		strcat(dataToSend, a->name);

		strcat(dataToSend, "&Value");
		itoa(i, buffer, 10);
		strcat(dataToSend, buffer);

		strcat(dataToSend, "=");

		sprintf(buffer, "%f", a->value);
		strcat(dataToSend, buffer);

		free(a);
		i++;
	}
}


void setup(void) {

	device_names[0] = "device_1";
	device_names[1] = "device_2";
	device_names[2] = "device_3";

	list_init(&devices);

  Serial.begin(9600);

  pinMode(4, OUTPUT);
  digitalWrite(4, HIGH);

  Serial.print(F("Starting ethernet..."));

  if (!Ethernet.begin(mac))
  {
	  Serial.println(F("Failed to start ethernet"));
  }
  else
  {
	  Serial.println(Ethernet.localIP());
  }

  delay(2000);
  Serial.println(F("Ready"));
}

void loop(void) {
  
  Ethernet.maintain();

  Serial.println();
  Serial.println("--== L O O P ==--");

  // 1. Read temp data and populate result list
  // 2. Transform result list to query string
  // 3. Send string data to server

  device* a = read_device_data();
  device* existing = find_device_by_name(a->name);

  if (existing == 0)
  {
	  list_push_back(&devices, &a->header);
  }
  else
  {
	  existing->value = a->value;
	  free(a);
  }

  // if ten seconds have passed since your last connection,
  // then connect again and send data:
  if (millis() - lastConnectionTime > postingInterval) 
  {
	  

	  for (device * a = (device *)list_begin(&devices); a; a = (device *)list_next(&a->header))
	  {
		  printf("device: %s - %f \n", a->name, a->value);
		  
	  }
	  
	  buildDataString();
	    
      if (!httpRequest(serverName, serverPort, pageName, dataToSend))
	  {
		  Serial.print(F("Fail "));
	  }
	  else
	  {
		  Serial.print(F("Pass "));
	  }
  }

  Serial.println();
  Serial.println("--==  E N D  ==--");
  Serial.println();
  Serial.println();
 
}

byte httpRequest(char* domainBuffer, int thisPort, char* page, char* thisData)
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

	lastConnectionTime = millis();

	Serial.println();
	Serial.println(F("disconnecting."));
	client.stop();
	return 1;
}
