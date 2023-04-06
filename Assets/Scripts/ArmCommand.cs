using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmCommand //: MonoBehaviour
{

    public float rotation;
    public float rotationVelocity;
    public float extension;
    public float extensionVelocity;

    public const float rotationTolerance = 1.0f;
    public const float extensionTolerance = 0.01f;

    public ArmControl arm;
    public ArmControl.Position DesiredState; 


    public ArmCommand( ArmControl a, ArmControl.Position s) {
        arm = a;
        arm.state = s;
    }


    public ArmCommand(ArmControl a, ArmControl.Position s, float r, float rv, float e, float ev) {
        arm = a;
        arm.state = s;
        rotation = r;
        rotationVelocity = rv;
        extension = e;
        extensionVelocity = ev;
    }


    public void initialize() {
        arm.state = ArmControl.Position.Transition;
    }


    public void execute( ) {
        if ( arm != null ) {
            arm.targetExtension = extension;
            arm.extensionVelocity = extensionVelocity;
            arm.targetRotation = rotation;
            arm.rotationVelocity = rotationVelocity;
            // arm.setAngle( rotation, rotationVelocity) ;
            // arm.setLength( ... ) ;
        }
    }


    public bool IsFinished() {
        if (arm != null ) {
            return ( rotationVelocity == 0.0f || Mathf.Abs(arm.Rotation() - rotation ) < rotationTolerance)
                   && ( extensionVelocity == 0.0f ||Mathf.Abs( arm.Extension() - extension) < extensionTolerance) ;
        }
        return true;
    }

}
