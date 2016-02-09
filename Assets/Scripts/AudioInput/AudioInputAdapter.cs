using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public delegate float VolumeSelectHandler (string dev);
public delegate int SpectrumSelectHandler (string dev);

public class AudioInputAdapter : GenericSingleton<AudioInputAdapter> {

    #region vars
    public AudioMixerGroup target;
    public event VolumeSelectHandler VolumeSelect;
    public event SpectrumSelectHandler SpectrumSelect;

    private Hashtable audioin_;
    private float sensitivity_ = Constant.kMicSens;
    private const int kVolumeSample = 2048;
    private const int kSpectrumSample = 64;
    #endregion

    #region public_methods
    /******************************************
    Use these two methods to get the input data
    ******************************************/
    public float GetInputVolume (string dev) {
        if (Input.GetKey(KeyCode.W))
            return 3;
        if (dev == null)
            return 0;
        return VolumeSelect(dev);
    }

    public float GetInputSpectrum (string dev) {
        float res = SpectrumSelect(dev);
        return res;
    }

    public static string GetDevice () {
        return Constant.kDevStr;
    }

    #endregion

    #region mono_methods

    void Start () {
        //Change the input device type here
        audioin_ = new Hashtable();
        AudioSource audio;
        
        foreach (string devn in Microphone.devices) {
            audio = gameObject.AddComponent<AudioSource>();
            audio.loop = true;
            audio.outputAudioMixerGroup = target;
            audio.ignoreListenerPause = true;
            if (!audioin_.ContainsKey(devn))
                audioin_.Add(devn, audio);
        }

        string dev = GetDevice();
        if (audioin_.ContainsKey(dev)) {
            ((AudioSource)audioin_[dev]).clip = Microphone.Start(dev, true, 240, 48000);
            ((AudioSource)audioin_[dev]).Play();
        }
        VolumeSelect += MicInputVolume;
        SpectrumSelect += MicInputFreq;
        gameObject.AddComponent<AudioHighPassFilter>().cutoffFrequency = Constant.kHighPass;
        gameObject.AddComponent<AudioLowPassFilter>().cutoffFrequency = Constant.kLowPass;
    }
    #endregion

    #region private_methods
    private float MicInputVolume (string dev) {
        if (dev == null)
            return 0;
        float[] _samples = new float[kVolumeSample];
        float sum = 0;
        if (!audioin_.ContainsKey(dev)) {
            Debug.LogError("Invalid Device");
            return 0;
        }
        AudioSource audio;
        if (audioin_.ContainsKey(dev))
            audio = audioin_[dev] as AudioSource;
        else
            return 0;
        if (audio != null)
            audio.GetOutputData(_samples, 0);
        foreach (float _sample in _samples) {
            sum += Mathf.Abs(_sample);
        }
        //Debug.Log(sum / kVolumeSample);
        return sensitivity_ * sum / kVolumeSample;
    }

    private int MicInputFreq (string dev) {
        if (dev == null)
            return -1;
        float[] samples = new float[kSpectrumSample];
        if (!audioin_.ContainsKey(dev)) {
            Debug.LogError("Invalid Device");
            return 0;
        }
        AudioSource audio = audioin_[dev] as AudioSource;
        audio.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);
        int peak = 0;
        float max = 0;
        for (int i = 0; i < samples.Length; i++) {
            if (samples[i] > max) {
                max = samples[i];
                peak = i;
            }
        }
        return peak;
    }
    #endregion
}
