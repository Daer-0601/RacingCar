using UnityEngine;
using System.IO.Ports;

public class ArduinoSteering : MonoBehaviour
{
    public string portName = "COM6";
    public int baudRate = 115200;
    public CarMovement carMovement;

    private SerialPort serial;

    void Start()
    {
        serial = new SerialPort(portName, baudRate);
        serial.ReadTimeout = 25;
        serial.DtrEnable = true;
        serial.RtsEnable = true;

        try
        {
            serial.Open();
            Debug.Log("✔ Puerto serial abierto: " + portName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ No se pudo abrir el puerto: " + e.Message);
        }
    }

    void Update()
    {
        if (serial == null || !serial.IsOpen) return;

        try
        {
            string line = serial.ReadLine().Trim();
            string[] parts = line.Split(',');

            // Esperamos: ANGULO,ACEL,BRK (3 valores) o ANGULO,ACEL,BRK,TURBO (4 valores - turbo ignorado)
            if (parts.Length >= 3)
            {
                int angle = int.Parse(parts[0]);
                int accel = int.Parse(parts[1]);
                int brake = int.Parse(parts[2]);

                carMovement.SetSteerInput(angle);
                carMovement.SetAccelInput(accel);
                carMovement.SetBrakeInput(brake);
                // Turbo eliminado - ya no se usa
            }
            else
            {
                Debug.LogWarning("⚠ Datos incompletos: " + line);
            }
        }
        catch (System.TimeoutException)
        {
            // No pasa nada, solo no hubo datos this frame
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("⚠ Error Serial: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        if (serial != null && serial.IsOpen)
            serial.Close();
    }
}
