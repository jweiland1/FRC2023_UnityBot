using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using System.Linq;


public class ClawControl : MonoBehaviour {

    [SerializeField]
    private ArticulationBody rightClaw;

    [SerializeField]
    private ArticulationBody leftClaw;

    InputDevice gamepad;
    InputControl toggleClaw;


    public bool clawOpen = false;
    const float clawOpenAngle = 20.0f;

    private GameObject heldObject;
    private Rigidbody heldObjectBody;

    [SerializeField]
    Vector3 offset;

    [SerializeField]
    float rayLength;

    [SerializeField]
    bool autoClose;

    private Transform grabOrigin;

    private bool waitingForObjectToLeave;

    private Transform parent;

    private float openTime;



    public void ToggleClaw() {
        var clawDrive = leftClaw.xDrive;

        clawOpen = !clawOpen;

        float target = 0.0f;
        if (clawOpen) {
            target = clawOpenAngle;
            openTime = Time.realtimeSinceStartup;
        } else {
            target = -clawOpenAngle;
        }

        clawDrive.target = target;
        leftClaw.xDrive = clawDrive;

        clawDrive.target = -target;
        rightClaw.xDrive = clawDrive;

        if (clawOpen && heldObject != null) {
            DropGamePiece();

        } else if (!clawOpen && heldObject == null) {
            Vector3 rayStart = this.transform.position + offset;
            if (Physics.Raycast(grabOrigin.position, this.transform.up, out RaycastHit hit, rayLength)) {
                if (hit.transform.gameObject.tag == "GamePiece") {
                    PickUpGamePiece(hit.transform.gameObject);
                }
            }
        }
    }




    // Start is called before the first frame update
    void Start() {
        grabOrigin = this.transform.Find("GrabOrigin");
    }


    void PickUpGamePiece(GameObject obj) {
        if (heldObject == null) {
            parent = obj.transform.parent;
            heldObject = obj;
            heldObjectBody = obj.GetComponent<Rigidbody>();
            heldObjectBody.useGravity = false;
            heldObjectBody.constraints = RigidbodyConstraints.FreezeAll;
            heldObjectBody.transform.parent = this.transform;
        }
    }


    void DropGamePiece() {
        heldObjectBody.useGravity = true;
        heldObjectBody.constraints = RigidbodyConstraints.None;
        heldObjectBody.transform.parent = parent;
        heldObject = null;
    }

    // Update is called once per frame
    void Update() {

        if (autoClose && clawOpen) {
            if (Time.realtimeSinceStartup - openTime > 2.0f) {
                if (Physics.Raycast(grabOrigin.position, this.transform.up, out RaycastHit hit, rayLength)) {
                    if (hit.transform.gameObject.tag == "GamePiece") {
                        //if (!waitingForObjectToLeave) {
                        ToggleClaw();
                        PickUpGamePiece(hit.transform.gameObject);
                        //waitingForObjectToLeave = true;
                        //}
                    }
                } else {
                    //waitingForObjectToLeave = false;
                }
            }
        }

        //Transform grabOrigin = this.transform.Find("GrabOrigin");
        Debug.DrawLine(
            grabOrigin.position,
            grabOrigin.position + (this.transform.up * rayLength),
            Color.yellow, 2.5f
            );

    }

}
