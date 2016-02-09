using UnityEngine;
using System.Collections;

public class MoveGear : MonoBehaviour {

    private const float kSpeed = -0.8f;
    private Rigidbody rig_;

	// Use this for initialization
	void Start () {
        rig_ = GetComponent<Rigidbody>();
        rig_.useGravity = false;
        rig_.velocity = transform.forward * kSpeed;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
	}

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Airplane") {
            if (GameController.birdHitCounter > 0)
                GameController.birdHitCounter--;
            StartCoroutine(SoundPlay());
            StartCoroutine(GearDestruct());
        }
    }

    IEnumerator GearDestruct () {
        int count = 30;
        float step = 1 / (float)count;
        while (count > 0) {
            transform.localScale = new Vector3(count * step, count * step, count * step);
            count--;
            yield return null;
        }
    }
    IEnumerator SoundPlay () {
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}
