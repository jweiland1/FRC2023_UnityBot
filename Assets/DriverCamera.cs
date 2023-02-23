using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverCamera : MonoBehaviour {

    [SerializeField]
    GameObject robot;


    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        if (robot != null ) {
            this.transform.LookAt(robot.transform);
        }

    }
}
