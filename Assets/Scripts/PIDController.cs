using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIDController : MonoBehaviour {

    [SerializeField]
    float kP;

    [SerializeField]
    float kI;

    [SerializeField]
    float kD;

    [SerializeField]
    float setPoint;

    private float lastError = 0.0f;
    private float totalError = 0.0f;

    public float Calculate(float measurement) {

        float error = measurement - setPoint;
        float deltaError = error - lastError;
        totalError += error;
        lastError = error;

        var result = error * kP + deltaError * kD + totalError * kI;
        return result;
    }


    public void Reset() {
        totalError = 0.0f;
        lastError = 0.0f;
    }

}
