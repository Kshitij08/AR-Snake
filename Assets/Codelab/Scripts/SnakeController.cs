using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

public class SnakeController : MonoBehaviour
{

    private TrackedPlane trackedPlane;
    public GameObject snakeHeadPrefab;
    private GameObject snakeInstance;

    public GameObject pointer;
    public Camera firstPersonCamera;
    public float speed = 20f;

	// Update is called once per frame
    
	void Update ()
    {
	    if (snakeInstance == null || snakeInstance.active == false)
	    {
	        pointer.SetActive(false);
	        return;
	    }
	    else
	    {
	        pointer.SetActive(true);
	    }

	    TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds;

	    if (Frame.Raycast(Screen.width / 2, Screen.height / 2, raycastFilter, out hit))
	    {
	        Vector3 pt = hit.Pose.position;
	        pt.y = snakeInstance.transform.position.y;
	        Vector3 pos = pointer.transform.position;
	        pos.y = pt.y;
	        pointer.transform.position = pos;

	        pointer.transform.position = Vector3.Lerp(pointer.transform.position, pt, Time.smoothDeltaTime * speed);
	    }

	    float dist = Vector3.Distance(pointer.transform.position, snakeInstance.transform.position) - 0.05f;
	    if (dist < 0)
	    {
	        dist = 0;
	    }

	    Rigidbody rb = snakeInstance.GetComponent<Rigidbody>();
        rb.transform.LookAt((pointer.transform.position));
	    rb.velocity = snakeInstance.transform.localScale.x * snakeInstance.transform.forward * dist / .01f;
	}

    void SpawnSnake()
    {
        if (snakeInstance != null)
        {
            DestroyImmediate(snakeInstance);
        }

        Vector3 pos = trackedPlane.CenterPose.position;
        pos.y += 0.1f;

        snakeInstance = Instantiate(snakeHeadPrefab, pos, Quaternion.identity, transform);
        
        GetComponent<Slithering>().Head = snakeInstance.transform;
        snakeInstance.AddComponent<FoodConsumer>();

    }

    public int GetLength()
    {
        return GetComponent<Slithering>().GetLength();
    }
    
    public void SetPlane(TrackedPlane plane)
    {
        trackedPlane = plane;
        SpawnSnake();
    }

}
