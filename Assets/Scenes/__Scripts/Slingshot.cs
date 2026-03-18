using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    // Inscribed fields are set in the Unity Inspector pane
    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;
    public MissionDemolition missionDemolition;
    public SlingshotRubberBand rubberBand;
    public SlingshotSFX sfx;

    // fields set dynamically
    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;

    void Awake()
    {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;

        if (missionDemolition == null) missionDemolition = FindFirstObjectByType<MissionDemolition>();
        if (rubberBand == null) rubberBand = GetComponent<SlingshotRubberBand>();
        if (sfx == null) sfx = GetComponent<SlingshotSFX>();
    }

    void OnMouseEnter()
    {
        //print("Slingshot:onMouseEnter");
        launchPoint.SetActive(true);
    }

    void OnMouseExit()
    {
        //print("Slingshot:onMouseExit");
        launchPoint.SetActive(false);
    }

    void OnMouseDown()
    {
        if (missionDemolition != null && missionDemolition.gameOver) return;
        if (missionDemolition != null && missionDemolition.won) return;

        // The player has pressed the mouse button while over Slingshot
        aimingMode = true;
        // Instantiate a Projectile
        projectile = Instantiate(projectilePrefab) as GameObject;
        // Start it at the launchPoint
        projectile.transform.position = launchPos;
        // Set it to isKinematic for now
        projectile.GetComponent<Rigidbody>().isKinematic = true;

        if (rubberBand != null) rubberBand.BeginAiming(projectile.transform);
    }

    void Update()
    {
        // If Slingshot is not in aimingMode, don't run this code
        if (!aimingMode) return;

        // If the game ends while aiming, cleanly stop aiming.
        if (missionDemolition != null && (missionDemolition.gameOver || missionDemolition.won))
        {
            aimingMode = false;
            if (rubberBand != null) rubberBand.EndAiming();
            return;
        }

        // Get the current mouse position in 2D screen coordinates
        Vector3 mousePos2D = Input.mousePosition;

        // Convert the mouse position to 3D world coordinates
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        // Find the delta from the launchPos to the mousePos3D
        Vector3 mouseDelta = mousePos3D - launchPos;

        // Limit mouseDelta to the radius of the Slingshot SphereCollider
        float maxMagnitude = GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        // Move the projectile to this new position
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        // If the player releases the mouse button, launch the projectile
        if (Input.GetMouseButtonUp(0))
        {
            // The mouse has been released
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;
            FollowCam.POI = projectile; // Set the __MainCamera POI
            // Add a ProjectLine to the Projectile
            Instantiate<GameObject>(projLinePrefab, projectile.transform);

            if (rubberBand != null) rubberBand.EndAiming();
            if (sfx != null) sfx.PlaySnap();
            if (missionDemolition != null) missionDemolition.ShotFired();
            projectile = null;
        }
    }
}