using System.Collections.Generic;
using UnityEngine;

public static class ArduinoData
{
    [Header("Com Ports")]
    public static List<string> ComPorts = new List<string>();

    [Header("Last Msgs")]
    public static List<string> strLastMsgs = new List<string>();
    [Header("Msgs")]
    public static List<string> strMsgs = new List<string>();

    [Header("Baud Rate")]
    public static List<int> iBaudRates = new List<int>();
}