using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class SceneController : MonoBehaviour {

    public GameObject trackedPlanePrefab;
    public Camera firstPersonCamera;

    public ScoreboardController scoreboard;
    public SnakeController snakeController;

	// Use this for initialization
	void Start () {
        QuitOnConnectionErrors();
	}
	
	// Update is called once per frame
	void Update () {
		if(Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
            return;
        }

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        ProcessPlanes();
        ProcessTouches();
        scoreboard.SetScore(snakeController.GetLength());
	}


    void QuitOnConnectionErrors()
    {
        // Don't update if ARCore is not trackeing
        if(Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            StartCoroutine(CodelabUtils.ToastAndExit("Camera permission is needed for this app", 5));
        }
        else if (Session.Status.IsError())
        {
            //Covers variety of errors
            StartCoroutine(CodelabUtils.ToastAndExit("ARCore encountered a problem, please restart", 5));
        }
    }


    void ProcessPlanes()
    {
        List<TrackedPlane> planes = new List<TrackedPlane>();
        Session.GetTrackables(planes, TrackableQueryFilter.New);

        for (int i = 0; i < planes.Count; i++)
        {
            //Instantiate a plane visualization prefab & set it to track the new plane
            
            GameObject planeObject = Instantiate(trackedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
            planeObject.GetComponent<TrackedPlaneController>().SetTrackedPlane(planes[i]);
        }
    }


    void ProcessTouches()
    {
        Touch touch;
        if(Input.touchCount != 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

        if(Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            SetSelectedPlane(hit.Trackable as TrackedPlane);
        }
    }

    void SetSelectedPlane(TrackedPlane plane)
    {
        scoreboard.SetSelectedPlane(plane);
        snakeController.SetPlane(plane);
        GetComponent<FoodController>().SetSelectedPlane(plane);
    }
}
