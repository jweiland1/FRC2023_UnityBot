using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerInput : MonoBehaviour {

    [SerializeField]
    private ArmControl armControl;

    [SerializeField]
    private ArticulationBody arm;

    [SerializeField]
    private ArticulationBody armCenter;

    [SerializeField]
    private ArticulationBody wrist;

    [SerializeField]
    ClawControl claw;

    //[SerializeField]
    //private ArticulationBody rightClaw;

    //[SerializeField]
    //private ArticulationBody leftClaw;

    [SerializeField]
    private InputActionReference rotateArmAction;

    [SerializeField]
    private InputActionReference extendArmAction;

    [SerializeField]
    float rotationScale = 0.25f;

    [SerializeField]
    float extendScale = 0.25f;

    [SerializeField]
    float extendAmount = 99.0f;

    //public InputAction playerControls;

    InputDevice gamepad;

    InputControl leftY;
    InputControl rightY;
    InputControl toggleClaw;

    //bool clawOpen = false;
    //float clawButtonState = 0.0f;
    //float lastClawButton = 0.0f;


    const float clawOpenAngle = 25.0f;

    RobotActions actions;

    enum Position { Home, Level1, Level2, Level3, Floor, Substation };
    Position state = Position.Home;





    private void Awake() {
        actions = new RobotActions();
    }

    private void OnEnable() {
        actions.Enable();
    }

    private void OnDisable() {
        actions.Disable();
    }


    // Start is called before the first frame update
    void Start() {
        var devices = InputSystem.devices.Where<InputDevice>((id) => id.path.Contains("Logitech"));
        if ( devices.Count() > 1 ) {
            gamepad = devices.Last<InputDevice>();
            leftY = gamepad.allControls.Where<InputControl>((ic) => ic.path.EndsWith("/stick/y")).First();
            rightY = gamepad.allControls.Where<InputControl>((ic) => ic.path.EndsWith("/rz")).First();
            toggleClaw = gamepad["button6"];
        }

        actions.RobotControl.ArmHome.performed += ArmHome_performed;
        actions.RobotControl.ArmGridLevel3.performed += ArmGridLevel3_performed;
        actions.RobotControl.ArmGridLevel2.performed += ArmGridLevel2_performed;
        actions.RobotControl.ArmGridLevel1.performed += ArmGridLevel1_performed;
        actions.RobotControl.ArmFloorPickup.performed += ArmFloorPickup_performed;
        actions.RobotControl.ArmSubstationPickup.performed += ArmSubstationPickup_performed;
        actions.RobotControl.ToggleClaw.performed += ToggleClaw_performed;

    }

    private void ToggleClaw_performed(InputAction.CallbackContext obj) {
        claw.ToggleClaw();
    }

    private void ArmSubstationPickup_performed(InputAction.CallbackContext obj)
    {
        armControl.rotationVelocity = 75.0f;
        armControl.targetRotation = 93.0f;
        armControl.extensionVelocity = 0.3f;
        armControl.targetExtension = 0.0f;
        state = Position.Substation;
    }

    private void ArmFloorPickup_performed(InputAction.CallbackContext obj)
    {
        armControl.rotationVelocity = 75.0f;
        armControl.targetRotation = 30.0f;
        armControl.extensionVelocity = 0.3f;
        armControl.targetExtension = 0.30f;
        state = Position.Floor;
    }

    private void ArmGridLevel1_performed(InputAction.CallbackContext obj)
    {
        armControl.rotationVelocity = 75.0f;
        armControl.targetRotation = 30.0f;
        armControl.extensionVelocity = 0.3f;
        armControl.targetExtension = 0.30f;
        state = Position.Level1;
    }

    private void ArmGridLevel2_performed(InputAction.CallbackContext obj)
    {
        armControl.rotationVelocity = 45.0f;
        armControl.targetRotation = 100.0f;
        armControl.extensionVelocity = 0.1f;
        armControl.targetExtension = 0.05f;
        state = Position.Level2;
    }

    private void ArmGridLevel3_performed(InputAction.CallbackContext obj) {
        armControl.rotationVelocity = 75.0f;
        armControl.targetRotation = 110.0f;
        armControl.extensionVelocity = 0.3f;
        armControl.targetExtension = 0.7f ;
        state = Position.Level3;
        Debug.Log("Level 3 button");
    }

    private void ArmHome_performed(InputAction.CallbackContext obj) {
        armControl.rotationVelocity = 75.0f;
        armControl.targetRotation = 0.0f;
        armControl.extensionVelocity = 1.0f;
        armControl.targetExtension = 0;
        state = Position.Home;
    }

    void RotateArmTo(float angle) {
        armControl.targetRotation = angle;
    }


    // Update is called once per frame
    void FixedUpdate() {

        if ( leftY != null ) {

            float armRotation = (float)leftY.ReadValueAsObject();
            extendAmount = -(float)rightY.ReadValueAsObject();

            var currentRotation = armControl.targetRotation ;
            currentRotation += armRotation * rotationScale;
            RotateArmTo(currentRotation);

            var currentExtension = armControl.targetExtension;
            currentExtension += extendAmount * extendScale;

            armControl.targetExtension = currentExtension;

            extendAmount = currentExtension;

        }


    }
}
