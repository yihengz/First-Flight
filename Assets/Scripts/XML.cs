using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

[Serializable]
[XmlRoot("Configuration")]
public class XML {
    [XmlArray("Microphones")]
    [XmlArrayItem("Microphone")]
    public Entry[] configs;

    public void Save (string path) {
        XmlSerializer xs = new XmlSerializer(typeof(XML));
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.CloseOutput = true;
        using (var stream = new FileStream(path, FileMode.Create)) {
            xs.Serialize(stream, this);
            stream.Close();
        }
    }

    public static XML Load (string path) {
        XmlSerializer xs = new XmlSerializer(typeof(XML));
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.CloseOutput = true;
        using (var stream = new FileStream(path, FileMode.Open)) {
            XML val = xs.Deserialize(stream) as XML;
            stream.Close();
            return val;
        }
    }

}

[Serializable]
public class Entry {
    [XmlAttribute("DeviceName")]
    public string DeviceName;

    public float MicSensitivity = 1000;
    public float LowPassCutOff = 1200;
    public float HighPassCutOff = 100;
}
