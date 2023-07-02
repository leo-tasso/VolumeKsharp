#include <FastLED.h>
#include "FastLED_RGBW.h"

#define NUM_LEDS 17
#define DATA_PIN 2
#define encoder0PinA  5
#define encoder0PinB  6
#define buttonPin  7
#define delai  20

#define MIN_BRIGHTNESS 0
#define MAX_BRIGHTNESS 254

CRGBW leds[NUM_LEDS];
CRGB *ledsRGB = (CRGB *) &leds[0];

const uint8_t brightness = 255;
unsigned long int tim = 0;
volatile int mode = 0;
volatile int param = 0;
bool buttonS, buttonSo;

int r,g,b,w,bri;

void doEncoder() {
  delayMicroseconds(1000);

  int a = digitalRead(encoder0PinA);
  int b = digitalRead(encoder0PinB);

  if (millis() > tim + delai && !a ) {
    tim = millis();
    if (b == 0) {
      Serial.println("-");
    } else {
      Serial.println("+");
    }
  }
}

// the setup function runs once when you press reset or power the board
void setup()
{

  FastLED.addLeds<WS2812B, DATA_PIN, RGB>(ledsRGB, getRGBWsize(NUM_LEDS));
  FastLED.setBrightness(brightness);
  FastLED.show();
  Serial.begin(9600);
  pinMode(encoder0PinA, INPUT_PULLUP);
  pinMode(encoder0PinB, INPUT_PULLUP);
  pinMode(buttonPin, INPUT_PULLUP);

  attachInterrupt(digitalPinToInterrupt(encoder0PinA), doEncoder, FALLING);
  //attachInterrupt(digitalPinToInterrupt(encoder0PinB), updateEncoder, CHANGE);
  Serial.begin (9600);
}






void loop()
{

  buttonS = digitalRead(buttonPin);
  if (buttonS == LOW && buttonSo == HIGH)Serial.println("o");
  if (buttonS == HIGH && buttonSo == LOW)Serial.println("0");

  buttonSo = buttonS;

  if (Serial.available() > 0) {
    String incoming = Serial.readStringUntil('\n');
    if (incoming[0] == 'p') {
      param = incoming.substring(1).toInt();
      mode = 8;
      CangeCol();
    }
     if (incoming[0] == 's') {
      r = incoming.substring(1,4).toInt();
      g = incoming.substring(5,8).toInt();
      b = incoming.substring(9,12).toInt();
      w = incoming.substring(13,16).toInt();
      mode = 1;
    }
    else mode = incoming.toInt();
  }
  CangeCol();
}


void colorFill(CRGBW c) {
  for (int i = 0; i < NUM_LEDS; i++) {
    leds[i] = c;
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
  static uint8_t hue;
  if (mode == 5) {
    for (int i = 0; i < NUM_LEDS; i++) {
      leds[i] = CHSV((i * 256 / NUM_LEDS) + hue, 255, 255);
    }
    FastLED.setBrightness(brightness);
    FastLED.show();
    hue++;
  }
  else {
    CangeCol();
  }
}


void rainbowLoop() {
  while (mode == 5) {
    rainbow();
  }
}


void breathWhite()
{
  static uint8_t hue;
  for (int i = 0; i < NUM_LEDS; i++) {
    leds[i] = CRGBW(0, 0, 0, 255);
  }
  float breath = ((exp(sin(2 * millis() / 5000.0 * PI)) - 0.36787944) * 108.0);
  //breath = map(breath, 0, 255, MIN_BRIGHTNESS, MAX_BRIGHTNESS);
  FastLED.setBrightness(breath);
  FastLED.show();
  //CangeCol();
}
void chaseGreen() {
  for (int i = 0; i < NUM_LEDS; i++) {
    leds[i] = CRGBW(0, 255 - ((255 / NUM_LEDS * i) + (millis() / 10) % 255), 0, 0);
  }
  FastLED.show();
}
void progress(int r, int g, int b, int brightness, float percentage) {
  for (int i = 0; i < NUM_LEDS; i++) {
    if (i < percentage * (float)NUM_LEDS / 100) leds[i] = CRGBW(0, brightness, 0, 0);
    else if (i - 1 < percentage * (float)NUM_LEDS / 100) leds[i] = CRGBW(0, brightness * (percentage * (float)NUM_LEDS / 100) - (int)(percentage * (float)NUM_LEDS / 100), 0, 0);
    else leds[i] = CRGBW(0, 0, 0, 0);
  }
  FastLED.show();
}
void CangeCol() {
  switch (mode) {
    case 1:
      colorFill(CRGBW(r, g, b, w));
      break;
    case 2:
      colorFill(CRGB::Green);
      break;
    case 3:
      colorFill(CRGB::Blue);
      break;
    case 4:
      fillWhite();
      break;
    case 5:
      rainbowLoop();
      break;
    case 6:
      breathWhite();
      break;
    case 7:
      chaseGreen();
      break;
    case 8:
      progress(0, 255, 0, 255, param);
      break;
  }
}
