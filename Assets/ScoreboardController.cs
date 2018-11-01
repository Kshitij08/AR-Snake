using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class ScoreboardController : MonoBehaviour {

    public Camera firstPersonCamera;
    private Anchor anchor;
    private TrackedPlane trackedPlane;

    private float yOffset;

    private int score;

	// Use this for initialization
	void Start () {
		
        foreach(Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }

	}
	
	// Update is called once per frame
	void Update () {
		
        if(Session.Status != SessionStatus.Tracking)
        {
            return;
        }

        if(trackedPlane == null)
        {
            return;
        }

        while(trackedPlane.SubsumedBy != null)
        {
            trackedPlane = trackedPlane.SubsumedBy;
        }


        transform.LookAt(firstPersonCamera.transform);

        transform.position = new Vector3(transform.position.x, trackedPlane.CenterPose.position.y + yOffset, transform.position.z);

	}

    public void SetSelectedPlane(TrackedPlane trackedPlane)
    {
        this.trackedPlane = trackedPlane;
        CreateAnchor();
    }

    void CreateAnchor()
    {

        //Create the position of the anchor by raycasting a point towards top of the screen
        Vector2 pos = new Vector2(Screen.width * 0.5f, Screen.height * 0.9f);
        Ray ray = firstPersonCamera.ScreenPointToRay(pos);
        Vector3 anchorPosition = ray.GetPoint(5f);


        // Create anchor at that point
        if(anchor != null)
        {
            DestroyObject(anchor);
        }

        anchor = trackedPlane.CreateAnchor(new Pose(anchorPosition, Quaternion.identity));

        transform.position = anchorPosition;
        transform.SetParent(anchor.transform);

        yOffset = transform.position.y - trackedPlane.CenterPose.position.y;

        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = true;
        }

    }


    public void SetScore(int score)
    {
        if(this.score != score)
        {
            GetComponentInChildren<TextMesh>().text = "Score: " + score;
            this.score = score;
        }
    }
}
