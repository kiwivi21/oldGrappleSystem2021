using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    #region Variables
    [Header("Keybinds")]
    //[SerializeField] KeyCode grappleKey = KeyCode.LeftControl;

    [Header("Grapple Hook")]
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public Transform gunTip, cam, player;
    [SerializeField] LayerMask grappleableObjects;
    [SerializeField] float maxDistance = 100f;
    [SerializeField] float pullSpeed = 10f; // Speed at which the player is pulled
    [SerializeField] float stopDistance = 1f; // Distance to stop pulling
    private SpringJoint joint;
    private Rigidbody playerRb;

    #endregion


    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        playerRb = player.GetComponent<Rigidbody>(); // Get the player's Rigidbody
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }

        if (joint != null)
        {
            PullPlayerTowardsGrapple();
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxDistance, grappleableObjects))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            //joint.spring = 4.5f;
            //joint.damper = 7f;
            //joint.massScale = 4.5f;

            lr.positionCount = 2;
        }
    }

    void PullPlayerTowardsGrapple()
    {
        // Calculate the direction and distance towards the grapple point
        Vector3 directionToGrapplePoint = (grapplePoint - player.position).normalized;
        float distanceToGrapplePoint = Vector3.Distance(player.position, grapplePoint);

        // Apply force to the player's Rigidbody for pulling while allowing gravity
        if (distanceToGrapplePoint > stopDistance) // Stop pulling when the player is within a certain distance
        {
            playerRb.AddForce(directionToGrapplePoint * pullSpeed, ForceMode.Acceleration);
        }
        else
        {
            StopGrapple();
        }
    }

    void DrawRope()
    {
        if (!joint)
        {
            return;
        }
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, grapplePoint);
    }

    void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplingPoint()
    {
        return grapplePoint;
    }
}
