using UnityEngine;
using System.Collections;
using Valve.VR;

public class TeleporterController : MonoBehaviour {

    public bool ShowTargetPlayArea = true;

    LaserPointerController laserController;
    GameObject cameraRig;
    SteamVR_PlayArea playArea;
    bool lastLaserState, currLaserState;

    // Use this for initialization
    void Start () {
        laserController = GetComponentInParent<LaserPointerController>();
        cameraRig = GameObject.Find("[CameraRig]");
        if (ShowTargetPlayArea)
        {

        }
    }

    // Update is called once per frame
    void Update () {
        currLaserState = laserController.LaserActive;

        if (laserController.LaserActive && laserController.LastLaserValid)
        {
            // Show target play area if laser is on and target is valid
        }

        // Check if user just turned off laser
        if (!currLaserState && lastLaserState && laserController.LastLaserValid)
        {
            DoTeleport();
        }

        lastLaserState = currLaserState;
    }

    void DoTeleport()
    {
        cameraRig.transform.position = laserController.LastLaserHitPoint;
    }
}
