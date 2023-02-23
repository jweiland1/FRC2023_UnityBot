using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmCommand : MonoBehaviour
{

    public float rotation;
    public float rotationVelocity;
    public float extension;
    public float extensionVelocity;

    public const float rotationTolerance = 1.0f;
    public const float extensionTolerance = 0.1f;

    public ArmControl arm;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void execute( ) {
        if ( arm != null ) {
            arm.targetExtension = extension;
            arm.extensionVelocity = extensionVelocity;
            arm.targetRotation = rotation;
            arm.rotationVelocity = rotationVelocity;
        }
    }


    public bool IsFinished() {
        if (arm != null ) {
            return Mathf.Abs(arm.Rotation() - rotation ) < rotationTolerance
                   && Mathf.Abs( arm.Extension() - extension) < extensionTolerance ;
        }
        return true;
    }

}
