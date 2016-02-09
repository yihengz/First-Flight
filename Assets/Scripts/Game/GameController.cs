using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : GenericSingleton<GameController> {
    //Delegates
    public delegate void StartGameHandler ();
    public delegate void EndGameHandler ();
    public event StartGameHandler StartGame;
    public event EndGameHandler EndGame;

    // Vars
    public float startWaitVer, startWaitHor, waveWaitVer, waveWaitHor;
    public Text hudText, hudGameOver;
    public GameObject landscape, cloud, airplane;

    // Counter for GUI text
    public static int birdHitCounter;

    // SpawnController
    private static SpawnController _spawn;
    private static bool ingame_ = false;
    private const float kLandSpeed = 0.4f;
    private const float kStartPos = 38;
    private const float kEndPos = -40;

    private static BackgroundScroll kBack;
    private static AudioInputController kAudioCtrl;
    private static AudioInputAdapter kAudioIn;
    private static SoundManager kSound;
    private static TutorialController kTutorial;

    #region publicmethods
    public void OnGameOver (bool iswin) {
        if (iswin) {
            if (EndGame != null)
                EndGame();
            Application.LoadLevel("goodEnd");
        }
        else {
            if (EndGame != null)
                EndGame();
            Application.LoadLevel("badEnd");
        }
        
    }

    public void OnGamePause () {
        Time.timeScale = 0;
    }

    public void OnGameRes () {
        Time.timeScale = 1;
        PlayerController.Instance.StartGame();
    }

    public void OnGameStart () {
        if (StartGame != null) {
            StartGame();
        }
    }
    #endregion

    void Awake () {
        kAudioCtrl = AudioInputController.Instance;
        kAudioIn = AudioInputAdapter.Instance;
        kSound = SoundManager.Instance;
        StartGame += GameCtrlStart;
        EndGame += GameCtrlEnd;
    }

    void Start () {
        _spawn = SpawnController.Instance;
        kTutorial = TutorialController.Instance;
        birdHitCounter = Constant.kPlaneLife / 3 - 1;
        kTutorial.Shout();
        kAudioCtrl.StopDetect();
        OnGamePause();
    }

    void FixedUpdate () {
        if (ingame_) {
            if (landscape.transform.position.z > kStartPos - 5) { 
                landscape.transform.position += new Vector3(0, 0, -kLandSpeed * Mathf.Pow(Mathf.Abs(landscape.transform.position.z - 32), 3) * Time.deltaTime);
            }
            else
                landscape.transform.position += new Vector3(0, 0, -kLandSpeed * Time.deltaTime);
        }
        if (ingame_ && cloud.transform.localScale.x > 1) {
            cloud.transform.position += new Vector3(0, -0.001f, 0);
            cloud.transform.localScale += new Vector3(-0.003f, 0, -0.003f);
        }
        if (landscape.transform.position.z <= kEndPos + 5) {
            kSound.restartLevelSound();
        }
        if (landscape.transform.position.z <= kEndPos) {
            OnGameOver(true);
        }
    }

    void GameCtrlStart () {
        ingame_ = true;
        StartCoroutine(LoadLevel1());
    }

    void GameCtrlEnd () {
        ingame_ = false;
        StopAllCoroutines();
    }

	IEnumerator LoadLevel2(){
		yield return new WaitForSeconds(Constant.kSpawnDelay);
        kSound.restartLevelSound();
		Vector3 tempPosition = _spawn.spawnValuesVer;
		_spawn.spawnValuesVer = new Vector3 (1, tempPosition.y, tempPosition.z);
        _spawn.SpawnBird(BirdType.Regular, Direction.Vertical, 1);
        _spawn.SpawnBird(BirdType.Gear, Direction.Vertical, 1);
		yield return new WaitForSeconds(Constant.kSpawnDelay);
		_spawn.spawnValuesVer = tempPosition;
		_spawn.SpawnBird(BirdType.Regular, Direction.Vertical, 3);
		yield return new WaitForSeconds(Constant.kSpawnDelay);
		_spawn.SpawnBird(BirdType.Fast, Direction.Horizontal, 1);
        yield return new WaitForSeconds(Constant.kSpawnDelay);
		_spawn.SpawnBird(BirdType.Regular, Direction.Vertical, 1);
		_spawn.SpawnBird(BirdType.Fast, Direction.Horizontal, 1);
        _spawn.SpawnBird(BirdType.Gear, Direction.Vertical, 1);
        yield return new WaitForSeconds(Constant.kSpawnDelay);
		_spawn.SpawnBird(BirdType.Regular, Direction.Vertical, 2);
		_spawn.SpawnBird(BirdType.Fast, Direction.Horizontal, 2);
        yield return new WaitForSeconds(Constant.kSpawnDelay);
		_spawn.SpawnBird(BirdType.Big, Direction.Diagonal, 1);
		_spawn.SpawnBird(BirdType.Fast, Direction.Horizontal, 1);
        yield return new WaitForSeconds(Constant.kSpawnDelay);
		_spawn.SpawnBird(BirdType.Regular, Direction.Vertical, 2);
		_spawn.SpawnBird(BirdType.Fast, Direction.Horizontal, 1);
		_spawn.SpawnBird(BirdType.Big, Direction.Diagonal, 1);
    }

	IEnumerator LoadLevel1(){

		yield return new WaitForSeconds(1);

        OnGamePause();
        kTutorial.Stable();

		Vector3 tempPosition = _spawn.spawnValuesVer;
		_spawn.spawnValuesVer = new Vector3 (1, tempPosition.y, tempPosition.z);
        _spawn.SpawnBird(BirdType.Regular, Direction.Vertical, 1);
        yield return new WaitForSeconds(Constant.kSpawnDelay / 4);

        OnGamePause();
        kTutorial.TurnLeft();

        yield return new WaitForSeconds(Constant.kSpawnDelay / 4);

        OnGamePause();
        kTutorial.TurnRight();


        yield return new WaitForSeconds(Constant.kSpawnDelay / 2);
        _spawn.spawnValuesVer = tempPosition;
		_spawn.SpawnBird(BirdType.Regular, Direction.Vertical, 3);
        yield return new WaitForSeconds(Constant.kSpawnDelay);
        _spawn.SpawnBird(BirdType.Fast, Direction.Horizontal, 1);
        _spawn.SpawnBird(BirdType.Gear, Direction.Vertical, 1);
        yield return new WaitForSeconds(Constant.kSpawnDelay / 3);

        OnGamePause();
        kTutorial.Gear();

        yield return new WaitForSeconds(Constant.kSpawnDelay * 2 / 3);
        _spawn.SpawnBird(BirdType.Regular, Direction.Vertical, 1);
		_spawn.SpawnBird(BirdType.Fast, Direction.Horizontal, 1);
        _spawn.SpawnBird(BirdType.Gear, Direction.Vertical, 1);
        yield return new WaitForSeconds(Constant.kSpawnDelay);

        _spawn.SpawnBird(BirdType.Regular, Direction.Vertical, 2);
		_spawn.SpawnBird(BirdType.Fast, Direction.Horizontal, 2);
        _spawn.SpawnBird(BirdType.Gear, Direction.Vertical, 1);
        yield return new WaitForSeconds(Constant.kSpawnDelay);
		_spawn.SpawnBird(BirdType.Big, Direction.Diagonal, 1);
		_spawn.SpawnBird(BirdType.Fast, Direction.Horizontal, 1);
		yield return new WaitForSeconds(Constant.kSpawnDelay);
		_spawn.SpawnBird(BirdType.Regular, Direction.Vertical, 2);
		_spawn.SpawnBird(BirdType.Fast, Direction.Horizontal, 1);
        _spawn.SpawnBird(BirdType.Big, Direction.Diagonal, 1);
        _spawn.SpawnBird(BirdType.Gear, Direction.Vertical, 2);
        yield return new WaitForSeconds(Constant.kSpawnDelay);
        kSound.endLevelSound();
        yield return new WaitForSeconds(Constant.kSpawnDelay);
        kSound.initiateSpinSound();
		airplane.GetComponent<PlaneFlip> ().enabled = true;
		StartCoroutine(LoadLevel2 ());
	}
}