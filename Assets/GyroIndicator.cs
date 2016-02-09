using UnityEngine;

public class GyroIndicator : MonoBehaviour {


	// Use this for initialization
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!Input.GetKey(KeyCode.F))
            transform.localRotation = Quaternion.Euler(0, 0, (float)GameController.birdHitCounter / (float)Constant.kPlaneLife * 60 * Mathf.Sin(Time.time));
        else
            transform.localRotation = Quaternion.identity;
	}
}
