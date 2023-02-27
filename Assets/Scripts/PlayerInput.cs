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


    //const float clawOpenAngle = 25.0f;

    RobotActions actions;






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
        armControl.armCommands.Clear();
        ArmCommand c = new ArmCommand(armControl, ArmControl.Position.Substation);
        c.rotationVelocity = 75.0f;
        c.rotation = 93.0f;
        c.extensionVelocity = 0.3f;
        c.extension = 0.0f;
        armControl.armCommands.Add(c);
    }

    private void ArmFloorPickup_performed(InputAction.CallbackContext obj)
    {
        armControl.armCommands.Clear();
        ArmCommand c = new ArmCommand(armControl, ArmControl.Position.Floor);
        c.rotationVelocity = 75.0f;
        c.rotation = 30.0f;
        c.extensionVelocity = 0.3f;
        c.extension = 0.30f;
        armControl.armCommands.Add(c);
    }

    private void ArmGridLevel1_performed(InputAction.CallbackContext obj)
    {
        armControl.armCommands.Clear();
        ArmCommand c = new ArmCommand(armControl, ArmControl.Position.Level1);
        c.rotationVelocity = 75.0f;
        c.rotation = 30.0f;
        c.extensionVelocity = 0.3f;
        c.extension = 0.30f;
        armControl.armCommands.Add(c);
    }

    private void ArmGridLevel2_performed(InputAction.CallbackContext obj)
    {
        if ( armControl.state == ArmControl.Position.Floor || armControl.state == ArmControl.Position.Level1) {

            armControl.armCommands.Clear();
            ArmCommand c = new ArmCommand(armControl, ArmControl.Position.Transition);
            c.rotationVelocity = 75.0f;
            c.rotation = 100.0f;
            c.extensionVelocity = 0.0f;
            c.extension = 0.0f;
            armControl.armCommands.Add(c);

            c = new ArmCommand(armControl, ArmControl.Position.Level2);
            c.rotationVelocity = 0.0f;
            c.rotation = 100.0f;
            c.extensionVelocity = 0.4f;
            c.extension = 0.05f;
            armControl.armCommands.Add(c);

        } else {
            armControl.armCommands.Clear();
            ArmCommand c = new ArmCommand(armControl, ArmControl.Position.Transition);
            c.rotationVelocity = 75.0f;
            c.rotation = 100.0f;
            c.extensionVelocity = 0.0f;
            c.extension = 0.0f;
            armControl.armCommands.Add(c);

            c = new ArmCommand(armControl, ArmControl.Position.Level2);
            c.rotationVelocity = 0.0f;
            c.rotation = 100.0f;
            c.extensionVelocity = 0.4f;
            c.extension = 0.05f;
            armControl.armCommands.Add(c);
        }
    }

    private void ArmGridLevel3_performed(InputAction.CallbackContext obj) {
        armControl.armCommands.Clear();
        ArmCommand c = new ArmCommand(armControl, ArmControl.Position.Level3);
        c.rotationVelocity = 75.0f;
        c.rotation = 110.0f;
        c.extensionVelocity = 0.3f;
        c.extension = 0.7f ;
        armControl.armCommands.Add(c);
    }

    private void ArmHome_performed(InputAction.CallbackContext obj) {
        if ( armControl.state == ArmControl.Position.Floor || armControl.state == ArmControl.Position.Level1) {
            armControl.armCommands.Clear();
            ArmCommand c = new ArmCommand(armControl, ArmControl.Position.Transition);
            c.rotationVelocity = 50.0f;
            c.rotation = 50.0f;
            c.extensionVelocity = 0.0f;
            c.extension = armControl.Extension();
            armControl.armCommands.Add(c);

            c = new ArmCommand(armControl, ArmControl.Position.Home);
            c.rotationVelocity = 50.0f;
            c.rotation = 0.0f;
            c.extensionVelocity = 0.5f;
            c.extension = 0;
            armControl.armCommands.Add(c);
        } else {
            ArmCommand c = new ArmCommand(armControl, ArmControl.Position.Transition);
            c.rotationVelocity = 0.0f;
            c.rotation = armControl.Rotation();
            c.extensionVelocity = 0.6f;
            c.extension = 0.0f;
            armControl.armCommands.Add(c);

            c = new ArmCommand(armControl, ArmControl.Position.Home);
            c.rotationVelocity = 75.0f;
            c.rotation = 0.0f;
            c.extensionVelocity = 0.5f;
            c.extension = 0;
            armControl.armCommands.Add(c);
        }
    }

    void RotateArmTo(float angle) {
        armControl.armCommands.Clear();
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
