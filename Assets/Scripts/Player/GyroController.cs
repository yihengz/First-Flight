using UnityEngine;

public class GyroController : MonoBehaviour {

    private float trot;
    private float rot;
    private float angular;

    void Update () {
        trot = Mathf.Clamp((float)GameController.birdHitCounter, 0, Constant.kPlaneLife) / (float)Constant.kPlaneLife * 240f;
        rot = Mathf.SmoothDamp(rot, trot, ref angular, 1.0f);
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rot));
    }
}
