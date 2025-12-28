// Donanım: Arduino Mega
// Serial1 -> GSM Modülü (SIM800L)
// Serial2 -> UHF RFID Okuyucu

String serverURL = "http://smartwasteai.net/api/kayit.php"; // API
String deviceID = "1106"; // Bu cihaz

void setup() {
  Serial.begin(9600);
  Serial1.begin(9600);  // GSM Modül
  Serial2.begin(115200); // UHF RFID

  Serial.println("Sistem Baslatiliyor");
  delay(1000);

  gsmBaslat();
}

void loop() {

  if (Serial2.available()) {
    String okunanTag = "";
    

    while (Serial2.available()) {
      int c = Serial2.read();
      okunanTag += String(c, HEX);
      delay(5);
    }
    
    okunanTag.toUpperCase();
    Serial.println("Okunan RFID: " + okunanTag);

    veriyiGonder(okunanTag);

    // 1.5 Dakika Bekleme
    delay(90000); 
    
    while(Serial2.available()) Serial2.read();
  }
}

void gsmBaslat() {
  Serial.println("GSM Baglantisi Kuruluyor...");
  Serial1.println("AT"); 
  delay(1000);
  Serial1.println("AT+CPIN?");
  delay(1000);
  Serial1.println("AT+CREG?");
  delay(1000);
  Serial1.println("AT+SAPBR=3,1,\"Contype\",\"GPRS\"");
  delay(1000);
  Serial1.println("AT+SAPBR=3,1,\"APN\",\"internet\""); 
  delay(1000);
  Serial1.println("AT+SAPBR=1,1"); 
  delay(3000);
  Serial.println("GSM Hazir!");
}

void veriyiGonder(String rfidData) {
  
  Serial1.println("AT+HTTPINIT");
  delay(500);
  Serial1.println("AT+HTTPPARA=\"CID\",1");
  delay(500);
  
  Serial1.print("AT+HTTPPARA=\"URL\",\"");
  Serial1.print(serverURL);
  Serial1.println("\"");
  delay(500);
  
  Serial1.println("AT+HTTPPARA=\"CONTENT\",\"application/json\"");
  delay(500);
  

  String data = "{\"cihaz\":\"" + deviceID + "\",\"rfid\":\"" + rfidData + "\"}";
  
  Serial1.print("AT+HTTPDATA=");
  Serial1.print(data.length());
  Serial1.println(",10000");
  delay(500);
  
  Serial1.println(data);
  delay(500);
  
  // POST İşlemi
  Serial1.println("AT+HTTPACTION=1"); 
  delay(3000); 
  
  Serial1.println("AT+HTTPREAD"); 
  delay(500);
  
  Serial1.println("AT+HTTPTERM"); 
}
