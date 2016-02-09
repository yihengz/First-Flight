using UnityEngine;

public static class Constant {

    private static string devstr = Microphone.devices[0];

    public static string kDevStr {
        get { return devstr; }
        set { devstr = value; }
    }
    public static float kMicSens = 1000;
    public static float kLowPass = 1200;
    public static float kHighPass = 100;
    public const float kMkSens = 0.05f;

    public const int kPlaneLife = 27;

    //plane move speed
    public const float kMaxSpeed = 5.0f;
    //plane tilt angle
    public const float kTilt = 10.0f;
    //plane boost threshold
    public const float kBoostThres = 0.8f;
    //plane boost duration
    public const float kBoostDura = 0.02f;
    //plane boost speed
    public const float kBoostSpeed = 5f;
    //plane retrieve speed
    public const float kRetrSpeed = 1f;

    //plane wobble amp
    public const float kWobAmp = 0.1f;
    //plane crash duration
    public const float kCrashDura = 10.0f;

    //plane body wiggle amp
    public const float kWigAmp = 0.004f;
    //plane explode amp
    public const float kExpAmp = 15.0f;

    //bird spawn delay
    public const float kSpawnDelay = 10.0f;
}