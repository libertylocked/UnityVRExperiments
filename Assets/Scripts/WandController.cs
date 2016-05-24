using UnityEngine;
using System.Collections;
using Valve.VR;

public class WandController : MonoBehaviour
{

    public float MaxLength;
    public float PerIncrement;
    public EVRButtonId WandButton = EVRButtonId.k_EButton_SteamVR_Trigger;

    SteamVR_TrackedObject trackedObj;
    CapsuleCollider stickCollider;
    float minLength;

    // Use this for initialization
    void Start()
    {
        trackedObj = GetComponentInParent<SteamVR_TrackedObject>();
        stickCollider = GetComponent<CapsuleCollider>();
        minLength = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Get trigger input from vive controller
        var inputDevice = SteamVR_Controller.Input((int)trackedObj.index);
        float newLength;
        float currLength = transform.localScale.y;
        if (inputDevice.GetPress(WandButton))
        {
            // make wand longer
            newLength = Mathf.Min(currLength + PerIncrement * Time.deltaTime, MaxLength);
        }
        else
        {
            // shrink to min
            newLength = Mathf.Max(currLength - PerIncrement * Time.deltaTime, minLength);
        }

        // update scale, position, and collider size
        if (newLength != currLength)
        {
            var newScale = transform.localScale;
            newScale.y = newLength;
            transform.localScale = newScale;
            transform.localPosition = new Vector3(0, 0, transform.localScale.y);
            stickCollider.height = newLength;
        }
    }
}
