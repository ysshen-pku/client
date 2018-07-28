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
    UInt16 traptype = 0;

    private void Awake()
    {
        playerInfo = PlayerInfo.getinstance();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        shootableMask = LayerMask.GetMask("Floor");
    }

	
	// Update is called once per frame
	void Update () {
        placetimer += Time.deltaTime;
        lastObject = GameObject.FindGameObjectWithTag("Temp");
        if (lastObject != null)
        {
            DestroyObject(lastObject);
        }
        if (playerInfo.playerstate == Config.PLAYER_STATE_TRAPING)
        {
            if (Input.GetButton("Fire1") && placetimer >= 2f && Time.timeScale != 0)
            {
                TryPlaced(traptype);
                traptype = 0;
            }
            if (Input.GetButton("Key1"))
            {
                traptype = 1;
            }
            else if(Input.GetButton("Key2"))
            {
                traptype = 2;
            }
            if (traptype != 0)
            {
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
                        int ax = 0, az = 0;
                        GetArrayPos(placePosition, ref ax, ref az);
                        placePosition = GetWorldPos(ax, az);
                        placePosition.y = 0.01f;
                        Quaternion rot = new Quaternion();
                        Instantiate(trap[traptype-1], placePosition, rot);
                    }

                }
            }
        }
    }

    void TryPlaced(UInt16 type)
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
                int ax=0, az=0;
                GetArrayPos(pos, ref ax, ref az);
                Message msg = new MsgCSTrapPlace(playerInfo.GetPlayerId(), type, (UInt16)ax, (UInt16)az);
                gameController.SendMessage(ref msg);
                //placePosition.y += 0.01f;
                //Instantiate(trap[1], placePosition, Quaternion.identity);
            }

        }
    }

    private void GetArrayPos(Vector3 pos,ref int ax,ref int az)
    {
        ax = (int)(5 * (pos.x + 10));
        az = (int)(5 * (35 - pos.z));
    }

    private Vector3 GetWorldPos(int ax, int az)
    {
        Vector3 fpos = new Vector3();
        fpos.x = (float)(ax * 0.2 - 10);
        fpos.z = (float)(35 - az * 0.2);
        return fpos;
    }


}
