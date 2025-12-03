#define POT_STEER A0     // Volante (potenciómetro)

// Botones
#define ACCEL_PIN 8      // Acelerador
#define BRAKE_PIN 3      // Freno

// Filtro del volante
float filteredAngle = 0.0;

// Rango de giro permitido
const int MIN_ANGLE = -450;
const int MAX_ANGLE = 450;

void setup() {
  Serial.begin(115200);

  // Botones en modo PULLUP (presionado = LOW)
  pinMode(ACCEL_PIN, INPUT_PULLUP);
  pinMode(BRAKE_PIN, INPUT_PULLUP);
}

void loop() {
  // -------------------- Volante --------------------
  int raw = analogRead(POT_STEER);

  // Convertir a grados
  int angle = map(raw, 0, 1023, MIN_ANGLE, MAX_ANGLE);

  // Suavizado (anti-ruido)
  filteredAngle = (filteredAngle * 0.85f) + (angle * 0.15f);

  // -------------------- Botones --------------------
  int accel = (digitalRead(ACCEL_PIN) == LOW) ? 1 : 0;
  int brake = (digitalRead(BRAKE_PIN) == LOW) ? 1 : 0;

  // -------------------- Formato enviado --------------------
  //     ANGULO,ACELERAR,FRENO
  Serial.print((int)filteredAngle);
  Serial.print(",");
  Serial.print(accel);
  Serial.print(",");
  Serial.println(brake);

  delay(16); // 60 FPS
}
