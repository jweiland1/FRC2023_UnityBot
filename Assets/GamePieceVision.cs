using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePieceVision : MonoBehaviour {


    [SerializeField]
    Camera clawCamera;

    [SerializeField]
    GameObject gamePiecesFolder;

    //[SerializeField]
    //GameObject testPiece;

    private bool canSeeGamePiece = false;
    public float angleToGamePiece ;


    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        Debug.DrawLine(
            clawCamera.transform.position,
            clawCamera.transform.position + (clawCamera.transform.forward * 1.0f),
            Color.green, 2.5f
            );

        canSeeGamePiece = false;
        angleToGamePiece = 0.0f;
        float minDistance = float.MaxValue;
        GamePiece selectedPiece;

        var gamePieces = gamePiecesFolder.GetComponentsInChildren<GamePiece>();
        foreach( var gp in gamePieces) {
            Collider collider = gp.GetComponentInChildren<Collider>();
            if (collider != null) {
                var bounds = collider.bounds;
                var cameraFrustum = GeometryUtility.CalculateFrustumPlanes(clawCamera);
                if (GeometryUtility.TestPlanesAABB(cameraFrustum, bounds)) {
                    float distanceToPiece = Vector3.Distance(gp.transform.position, clawCamera.transform.position);
                    if (distanceToPiece < minDistance) {
                        selectedPiece = gp;
                        angleToGamePiece = Vector3.SignedAngle(clawCamera.transform.forward, gp.transform.position - clawCamera.transform.position, clawCamera.transform.up);
                    }
                    canSeeGamePiece = true;
                } 
            }

        }

    }
}
