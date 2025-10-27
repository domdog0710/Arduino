using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class ArduinoManager : MonoBehaviour
{
    [SerializeField, Space, Header("Arduino Json Manager")]
    ArduinoJsonManager ArduinoJsonManager;

    [Space, Header("Serial Ports")]
    List<SerialPort> SerialPorts = new List<SerialPort>();

    [Space, Header("Thread")]
    Thread Thread;

    // Start is called before the first frame update
    void Start()
    {
        GetSerialPort();
        OpenPort();

        try
        {
            Thread = new Thread(new ThreadStart(ReadData));
            Thread.Start();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ReadData();
    }

    private void GetSerialPort()
    {
        RegistryKey rk1 = Registry.LocalMachine;
        RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
        string temp;

        foreach (string s3 in rk2.GetSubKeyNames())
        {
            RegistryKey rk3 = rk2.OpenSubKey(s3);
            foreach (string s in rk3.GetSubKeyNames())
            {
                if (s.Contains("VID") && s.Contains("PID"))
                {
                    RegistryKey rk4 = rk3.OpenSubKey(s);
                    foreach (string s2 in rk4.GetSubKeyNames())
                    {
                        RegistryKey rk5 = rk4.OpenSubKey(s2);

                        if ((temp = (string)rk5.GetValue("FriendlyName")) != null && temp.Contains(ArduinoJsonManager.ArduinoBoardData.FriendlyName))
                        {
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            if (rk6 != null && (temp = (string)rk6.GetValue("PortName")) != null)
                            {
                                SerialPort serialport = new SerialPort(temp, ArduinoJsonManager.ArduinoBoardData.iBaudRates);

                                try
                                {
                                    serialport.Open();

                                    ArduinoData.ComPorts.Add(temp);
                                    ArduinoData.strLastMsgs.Add("");
                                    ArduinoData.strMsgs.Add("");
                                    ArduinoData.iBaudRates.Add(ArduinoJsonManager.ArduinoBoardData.iBaudRates);

                                    serialport.Close();

                                    SerialPorts.Add(new SerialPort());
                                }
                                catch 
                                {
                                    serialport.Close();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void OpenPort()
    {
        // check whether port is open 
        try
        {
            for (int i = 0; i < SerialPorts.Count; i++) 
            {
                SerialPorts[i] = new SerialPort(ArduinoData.ComPorts[i], ArduinoData.iBaudRates[i]);
                SerialPorts[i].Open();
                SerialPorts[i].ReadTimeout = 10;

                Debug.Log("Open Port Success...");
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public void ReadData()
    {
        try
        {
            foreach (var (serialport, index) in SerialPorts.Select((value, i) => (value, i)))
            {
                if (serialport.IsOpen)
                {
                    string strMsg = SerialPorts[index].ReadLine();
                    if (strMsg != null)
                    {
                        ArduinoData.strMsgs[index] = strMsg;
                    }
                }
            }
        }
        catch (Exception)
        {
            
        }
    }

    void OnDisable()
    {
        foreach (var (serialport, index) in SerialPorts.Select((value, i) => (value, i)))
        {
            serialport.Close();
        }
    }
    void OnApplicationQuit()
    {
        foreach (var (serialport, index) in SerialPorts.Select((value, i) => (value, i)))
        {
            serialport.Close();
        }
    }
}