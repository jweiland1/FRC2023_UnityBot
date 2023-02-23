using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;



public class RobotDrive : MonoBehaviour {

    [Header("Wheels")]
    [SerializeField]
    ArticulationBody leftFrontWheel;

    [SerializeField]
    ArticulationBody leftMiddleWheel;

    [SerializeField]
    ArticulationBody leftBackWheel;

    [SerializeField]
    ArticulationBody rightFrontWheel;

    [SerializeField]
    ArticulationBody rightMiddleWheel;

    [SerializeField]
    ArticulationBody rightBackWheel;

    [SerializeField]
    float gain;

    [SerializeField]
    float velocity;


    [SerializeField]
    private InputActionReference driveAction;

    [SerializeField]
    private InputActionReference turnAction;

    [SerializeField]
    private InputActionReference autoSteerAction;

    [SerializeField]
    float yawMultiplier = 1.0f;

    [SerializeField]
    float leftSpeed;

    [SerializeField]
    float rightSpeed;

    [SerializeField]
    float speed;

    [SerializeField]
    float angularVelocity;

    [SerializeField]
    float accelTime = 0.25f;

    [SerializeField]
    GamePieceVision vision;

    [SerializeField]
    ClawControl theClaw;

    InputDevice gamepad;

    InputControl leftY;
    InputControl rightX;

    float x;
    float yaw;
    bool lastAutoSteer = false;

    PIDController pid;

    //public Vector2 moveInput;

    //public void OnDrive(InputValue value) {
    //    moveInput = value.Get<Vector2>();
    //    Debug.Log("OnDrive called");
    //}

    //private void OnDrive2(InputValue value) {
    //    x = value.Get<float>();
    //    //Debug.Log($"XDrive called with {value.Get<float>()}");
    //}

    //private void OnTurn(InputValue value) {
    //    Debug.Log($"Turn called with {value.Get<float>()}");
    //}


    //private void OnAutoSteer(InputValue value) {
    //    Debug.Log($"Autosteer called with value {value}");
    //}




    // Start is called before the first frame update
    void Start() {
        //var devices = InputSystem.devices.Where<InputDevice>((id) => id.path.Contains("Logitech"));
        //if (devices.Count() > 0) {
        //    gamepad = devices.First<InputDevice>();
        //    leftY = gamepad.allControls.Where<InputControl>((ic) => ic.path.EndsWith("/stick/y")).First();
        //    rightX = gamepad.allControls.Where<InputControl>((ic) => ic.path.EndsWith("/z")).First();
        //}

        pid = this.GetComponent<PIDController>();
    }

    class WheelSpeeds {
        public float leftSpeed;
        public float rightSpeed;

        public WheelSpeeds(float l, float r) {
            leftSpeed = l;
            rightSpeed = r;
        }
    }



    private static WheelSpeeds arcadeDriveIK(float xSpeed, float zRotation, bool squareInputs) {

        // Square the inputs (while preserving the sign) to increase fine control
        // while permitting full power.

        xSpeed = Mathf.Clamp(xSpeed, -1.0f, 1.0f);
        zRotation = Mathf.Clamp(zRotation, -1.0f, 1.0f);

        if (squareInputs) {
            xSpeed = xSpeed * xSpeed * Mathf.Sign(xSpeed);
            zRotation = zRotation * zRotation * Mathf.Sign(zRotation);
        }

        float leftSpeed = xSpeed - zRotation;
        float rightSpeed = xSpeed + zRotation;

        // Find the maximum possible value of (throttle + turn) along the vector
        // that the joystick is pointing, then desaturate the wheel speeds
        double greaterInput = Mathf.Max(Mathf.Abs(xSpeed), Mathf.Abs(zRotation));
        double lesserInput = Mathf.Min(Mathf.Abs(xSpeed), Mathf.Abs(zRotation));
        if (greaterInput == 0.0) {
            return new WheelSpeeds(0.0f, 0.0f);
        }
        double saturatedInput = (greaterInput + lesserInput) / greaterInput;
        leftSpeed /= (float)saturatedInput;
        rightSpeed /= (float)saturatedInput;

        return new WheelSpeeds(leftSpeed, rightSpeed);
    }



    float LimitedAccel(float accel, float deltaTime) {
        float maxAccel = (deltaTime / accelTime) * gain;
        if (Mathf.Abs(accel) > maxAccel) {
            return maxAccel * Mathf.Sign(accel);
        }
        return accel;
    }


    float TargetVelocity(float currentVelocity, float desiredVelocity, float deltaTime) {
        float accel = LimitedAccel(desiredVelocity - currentVelocity, deltaTime);
        return currentVelocity + accel;
    }



    // Update is called once per frame
    void FixedUpdate() {

        bool autoSteer = autoSteerAction.action.ReadValue<float>() > 0.0f;

        x = -driveAction.action.ReadValue<float>();
        if (Mathf.Abs(x) < 0.05f) {
            x = 0.0f;
        }

        float speed = 0.0f;
        if (x != 0) {
            speed = (Mathf.Abs(x) / x) * (Mathf.Exp(-400.0f * Mathf.Pow(x / 3.0f, 4.0f)))
                    + (-Mathf.Abs(x) / x);
        }


        yaw = 0.0f;
        if (!autoSteer || !theClaw.clawOpen) {
            yaw = -turnAction.action.ReadValue<float>();

            yawMultiplier = 0.6f + Mathf.Abs(speed) * 0.2f;


            yaw = Mathf.Sign(yaw) * (yaw * yaw) * yawMultiplier;
            if (Mathf.Abs(yaw) < 0.05f) {
                yaw = 0.0f;
            }
            lastAutoSteer = false;
        } else {
            if (!lastAutoSteer) {
                pid.Reset();
            }
            yaw = -pid.Calculate(vision.angleToGamePiece);
            lastAutoSteer = true;
        }

        //double turn = yaw * 1.0f;

        // double turn = 0.0;
        // if (yaw != 0) {
        // turn = (Math.abs(yaw) / yaw) * (Math.exp(-400.0 * Math.pow(yaw / 3.0, 4.0)))
        // + (-Math.abs(yaw) / yaw);
        // }
        // The turn input results in really quick movement of the bot, so
        // let's reduce the turn input and make it even less if we are going faster
        // This is a simple y = mx + b equation to adjust the turn input based on the
        // speed.

        //turn = turn * (-0.2 * Mathf.Abs(speed) + yawMultiplier);


        WheelSpeeds speeds = arcadeDriveIK(speed, yaw, true);

        leftSpeed = gain * speeds.leftSpeed;
        rightSpeed = gain * speeds.rightSpeed;

        float leftVelocity = gain * (float)speeds.leftSpeed;
        float rightVelocity = gain * (float)speeds.rightSpeed;

        var leftDrive = leftBackWheel.xDrive;
        leftDrive.targetVelocity = TargetVelocity(leftDrive.targetVelocity, leftVelocity, Time.deltaTime);

        var rightDrive = rightBackWheel.xDrive;
        rightDrive.targetVelocity = TargetVelocity(rightDrive.targetVelocity, rightVelocity, Time.deltaTime);

        leftBackWheel.xDrive = leftDrive;
        leftMiddleWheel.xDrive = leftDrive;
        leftFrontWheel.xDrive = leftDrive;
        rightBackWheel.xDrive = rightDrive;
        rightMiddleWheel.xDrive = rightDrive;
        rightFrontWheel.xDrive = rightDrive;

        angularVelocity = leftBackWheel.angularVelocity.x;

    }
}
