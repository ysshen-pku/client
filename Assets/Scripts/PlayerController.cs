using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private PlayerInfo playerInfo;
    private GameController gameController;

	// Use this for initialization
	void Start () {
        playerInfo = PlayerInfo.getinstance();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        playerInfo.playerstate = Config.PLAYER_STATE_COMMON;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Trap"))
        {
            if (playerInfo.playerstate == Config.PLAYER_STATE_COMMON)
            {
                Cursor.visible = false;
                playerInfo.playerstate = Config.PLAYER_STATE_TRAPING;
            }
            else if (playerInfo.playerstate == Config.PLAYER_STATE_TRAPING)
            {
                playerInfo.playerstate = Config.PLAYER_STATE_COMMON;
            }
        }
        else if (Input.GetButton("Buy"))
        {
            if (playerInfo.playerstate == Config.PLAYER_STATE_COMMON)
            {
                Cursor.visible = true;
                playerInfo.playerstate = Config.PLAYER_STATE_BUYING;
            }
            else
            {
                Cursor.visible = false;
                playerInfo.playerstate = Config.PLAYER_STATE_COMMON;
            }
        }
    }


}
