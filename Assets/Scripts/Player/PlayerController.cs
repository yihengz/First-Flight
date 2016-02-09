using UnityEngine;
using System.Collections;

struct Boundary {
    public float kXMin, kXMax, kZMin, kZMax;
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : GenericSingleton<PlayerController> {
    #region vars
    // Physics variables
    public GameObject explosion;

    private Boundary boundary_;
    private Rigidbody player_;
    private Vector3 target_;
    private float qytarget_;
    private float qztarget_;
    private Vector3 current_velocity = Vector3.zero;
    private float current_yangular = 0;
    private float current_zangular = 0;
    private float moveHorizontal;
    private float moveVertical;

    // Boost variables
    private bool inBoostMode = false;
    private float time_stamp_;
    private float crash_;

    // Game control vars
    private static GameController kGameCtrl;
    private static AudioInputAdapter kAudioIn;
    private static SoundManager kSound;
    private static bool ingame_ = false;
    #endregion

    #region public_methods
    public void MoveTarget (Vector3 movement) {
        target_ += movement;
        target_.x = Mathf.Clamp(target_.x, boundary_.kXMin, boundary_.kXMax);
        target_.z = Mathf.Clamp(target_.z, boundary_.kZMin, boundary_.kZMax);
    }
    #endregion

    #region mono_methods
    void Awake () {
        kGameCtrl = GameController.Instance;
        kGameCtrl.StartGame += StartGame;
        kGameCtrl.EndGame += EndGame;
    }

    void Start () {
        player_ = GetComponent<Rigidbody>();
        player_.useGravity = false;
        boundary_.kXMin = -7.5f;
        boundary_.kXMax = 7.5f;
        boundary_.kZMin = 0f;
        boundary_.kZMax = 7f;

        kSound = SoundManager.Instance;
        kAudioIn = AudioInputAdapter.Instance;
    }

    void Update () {
        if (ingame_) {
            // Move plane
            moveHorizontal = Input.GetAxis("Horizontal") * Time.timeScale;
            moveVertical = Input.GetAxis("Vertical") * Time.timeScale;
            Vector3 movement = new Vector3(moveHorizontal * Constant.kMkSens, 0.0f, 0.0f);
            // Boost the plane
            if (kAudioIn.GetInputVolume(AudioInputAdapter.GetDevice()) >= Constant.kBoostThres) { 
                time_stamp_ = Time.time;
                inBoostMode = true;
            }
            if (Time.time - time_stamp_ > Constant.kBoostDura && inBoostMode) {
                inBoostMode = false;
            }
            if (inBoostMode) {
                movement.z += (kAudioIn.GetInputVolume(AudioInputAdapter.GetDevice()) - player_.position.z / 3) * Constant.kBoostSpeed * Time.deltaTime;
            }
            else {
                movement.z += -(player_.position.z - kAudioIn.GetInputVolume(AudioInputAdapter.GetDevice())) * Time.deltaTime * Constant.kRetrSpeed;
            }
            //movement.x += Random.Range(-0.01f * GameController.birdHitCounter, 0.01f * GameController.birdHitCounter);
            //movement.z += Random.Range(-0.01f * GameController.birdHitCounter, 0.01f * GameController.birdHitCounter);

            MoveTarget(movement);

            // Wobble plane unless middle connection exists
            if (!Input.GetKey(KeyCode.F) && moveHorizontal == 0) {
                //Plane fall
                if (Time.time - crash_ > Constant.kCrashDura) {
                    player_.useGravity = true;
                    kSound.fallingSound();
                    ingame_ = false;
                    StartCoroutine(PlaneDestruct());
                }
                Vector3 randmove = new Vector3(Random.Range(-Constant.kWobAmp * (Time.time - crash_), Constant.kWobAmp * (Time.time - crash_)), 0, 0);
                MoveTarget(randmove);
            }
            else {
                crash_ = Time.time;
            }

            //Plane fall
            if (GameController.birdHitCounter >= Constant.kPlaneLife) {
                player_.useGravity = true;
                kSound.fallingSound();
                ingame_ = false;
                StartCoroutine(PlaneDestruct());
            }
        }
    }

    void FixedUpdate () {
        if (ingame_) {
            //Smooth out the movement
            player_.position = Vector3.SmoothDamp(player_.position, target_, ref current_velocity, 0.3f, Constant.kMaxSpeed);

            //Rotate the plane
            qztarget_ = current_velocity.x * -Constant.kTilt;
            qytarget_ = current_velocity.x * Constant.kTilt;
            player_.rotation = Quaternion.Euler(
                0,
                Mathf.SmoothDamp(player_.rotation.y, qytarget_, ref current_yangular, 0.08f),
                Mathf.SmoothDamp(player_.rotation.z, qztarget_, ref current_zangular, 0.08f));

        }
    }
    #endregion

    #region private_methods

    public void StartGame () {
        ingame_ = true;
        crash_ = Time.time;
        time_stamp_ = Time.time;
    }

    void EndGame () {
        ingame_ = false;
    }

    IEnumerator PlaneDestruct () {
        int count = 100;
        float step = 1 / (float)count;
        while (count > 0) {
            transform.localScale = new Vector3(count * step, count * step, count * step);
            transform.localRotation = Quaternion.Euler(0, count * 5, 0);
            count--;
            yield return null;
        }
        Vector3 ex_pos = new Vector3(transform.position.x, -10, transform.position.z);
        ExplosionController.Instance.Explode(ex_pos);
        yield return new WaitForSeconds(2f);
        kGameCtrl.OnGameOver(false);
        Destroy(gameObject);
    }
    #endregion
}
