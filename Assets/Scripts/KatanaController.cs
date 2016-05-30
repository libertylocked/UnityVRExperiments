using UnityEngine;
using System.Collections;
using Valve.VR;

public class KatanaController : MonoBehaviour
{
    public EVRButtonId sliceModeButton;
    public float CutPlaneSize = 1f;

    SteamVR_TrackedObject trackedObj;
    Collider bladeCollider;
    Vector3 collisionEnterPos, collisionExitPos;

    void Start()
    {
        trackedObj = GetComponentInParent<SteamVR_TrackedObject>();
        bladeCollider = GetComponentInChildren<Collider>();
    }

    void Update()
    {
        var inputDevice = SteamVR_Controller.Input((int)trackedObj.index);
        if (inputDevice.GetPressDown(sliceModeButton)) bladeCollider.isTrigger = true;
        if (inputDevice.GetPressUp(sliceModeButton)) bladeCollider.isTrigger = false;

        Debug.DrawLine(bladeCollider.transform.position, bladeCollider.transform.position + bladeCollider.transform.up * 1f, Color.red); // towards tip
        Debug.DrawLine(bladeCollider.transform.position, bladeCollider.transform.position + bladeCollider.transform.forward * 1f, Color.blue); // down
    }

    void OnTriggerEnter(Collider other)
    {
        collisionEnterPos = transform.position;
    }

    void OnTriggerExit(Collider other)
    {
        collisionExitPos = transform.position;
        CreateCutPlane(collisionEnterPos, collisionExitPos, bladeCollider.transform.up);
    }

    private void CreateCutPlane(Vector3 startPos, Vector3 endPos, Vector3 forward)
    {
        Vector3 center = Vector3.Lerp(startPos, endPos, .5f);
        Vector3 cut = (endPos - startPos).normalized;
        Vector3 fwd = forward.normalized;
        Vector3 normal = Vector3.Cross(fwd, cut).normalized;

        GameObject goCutPlane = new GameObject("CutPlane", typeof(BoxCollider), typeof(Rigidbody), typeof(SplitterSingleCut));

        goCutPlane.GetComponent<Collider>().isTrigger = true;
        Rigidbody bodyCutPlane = goCutPlane.GetComponent<Rigidbody>();
        bodyCutPlane.useGravity = false;
        bodyCutPlane.isKinematic = true;

        Transform transformCutPlane = goCutPlane.transform;
        transformCutPlane.position = center;
        transformCutPlane.localScale = new Vector3(CutPlaneSize, .01f, CutPlaneSize);
        transformCutPlane.up = normal;
        float angleFwd = Vector3.Angle(transformCutPlane.forward, fwd);
        transformCutPlane.RotateAround(center, normal, normal.y < 0f ? -angleFwd : angleFwd);
    }
}
