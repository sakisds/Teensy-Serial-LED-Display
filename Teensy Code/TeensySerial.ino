// SmartMatrix Library
#include <SmartMatrix_16x32.h>

// LED Pin (used for serial communication)
#define LED 13

// Matrix
SmartMatrix matrix;
uint8_t incomingByte;

void setup() {
    // Flash LED while setup is going on
    pinMode(LED, OUTPUT);
    digitalWrite(LED, HIGH);

    // Serial always runs at full USB speed
    Serial.begin(115200);

    // Prepare matrix
    matrix.begin();
    matrix.setBrightness(100*(255/100)); // Max brightness

    matrix.setColorCorrection(cc24);
    
    // Turn LED off
    digitalWrite(LED, LOW);
}

void loop() {
    if (Serial.available()) {
        digitalWrite(13, HIGH);
        
        incomingByte = Serial.read();
        switch (incomingByte) {
            case 0xFF:
                matrix.swapBuffers(false);
                break;
            case 0xFE:
                matrix.setBrightness(Serial.read());
                break;
            case 0xFD:
                readFrame();
                break;
            default:
                readPixel();
        }
    } else 
        digitalWrite(13, LOW);
}

void readPixel() {
    digitalWrite(13, HIGH);
    
    elapsedMillis sinceBegin;
    
    while (sinceBegin < 1000) {
        if (Serial.available() >= 4) {        
            matrix.drawPixel(incomingByte, Serial.read(), {Serial.read(), Serial.read(), Serial.read()});
            break;
        }
    }
    
    digitalWrite(13, LOW);
}

void readFrame() {
    digitalWrite(13, HIGH);

    uint8_t x = 0;
    uint8_t y = 0;    
    
    elapsedMillis sinceBegin;
    
    while (sinceBegin < 1000 && y != 16) {
        if (Serial.available() >= 3) {
            matrix.drawPixel(x, y, {Serial.read(), Serial.read(), Serial.read()});
            x++;
            
            if (x == 32) {
                y++; x = 0;
            }
        }
    }
    
    digitalWrite(13, LOW);
    
    if (y == 16)
        matrix.swapBuffers(false);
}