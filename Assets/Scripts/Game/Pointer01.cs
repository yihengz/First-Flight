using UnityEngine;

public class Pointer01 : MonoBehaviour {

    private AudioInputAdapter kAudioIn;
    private float rot;

	// Use this for initialization
	void Start () {
        kAudioIn = AudioInputAdapter.Instance;
	}
	
	// Update is called once per frame
	void Update () {
        rot = Mathf.Clamp(kAudioIn.GetInputVolume(AudioInputAdapter.GetDevice()), 0, 7.5f);
        rot = 220 + rot * 18.67f;
        transform.localRotation = Quaternion.Euler(new Vector3(0, rot, 0));
	}
}
