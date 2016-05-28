/*
 * Whip Controller
 * Rope is spawned in the scene with no parents because of some physics problems
 */

using UnityEngine;
using System.Collections;

public class WhipController : MonoBehaviour
{
    public GameObject RopePrefab;

    SteamVR_TrackedObject trackedObj;
    GameObject ropeInstance;
    Transform ropeBone0Transform;

    void Start()
    {
        trackedObj = GetComponentInParent<SteamVR_TrackedObject>();

        ropeInstance = Instantiate(RopePrefab);
        if (ropeInstance == null)
        {
            Debug.LogError("Rope instance cannot be null");
        }
        ropeInstance.transform.rotation = Quaternion.AngleAxis(90, Vector3.left);
        ropeBone0Transform = ropeInstance.GetComponentsInChildren<Transform>()[0];
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        ropeBone0Transform.position = trackedObj.transform.position;
    }
}
