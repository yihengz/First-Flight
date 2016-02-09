using UnityEngine;
using System.IO;
using System.IO.IsolatedStorage;

public class Config : MonoBehaviour {

    private XML xml;

    void Awake () {
        try {
            xml = XML.Load(Application.dataPath + "/config.xml");
        }
        catch(IsolatedStorageException e) {
            xml = new XML();
            xml.configs = new Entry[Microphone.devices.Length];
            for(int i = 0; i < Microphone.devices.Length; i++) {
                xml.configs[i] = new Entry();
                xml.configs[i].DeviceName = Microphone.devices[i];
            }
            xml.Save(Application.dataPath + "/config.xml");
        }
        Entry conf = xml.configs[0];
        Constant.kDevStr = conf.DeviceName;
        Constant.kMicSens = conf.MicSensitivity;
        Constant.kLowPass = conf.LowPassCutOff;
        Constant.kHighPass = conf.HighPassCutOff;
        Debug.LogError(Constant.kMicSens);
    }
}