using UnityEngine;
using System.IO.Ports;

public class ArduinoSteering : MonoBehaviour
{
    public string portName = "COM3"; 
    public int baudRate = 115200;
    public CarMovement carMovement;

    private SerialPort serial;

    void Start()
    {
        serial = new SerialPort(portName, baudRate);
        serial.ReadTimeout = 30;

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

            if (parts.Length == 2)
            {
                int angle = int.Parse(parts[0]);
                int accel = int.Parse(parts[1]);

                carMovement.SetSteerInput(angle);
                carMovement.SetAccelInput(accel);
            }
        }
        catch (System.TimeoutException) { }
        catch (System.Exception e)
        {
            Debug.LogWarning("Error Serial: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        if (serial != null && serial.IsOpen)
            serial.Close();
    }
}
