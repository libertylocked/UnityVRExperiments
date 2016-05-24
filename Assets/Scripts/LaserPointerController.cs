using UnityEngine;
using System.Collections;
using Valve.VR;

public class LaserPointerController : MonoBehaviour {

    public float MaxLaserDistance = 30f;
    public EVRButtonId PointerButton = EVRButtonId.k_EButton_SteamVR_Touchpad;
    public Material LaserInvalidMaterial, LaserValidMaterial;
    public GameObject PointerTipPrefab;

    SteamVR_TrackedObject trackedObj;
    Transform parentTransform;
    LineRenderer lineRenderer;
    GameObject pointerTipInstance;

    // Properties
    public bool LaserActive
    {
        get;
        private set;
    }
    public bool LastLaserValid
    {
        get;
        private set;
    }
    public Vector3 LastLaserHitPoint
    {
        get;
        private set;
    }
    
    // Use this for initialization
    void Start () {
        trackedObj = GetComponentInParent<SteamVR_TrackedObject>();
        parentTransform = GetComponentInParent<Transform>();
        lineRenderer = GetComponent<LineRenderer>();
        if (PointerTipPrefab != null)
        {
            pointerTipInstance = Instantiate(PointerTipPrefab);
            pointerTipInstance.transform.SetParent(this.transform);
            pointerTipInstance.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        var inputDevice = SteamVR_Controller.Input((int)trackedObj.index);
        LaserActive = inputDevice.GetPress(PointerButton);

        // Turn on/off laser beam line renderer
        lineRenderer.enabled = LaserActive;

        // Update laser
        if (LaserActive)
        {
            // Use raycast tp find the target point
            RaycastHit laserHitInfo;
            if (Physics.Raycast(parentTransform.position, parentTransform.forward, out laserHitInfo, MaxLaserDistance))
            {
                // Use line renderer to render the ray, if hit
                lineRenderer.SetPositions(new Vector3[] { parentTransform.position, laserHitInfo.point });
                lineRenderer.material = LaserValidMaterial;

                LastLaserValid = true;
                LastLaserHitPoint = laserHitInfo.point;
            }
            else
            {
                // No valid target. Draw line towards parent's forward
                Vector3 dummyTarget = parentTransform.position + parentTransform.forward * MaxLaserDistance;
                lineRenderer.SetPositions(new Vector3[] { parentTransform.position, dummyTarget });
                lineRenderer.material = LaserInvalidMaterial;

                LastLaserValid = false;
            }
        }

        // Update laser tip object
        if (pointerTipInstance != null)
        {
            bool shouldShowTip = LaserActive && LastLaserValid;
            // Update active status
            pointerTipInstance.SetActive(shouldShowTip);
            // If active, update position
            if (shouldShowTip) pointerTipInstance.transform.position = LastLaserHitPoint;
        }
    }
}
