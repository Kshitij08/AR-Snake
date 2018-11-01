using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using GoogleARCore;

public class FoodController : MonoBehaviour
{

    private TrackedPlane trackedPlane;
    private GameObject foodInstance;

    private float foodAge;

    private readonly float maxAge = 10f;

    public GameObject[] foodModels;

	// Update is called once per frame
    
	void Update ()
    {
	    if (trackedPlane == null)
	    {
	        return;
	    }

	    if (trackedPlane.TrackingState != TrackingState.Tracking)
	    {
	        return;
	    }

	    while (trackedPlane.SubsumedBy != null)
	    {
	        trackedPlane = trackedPlane.SubsumedBy;
	    }

	    if (foodInstance == null || foodInstance.activeSelf == false)
	    {
	        SpawnFoodInstance();
	        return;
	    }

	    foodAge += Time.deltaTime;
	    if (foodAge >= maxAge)
	    {
	        Destroy(foodInstance);
	        foodInstance = null;
	    }
	}

    void SpawnFoodInstance()
    {
        GameObject foodItem = foodModels[Random.Range(0, foodModels.Length)];

        List<Vector3> vertices = new List<Vector3>();
        trackedPlane.GetBoundaryPolygon((vertices));
        Vector3 pt = vertices[Random.Range(0, vertices.Count)];
        float dist = Random.Range(0.05f, 1.0f);
        Vector3 position = Vector3.Lerp(pt, trackedPlane.CenterPose.position, dist);
        position.y += 0.05f;

        Anchor anchor = trackedPlane.CreateAnchor(new Pose(position, Quaternion.identity));

        foodInstance = Instantiate(foodItem, position, Quaternion.identity, anchor.transform);

        foodInstance.tag = "food";

        foodInstance.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
        foodInstance.transform.SetParent(anchor.transform);
        foodAge = 0;

        foodInstance.AddComponent<FoodMotion>();
    }

    public void SetSelectedPlane(TrackedPlane selectedPlane)
    {
        trackedPlane = selectedPlane;
    }
    
}
