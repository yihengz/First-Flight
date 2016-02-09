using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialController : GenericSingleton<TutorialController> {

    public Texture shout;
    public Texture stable;
    public Texture turnleft;
    public Texture turnright;
    public Texture gear;

    private RawImage canvas_;
    private static GameController kGameCtrl;
    private static AudioInputController kAudioCtrl;
    private static bool delay_ = false;
    private enum Status {
        kNone,
        kStable,
        kTurnLeft,
        kTurnRight,
        kGear,
        kShout
    }
    private Status status_ = Status.kNone;
    

    void Awake () {
        canvas_ = GetComponent<RawImage>();
        InitialCanvas();
        kGameCtrl = GameController.Instance;
        kAudioCtrl = AudioInputController.Instance;
    }

    void Update () {
        if (!delay_) {
            switch (status_) {
                case Status.kShout:
                    InitialCanvas();
                    Time.timeScale = 1;
                    kAudioCtrl.StartDetect();
                    break;
                case Status.kStable:
                    if (Input.GetKey(KeyCode.F)) {
                        kGameCtrl.OnGameRes();
                        InitialCanvas();
                    }
                    break;
                case Status.kTurnLeft:
                    if (Input.GetKey(KeyCode.LeftArrow)) {
                        kGameCtrl.OnGameRes();
                        InitialCanvas();
                    }
                    break;
                case Status.kTurnRight:
                    if (Input.GetKey(KeyCode.RightArrow)) {
                        kGameCtrl.OnGameRes();
                        InitialCanvas();
                    }
                    break;
                case Status.kGear:
                    kGameCtrl.OnGameRes();
                    InitialCanvas();
                    break;
                default:
                    break;
            }
        }
    }

    public void Shout () {
        DisplayCanvas(shout, Status.kShout, 200);
    }

    public void Stable () {
        DisplayCanvas(stable, Status.kStable);
    }

	public void TurnLeft () {
        DisplayCanvas(turnleft, Status.kTurnLeft);
    }

    public void TurnRight () {
        DisplayCanvas(turnright, Status.kTurnRight);
    }

    public void Gear () {
        DisplayCanvas(gear, Status.kGear);
    }

    private void DisplayCanvas (Texture tex, Status stat, int delay = 50) {
        canvas_.texture = tex;
        canvas_.color = new Color(1, 1, 1, 1);
        status_ = stat;
        delay_ = true;
        StartCoroutine(Delay(delay));
    }

    private void InitialCanvas () {
        canvas_.texture = null;
        canvas_.color = new Color(0, 0, 0, 0);
        status_ = Status.kNone;
    }

    IEnumerator Delay (int i = 50) {
        while (i > 0) {
            i--;
            yield return null;
        }
        delay_ = false;
        yield return null;
    }
}
