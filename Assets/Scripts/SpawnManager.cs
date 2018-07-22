using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

    public GameObject player;
    public GameObject remotePlayer;
    public GameObject [] monster;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InstantiatePlayer(Vector3 pos)
    {
        Quaternion origin = new Quaternion();
        var playerinstance = Instantiate(player, pos, origin);
        player.name = "localplayer";
    }

    public void InstantiateRemotePlayer(string uidstr,Vector3 pos)
    {
        Quaternion rot = new Quaternion();
        var newplayer = Instantiate(remotePlayer, pos, rot);
        newplayer.name = "player" + uidstr;
    }

}
