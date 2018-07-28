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
        Camera.main.gameObject.SetActive(false);
        //Camera.current.gameObject.SetActive(false);
        playerinstance.GetComponentInChildren<Camera>().gameObject.SetActive(true);
        return playerinstance;
    }

    public GameObject InstantiateRemotePlayer(string name,Vector3 pos, Quaternion rot)
    {
        var newplayer = Instantiate(remotePlayer, pos, rot);
        newplayer.name = name;
        return newplayer;
    }

    public GameObject InstantiateMonster(string name, UInt16 mtype, Vector3 pos, Quaternion rot)
    {
        var newmonster = Instantiate(monsters[mtype-1], pos, rot);
        newmonster.name = name;
        AICharactorController monsterAI = newmonster.GetComponent<AICharactorController>();
        monsterAI.health = 100;
        return newmonster;
    }

    public GameObject InstantiateTrap(string name, UInt16 type, double x, double z)
    {
        Vector3 pos = new Vector3((float)x, 0.01f, (float)z);
        Quaternion rot = new Quaternion();
        var newtrap = Instantiate(traps[type-1], pos, rot);
        newtrap.name = name;
        return newtrap;
    }
}
