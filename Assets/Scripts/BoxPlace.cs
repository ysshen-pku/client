using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPlace : MonoBehaviour {

    public GameObject gameObject;
    public GameObject gameObject2;

    public float range = 100f;                      // The distance the gun can fire.

    Ray shootRay = new Ray();                       // A ray from the gun end forwards.
    RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
    GameObject lastObject;
    int shootableMask;
    float timer;

    // Use this for initialization
    void Start () {
        shootableMask = LayerMask.GetMask("Floor");
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;

        lastObject = GameObject.FindGameObjectWithTag("Temp");
        if (lastObject != null)
        {
            DestroyObject(lastObject);
        }

        if (Input.GetButton("Fire1")&& timer>2.0f)
        {
            Placed();
            return;
        }

        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;
        // Perform the raycast against gameobjects on the shootable layer and if it hits something...
        if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
        {
            Debug.Log("Did Hit");
            if (shootHit.collider.gameObject.tag == "Ground")
            {
                Debug.Log("Did Hit Ground");
                // ... the enemy should take damage.
                Vector3 placePosition = shootHit.point;
                placePosition.y += 1f;
                Instantiate(gameObject, placePosition, Quaternion.identity);
            }

        }
        // If the raycast didn't hit anything on the shootable layer...
        else
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.black);
            //Debug.Log("Did not Hit");
            // ... set the second position of the line renderer to the fullest extent of the gun's range.
        }

    }

    void Placed()
    {
        timer = 0;
        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;
        // Perform the raycast against gameobjects on the shootable layer and if it hits something...
        if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * shootHit.distance, Color.yellow);
            Debug.Log("Did Hit");
            if (shootHit.collider.gameObject.tag == "Ground")
            {
                Debug.Log("Did Hit Ground");
                // ... the enemy should take damage.
                Vector3 placePosition = shootHit.point;
                placePosition.y += 1f;
                Instantiate(gameObject2, placePosition, Quaternion.identity);
            }

        }
    }
}
