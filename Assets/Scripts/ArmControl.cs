using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmControl : MonoBehaviour {

    [SerializeField]
    ArticulationBody theArm;

    [SerializeField]
    private ArticulationBody armCenter;

    [SerializeField]
    private ArticulationBody wrist;


    [SerializeField]
    public float targetRotation;

    [SerializeField]
    public float rotationVelocity = 90.0f ;

    [SerializeField]
    public float targetExtension;

    [SerializeField]
    public float extensionVelocity;

    public enum Position { Home, Level1, Level2, Level3, Floor, Substation, Transition };
    public Position state = Position.Home;



    public List<ArmCommand> armCommands;

    // Start is called before the first frame update
    void Start() {
        armCommands = new List<ArmCommand>();
    }


    public float Rotation() {
        return -theArm.xDrive.target;
    }



    void RotateArm() {
        var armDrive = theArm.xDrive;
        float current = armDrive.target;
        var deltaRotation = -targetRotation - current;
        var maxChange = Mathf.Abs(rotationVelocity * Time.deltaTime);

        if (targetRotation < 0.0f) {
            targetRotation = 0.0f;
        }
        if (targetRotation > 120.0f) {
            targetRotation = 120.0f;
        }

        var newTarget = -targetRotation;
        if (Mathf.Abs(deltaRotation) > maxChange) {
            newTarget = current + (maxChange * Mathf.Sign(deltaRotation));
        }

        armDrive.target = newTarget;
        theArm.xDrive = armDrive;

        var wristDrive = wrist.xDrive;
        wristDrive.target = -newTarget;
        wrist.xDrive = wristDrive;
    }



    public float Extension() {
        return -armCenter.xDrive.target;
    }


    void ExtendArm() {
        var extensionDrive = armCenter.xDrive;
        float current = extensionDrive.target;
        var deltaExtension = -targetExtension - current;
        var maxChange = Mathf.Abs(extensionVelocity * Time.deltaTime);

        if (targetExtension < extensionDrive.upperLimit) {
            targetExtension = extensionDrive.upperLimit;
        }
        if (targetExtension < extensionDrive.lowerLimit) {
            targetExtension = extensionDrive.lowerLimit;
        }

        var newTarget = -targetExtension;
        if (Mathf.Abs(deltaExtension) > maxChange) {
            newTarget = current + (maxChange * Mathf.Sign(deltaExtension));
        }

        extensionDrive.target = newTarget;
        armCenter.xDrive = extensionDrive;

    }



    // Update is called once per frame
    void FixedUpdate() {
        while ( armCommands.Count > 0 ) {
            ArmCommand c = armCommands[0];
            if (c.IsFinished()) {
                this.state = c.DesiredState;
                armCommands.RemoveAt(0);
            } else {
                c.execute();
                state = Position.Transition;
                break;
            }
        }
        RotateArm();
        ExtendArm();
    }
}
