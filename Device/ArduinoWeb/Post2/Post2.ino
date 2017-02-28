#include <Ethernet.h>
#include <SPI.h>

byte mac[] = { 0x00, 0xAA, 0xBB, 0xCC, 0xDE, 0x01 }; // RESERVED MAC ADDRESS
EthernetClient client;

long previousMillis = 0;
unsigned long currentMillis = 0;
long interval = 250000; // READING INTERVAL

int t = 0;  // TEMPERATURE VAR
int h = 0;  // HUMIDITY VAR
String data;

void setup() { 
  Serial.begin(115200);

  if (Ethernet.begin(mac) == 0) {
    Serial.println("Failed to configure Ethernet using DHCP"); 
  }

  data = "";
}

void loop(){

  currentMillis = millis();
  if(currentMillis - previousMillis > interval) { // READ ONLY ONCE PER INTERVAL
    previousMillis = currentMillis;
  h = 100; 
  t = 321; 
  
  }

  data = "temp1="+String(t)+"&hum1="+String(h);

  if (client.connect("https://postman-echo.com",80)) { // REPLACE WITH YOUR SERVER ADDRESS
    client.println("POST /post HTTP/1.1"); 
    client.println("Host: https://postman-echo.com"); // SERVER ADDRESS HERE TOO
    client.println("Content-Type: application/x-www-form-urlencoded"); 
    client.print("Content-Length: "); 
    client.println(data.length()); 
    client.println(); 
    client.print(data); 
  } 

  if (client.connected()) { 
    client.stop();  // DISCONNECT FROM THE SERVER
  }

  delay(3000); // WAIT 3 seconds
}




