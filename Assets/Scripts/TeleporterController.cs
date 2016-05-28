/*
 * Teleporter Controller Script
 * Must be attached to a child object under LaserPointer object
 */

using UnityEngine;
using System.Collections;
using Valve.VR;

public class TeleporterController : MonoBehaviour
{
    public float MaxHeightAdjust = 40f;
    public EVRButtonId TeleportButton = EVRButtonId.k_EButton_SteamVR_Touchpad;
    public float MaxSlope = 50f;
    public GameObject TargetPlayareaPrefab;

    SteamVR_TrackedObject trackedObj;
    LaserPointerController laserController;
    GameObject cameraHead, cameraRig;
    GameObject targetPlayareaInstance;

    bool tpButtonPressed;

    void Start()
    {
        trackedObj = GetComponentInParent<SteamVR_TrackedObject>();
        laserController = GetComponentInParent<LaserPointerController>();
        cameraHead = GameObject.Find("Camera (head)");
        cameraRig = GameObject.Find("[CameraRig]");
        if (TargetPlayareaPrefab != null)
        {
            targetPlayareaInstance = Instantiate(TargetPlayareaPrefab);
            targetPlayareaInstance.transform.SetParent(transform);
            targetPlayareaInstance.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    void Update()
    {
        var inputDevice = SteamVR_Controller.Input((int)trackedObj.index);
        tpButtonPressed = inputDevice.GetPressDown(TeleportButton);
    }

    void FixedUpdate()
    {
        Vector3 targetPos = Vector3.zero;
        // Only allow tp when laser is on and valid, and the adjusted position is valid
        bool targetValid = laserController.LaserActive && laserController.LastLaserValid &&
                GetHeightAdjustedTarget(laserController.LastLaserHitPoint, MaxHeightAdjust, MaxSlope, out targetPos);

        // Check if user pressed teleport teleport button, while laser active and valid
        if (tpButtonPressed && targetValid)
        {
            DoTeleport(targetPos);
        }

        if (targetPlayareaInstance != null)
        {
            targetPlayareaInstance.GetComponent<MeshRenderer>().enabled = targetValid;
            if (targetValid)
            {
                targetPlayareaInstance.transform.position = targetPos;
                targetPlayareaInstance.transform.rotation = Quaternion.identity;
            }
        }
    }

    void DoTeleport(Vector3 targetPos)
    {
        // Find where the rig has to go, where the head is at target
        Vector3 headLocalPos = cameraHead.transform.localPosition;
        cameraRig.transform.position = targetPos - new Vector3(headLocalPos.x, 0, headLocalPos.z);
    }

    /// <summary>
    /// Returns a point on a valid surface underneath the pointer
    /// </summary>
    static bool GetHeightAdjustedTarget(Vector3 pointer, float maxDistance, float maxSlope, out Vector3 target)
    {
        // Raycast downwards to find a valid hit point
        RaycastHit hitInfo;
        // XXX: Point may already be inside a collider. Apply an offset in Y
        if (Physics.Raycast(pointer + Vector3.up * 0.001f, Vector3.down, out hitInfo) &&
            hitInfo.distance <= maxDistance && 
            Vector3.Angle(hitInfo.normal, Vector3.up) <= maxSlope)
        {
            target = hitInfo.point;
            return true;
        }
        else
        {
            target = Vector3.zero;
            return false;
        }
    }
}
