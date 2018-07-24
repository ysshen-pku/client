using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnManager : MonoBehaviour {

    public GameObject player;
    public GameObject remotePlayer;
    public GameObject [] monsters;
    public GameObject[] traps;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public GameObject InstantiatePlayer(Vector3 pos)
    {
        Quaternion origin = new Quaternion();
        var playerinstance = Instantiate(player, pos, origin);
        playerinstance.name = "localplayer";
        Camera.current.gameObject.SetActive(false);
        playerinstance.GetComponent<Camera>().gameObject.SetActive(true);
        return playerinstance;
    }

    public GameObject InstantiateRemotePlayer(string name,Vector3 pos, Quaternion rot)
    {
        var newplayer = Instantiate(remotePlayer, pos, rot);
        newplayer.name = name;
        return newplayer;
    }

    public GameObject InstantiateMonster(string name, Vector3 pos, Quaternion rot)
    {
        var newmonster = Instantiate(monsters[0], pos, rot);
        newmonster.name = name;
        newmonster.GetComponent<AICharactorController>().health = 100;
        return newmonster;
    }

    public GameObject InstantiateTrap(string name, UInt16 type, double x, double z)
    {
        Vector3 pos = new Vector3((float)x, 0.06f, (float)z);
        Quaternion rot = new Quaternion();
        var newtrap = Instantiate(traps[type], pos, rot);
        newtrap.name = name;
        return newtrap;
    }
}
