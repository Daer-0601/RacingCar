#define POT_STEER A0     // Potenciómetro del volante
#define ACCEL_PIN 2      // Botón del acelerador

float filteredAngle = 0.0;  // Filtro para evitar giros raros

// Rango de giro del volante (ajusta si quieres más o menos grados)
const int MIN_ANGLE = -450;
const int MAX_ANGLE = 450;

void setup() {
  Serial.begin(115200);
  pinMode(ACCEL_PIN, INPUT_PULLUP);  // Botón conectado a GND
}

void loop() {
  // -------------------- Volante (potenciómetro) --------------------
  int raw = analogRead(POT_STEER);

  // Convertir a grados
  int angle = map(raw, 0, 1023, MIN_ANGLE, MAX_ANGLE);

  // Suavizado para evitar que el auto gire solo
  filteredAngle = (filteredAngle * 0.85f) + (angle * 0.15f);

  // -------------------- Acelerador (botón) --------------------
  int accel = (digitalRead(ACCEL_PIN) == LOW) ? 1 : 0;

  // -------------------- Enviar a Unity --------------------
  Serial.print((int)filteredAngle);
  Serial.print(",");
  Serial.println(accel);

  delay(16); // ~60 actualizaciones por segundo
}
