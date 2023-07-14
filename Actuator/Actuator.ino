#include <FastLED.h>
#include "FastLED_RGBW.h"

#define NUM_LEDS 17
#define DATA_PIN 2
#define encoder0PinA 5
#define encoder0PinB 6
#define buttonPin 7
#define delai 20

#define MIN_BRIGHTNESS 0
#define MAX_BRIGHTNESS 255

CRGBW leds[NUM_LEDS];
CRGB *ledsRGB = (CRGB *)&leds[0];

uint8_t brightness = 255;
unsigned long int tim = 0;
volatile int mode = 0;
volatile int param = 0;
bool buttonS, buttonSo;
float percentage;
int r, g, b, w, lightSpeed;

void doEncoder() {
  delayMicroseconds(1000);

  int a = digitalRead(encoder0PinA);
  int b = digitalRead(encoder0PinB);

  if (millis() > tim + delai && !a) {
    tim = millis();
    if (b == 0) {
      Serial.println("-");
    } else {
      Serial.println("+");
    }
  }
}

// the setup function runs once when you press reset or power the board
void setup() {

  FastLED.addLeds<WS2812B, DATA_PIN, RGB>(ledsRGB, getRGBWsize(NUM_LEDS));
  FastLED.setBrightness(brightness);
  FastLED.show();
  Serial.begin(9600);
  pinMode(encoder0PinA, INPUT_PULLUP);
  pinMode(encoder0PinB, INPUT_PULLUP);
  pinMode(buttonPin, INPUT_PULLUP);

  attachInterrupt(digitalPinToInterrupt(encoder0PinA), doEncoder, FALLING);
  //attachInterrupt(digitalPinToInterrupt(encoder0PinB), updateEncoder, CHANGE);
  Serial.begin(9600);
}


void loop() {

  buttonS = digitalRead(buttonPin);
  if (buttonS == LOW && buttonSo == HIGH) Serial.println("o");
  if (buttonS == HIGH && buttonSo == LOW) Serial.println("0");

  buttonSo = buttonS;

  if (Serial.available() > 0) {
    String incoming = Serial.readStringUntil('\n');
    if (incoming[0] == 'p') {
      r = incoming.substring(1, 4).toInt();
      g = incoming.substring(5, 8).toInt();
      b = incoming.substring(9, 12).toInt();
      w = incoming.substring(13, 16).toInt();
      brightness = incoming.substring(17, 20).toInt();
      percentage = incoming.substring(21, 26).toFloat();
      mode = 8;
      CangeCol();
    } else if (incoming[0] == 's') {
      r = incoming.substring(1, 4).toInt();
      g = incoming.substring(5, 8).toInt();
      b = incoming.substring(9, 12).toInt();
      w = incoming.substring(13, 16).toInt();
      brightness = incoming.substring(17, 20).toInt();
      mode = 1;
    } else if (incoming[0] == 'b') {
      r = incoming.substring(1, 4).toInt();
      g = incoming.substring(5, 8).toInt();
      b = incoming.substring(9, 12).toInt();
      w = incoming.substring(13, 16).toInt();
      brightness = incoming.substring(17, 20).toInt();
      lightSpeed = incoming.substring(21, 24).toInt();
      mode = 6;
    } else if (incoming[0] == 'c') {
      r = incoming.substring(1, 4).toInt();
      g = incoming.substring(5, 8).toInt();
      b = incoming.substring(9, 12).toInt();
      w = incoming.substring(13, 16).toInt();
      brightness = incoming.substring(17, 20).toInt();
      lightSpeed = incoming.substring(21, 24).toInt();
      mode = 7;
    } else if (incoming[0] == 'r') {
      brightness = incoming.substring(1, 4).toInt();
      lightSpeed = incoming.substring(5, 8).toInt();
      mode = 5;
    } else if (incoming[0] == 'f') {
      r = incoming.substring(1, 4).toInt();
      g = incoming.substring(5, 8).toInt();
      b = incoming.substring(9, 12).toInt();
      w = incoming.substring(13, 16).toInt();
      brightness = incoming.substring(17, 20).toInt();
      lightSpeed = incoming.substring(21, 24).toInt();
      mode = 2;
    } else mode = incoming.toInt();
  }
  CangeCol();
}


void colorFill(CRGBW c) {
  for (int i = 0; i < NUM_LEDS; i++) {
    leds[i] = c;
    FastLED.setBrightness(brightness);
    FastLED.show();
  }
}

void colorFill(CRGB c) {
  for (int i = 0; i < NUM_LEDS; i++) {
    leds[i] = c;
    FastLED.show();
  }
}

void fillWhite() {
  for (int i = 0; i < NUM_LEDS; i++) {
    leds[i] = CRGBW(0, 0, 0, 255);
    FastLED.show();
  }
}

void rainbow() {
  static double hue;
  for (int i = 0; i < NUM_LEDS; i++) {
    leds[i] = CHSV((i * 256 / NUM_LEDS) + (int)hue, 255, 255);
  }
  FastLED.setBrightness(brightness);
  FastLED.show();
  hue += lightSpeed / 200.0;
}



void breath() {
  static uint8_t hue;
  for (int i = 0; i < NUM_LEDS; i++) {
    leds[i] = CRGBW(r, g, b, w);
  }
  float breath = ((exp(sin(lightSpeed / 25 * millis() / 5000.0 * PI)) - 0.36787944) * 108.0);
  //breath = map(breath, 0, 255, MIN_BRIGHTNESS, MAX_BRIGHTNESS);
  FastLED.setBrightness(breath * brightness / MAX_BRIGHTNESS);
  FastLED.show();
  //CangeCol();
}
void chase() {
  for (int i = 0; i < NUM_LEDS; i++) {
    leds[i] = CRGBW(chaseColor(r, i), chaseColor(g, i), chaseColor(b, i), chaseColor(w, i));
  }
  FastLED.setBrightness(brightness);
  FastLED.show();
}

int chaseColor(int val, int num) {
  if (val == 0) return 0;
  int result = val - differencePosition(num) * 35;
  if (result > 0) return result;
  return 0;
}

int differencePosition(int pos) {
  int diff = (((int)(millis() / 13000.0 * lightSpeed) % NUM_LEDS) - pos);
  if (diff >= 0) return diff;
  return diff + NUM_LEDS;
}

void progress() {
  float partialLight = (percentage * (float)NUM_LEDS / 100) - (int)(percentage * (float)NUM_LEDS / 100);
  for (int i = 0; i < NUM_LEDS; i++) {
    if (i < percentage * (float)NUM_LEDS / 100) leds[i] = CRGBW(r, g, b, w);
    else if (i - 1 < percentage * (float)NUM_LEDS / 100) leds[i] = CRGBW(r * partialLight, g * partialLight, b * partialLight, w * partialLight);
    else leds[i] = CRGBW(0, 0, 0, 0);
  }
  FastLED.setBrightness(brightness);
  FastLED.show();
}
void flash() {
  CRGBW color;
  if ((int)(millis() / 10000.0 * lightSpeed) % 2 == 0) color = CRGBW(r, g, b, w);
  else color = CRGBW(0, 0, 0, 0);
  for (int i = 0; i < NUM_LEDS; i++) {
    leds[i] = color;
    FastLED.setBrightness(brightness);
    FastLED.show();
  }
}
void CangeCol() {
  switch (mode) {
    case 1:
      colorFill(CRGBW(r, g, b, w));
      break;
    case 2:
      flash();
      break;
    case 3:
      colorFill(CRGB::Blue);
      break;
    case 4:
      fillWhite();
      break;
    case 5:
      rainbow();
      break;
    case 6:
      breath();
      break;
    case 7:
      chase();
      break;
    case 8:
      progress();
      break;
  }
}
