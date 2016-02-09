using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaneBehaviour : GenericSingleton<PlaneBehaviour> {
    public GameObject smoke;
    public GameObject smokered;

    private static AudioInputAdapter kAudioIn;
    private List<GameObject> childlist_;
    private Hashtable rigtable_, postable_;
    private Animator anim_;
    private int hash_blend_ = Animator.StringToHash("Blend");
    private Vector3 tpos_, current_vel_;
    private GameObject smoketemp, smokeredtemp;
    private enum Smoke {
        kNone,
        kRegular,
        kRed
    }
    private Smoke smoke_ = Smoke.kNone;

    public void PlaneDestruct () {
        foreach (GameObject child in childlist_) {
            Rigidbody rig = rigtable_[child] as Rigidbody;
            rig.isKinematic = false;
            rig.velocity = new Vector3(Random.Range(-Constant.kExpAmp, Constant.kExpAmp), 0, Random.Range(-Constant.kExpAmp, Constant.kExpAmp));
        }
        Destroy(smoketemp);
        Destroy(smokeredtemp);
    }

    void Start () {
        kAudioIn = AudioInputAdapter.Instance;

        anim_ = GetComponent<Animator>();

        childlist_ = new List<GameObject>();
        rigtable_ = new Hashtable();
        postable_ = new Hashtable();
        GetAllChildren(gameObject);
        foreach (GameObject child in childlist_) {
            Rigidbody rig = child.AddComponent<Rigidbody>();
            rig.isKinematic = true;
            rigtable_.Add(child, rig);
            postable_.Add(child, rig.transform.localPosition);
            //child.AddComponent<BoxCollider>();
        }
    }

    void FixedUpdate () {
        anim_.SetFloat(hash_blend_, Mathf.Clamp(kAudioIn.GetInputVolume(AudioInputAdapter.GetDevice()), 0, 5f));
        foreach (GameObject child in childlist_) {
            ((Rigidbody)rigtable_[child]).transform.localPosition = ((Vector3)postable_[child])
                + new Vector3(
                    Random.Range(-GameController.birdHitCounter * Constant.kWigAmp, GameController.birdHitCounter * Constant.kWigAmp),
                    Random.Range(-GameController.birdHitCounter * Constant.kWigAmp, GameController.birdHitCounter * Constant.kWigAmp),
                    Random.Range(-GameController.birdHitCounter * Constant.kWigAmp, GameController.birdHitCounter * Constant.kWigAmp));
            //child.transform.localPosition = Vector3.SmoothDamp(child.transform.position, tpos_, ref current_vel_, 1.0f);
        }
        if (GameController.birdHitCounter >= (float)Constant.kPlaneLife * 2 / 3) {
            if (smoke_ != Smoke.kRed) {
                smokeredtemp = (GameObject)Instantiate(smokered, transform.position, Quaternion.Euler(0, 180, 0));
                smokeredtemp.transform.parent = transform.parent;
                smoke_ = Smoke.kRed;
            }
        }
        else if (GameController.birdHitCounter >= (float)Constant.kPlaneLife / 3) {
            if (smoke_ != Smoke.kRegular) {
                smoketemp = (GameObject)Instantiate(smoke, transform.position, Quaternion.Euler(0, 180, 0));
                smoketemp.transform.parent = transform.parent;
                if (smokeredtemp != null) {
                    StartCoroutine(SmokeDestroy(smokeredtemp));
                }
                smoke_ = Smoke.kRegular;
            }
        }
        else {
            smoke_ = Smoke.kNone;
            if (smoketemp != null) {
                StartCoroutine(SmokeDestroy(smoketemp));
            }
        }
        
    }

    IEnumerator SmokeDestroy(GameObject des) {
        des.GetComponent<ParticleSystem>().emissionRate = 0;
        yield return new WaitForSeconds(2);
        Destroy(des);
    }

    void GetAllChildren (GameObject gobj) {
        if (gobj.transform.childCount == 0)
            childlist_.Add(gobj);
        else {
            for (int i = 0; i < gobj.transform.childCount; i++) {
                GameObject temp = gobj.transform.GetChild(i).gameObject;
                GetAllChildren(temp);
            }
        }
    }
}


