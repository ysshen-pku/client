using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrapController : MonoBehaviour {

    public Transform trapPoint;
    public GameObject[] trap;

    private PlayerInfo playerInfo;
    private GameController gameController;
    Ray shootRay = new Ray();                       // A ray from the gun end forwards.
    RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
    int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
    GameObject lastObject;
    float placetimer;

    private void Awake()
    {
        playerInfo = PlayerInfo.getinstance();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        shootableMask = LayerMask.GetMask("Floor");
    }

	
	// Update is called once per frame
	void Update () {
        lastObject = GameObject.FindGameObjectWithTag("Temp");
        if (lastObject != null)
        {
            DestroyObject(lastObject);
        }
        if (playerInfo.playerstate == Config.PLAYER_STATE_TRAPING)
        {
            if (Input.GetButton("Fire1") && placetimer >= 2f && Time.timeScale != 0)
            {
                TryPlaced();
            }
            shootRay.origin = trapPoint.position;
            shootRay.direction = trapPoint.forward;
            // Perform the raycast against gameobjects on the shootable layer and if it hits something...
            if (Physics.Raycast(shootRay, out shootHit, playerInfo.shootRange, shootableMask))
            {
                //Debug.Log("Did Hit");
                if (shootHit.collider.gameObject.tag == "Ground")
                {
                    //Debug.Log("Did Hit Ground");
                    Vector3 placePosition = shootHit.point;
                    //fix the position 
                    placePosition = GetTrapFixedPos(placePosition);
                    placePosition.y += 0.01f;
                    Quaternion rot = new Quaternion();
                    Instantiate(trap[0], placePosition, rot);
                }

            }
        }
    }

    void TryPlaced()
    {
        placetimer = 0;
        shootRay.origin = trapPoint.position;
        shootRay.direction = trapPoint.forward;
        // Perform the raycast against gameobjects on the shootable layer and if it hits something...
        if (Physics.Raycast(shootRay, out shootHit, playerInfo.shootRange, shootableMask))
        {
            //Debug.Log("Did Hit");
            if (shootHit.collider.gameObject.tag == "Ground")
            {
                //Debug.Log("Did Hit Ground");
                // ... the enemy should take damage.
                Vector3 pos = shootHit.point;

                Message msg = new MsgCSTrapPlace(gameController.localPlayerId, 0, (double)pos.x, (double)pos.z);
                gameController.SendMessage(ref msg);
                //placePosition.y += 0.01f;
                //Instantiate(trap[1], placePosition, Quaternion.identity);
            }

        }
    }

    private Vector3 GetTrapFixedPos(Vector3 pos)
    {
        Vector3 fpos = new Vector3();
        int ax = (int)(5 * (pos.x + 10));
        fpos.x = (float)(ax * 0.2 - 10);

        int az = (int)(5 * (35 - pos.z));
        fpos.z = (float)(35 - az * 0.2);
        
        fpos.y = pos.y;
        return fpos;
    }


}
