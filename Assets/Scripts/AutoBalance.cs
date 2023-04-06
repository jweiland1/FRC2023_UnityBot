using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoBalance : MonoBehaviour {

    [SerializeField]
    public RobotDrive drive;

    private bool inProgress = false ;
    private float lastError = 0.0f;
    public float speed;
    float balanceTime = 0.0f;
    bool waitingForBalance = false;


    [SerializeField]
    float kP;

    [SerializeField]
    float kD;

    [SerializeField]
    float baseSpeed;

    public float deltaError;


    // Start is called before the first frame update
    void Start() {

    }


    public void InitiateBalance() {
        inProgress = true;
        waitingForBalance = false;
        lastError = 0.0f;
    }

    // Update is called once per frame
    void Update() {
        if (drive == null) {
            return;
        }
        if ( inProgress ) {
            float error = -drive.Pitch();
            deltaError = error - lastError;
            lastError = error;
            if ( Mathf.Abs(error) > 2.0f) {
                //if (Mathf.Abs(error) > 5.0f) {
                //    //speed = Mathf.Sign(error) * kP + deltaError * kD;
                //    speed = Mathf.Sign(error) * baseSpeed + deltaError * kD;
                //} else {
                //    speed = error * kP + deltaError * kD;
                //}
                if (Mathf.Sign(deltaError) != Mathf.Sign(error)) {
                    speed = 0.0f;
                } else {
                    speed = Mathf.Sign(error) * baseSpeed;
                }
                waitingForBalance = false;
            } else {
                if ( !waitingForBalance) {
                    waitingForBalance = true;
                    balanceTime = Time.fixedTime;
                    deltaError = 0.0f;
                }
                if ( Time.fixedTime - balanceTime > 2.0f) {
                    inProgress = false;
                    speed = 0.0f;
                }
            }
            drive.Drive(new RobotDrive.WheelSpeeds(speed, speed));
        }

    }
}
