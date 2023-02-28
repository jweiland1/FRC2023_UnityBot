using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingStation : MonoBehaviour {

    [SerializeField]
    Material onColor;

    [SerializeField]
    Material offColor;

    public float rotation;


    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        rotation = this.transform.rotation.eulerAngles.z;
        if ( Mathf.Abs(this.transform.rotation.eulerAngles.z ) < 2.5f) {
            this.transform.Find("Light1").GetComponent<Renderer>().material = onColor;
            this.transform.Find("Light2").GetComponent<Renderer>().material = onColor;
            this.transform.Find("Light3").GetComponent<Renderer>().material = onColor;
            this.transform.Find("Light4").GetComponent<Renderer>().material = onColor;
        } else {
            this.transform.Find("Light1").GetComponent<Renderer>().material = offColor;
            this.transform.Find("Light2").GetComponent<Renderer>().material = offColor;
            this.transform.Find("Light3").GetComponent<Renderer>().material = offColor;
            this.transform.Find("Light4").GetComponent<Renderer>().material = offColor;
        }

    }
}
