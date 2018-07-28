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
        if (Input.GetButton("Trap")&&playerInfo.playerstate != Config.PLAYER_STATE_DEAD)
        {
            playerInfo.playerstate = Config.PLAYER_STATE_TRAPING;
            Cursor.visible = true;
        }
        else if (Input.GetButton("Buy") && playerInfo.playerstate != Config.PLAYER_STATE_DEAD)
        {
            Cursor.visible = true;
            playerInfo.playerstate = Config.PLAYER_STATE_BUYING;
        }
        else if (Input.GetButton("KeyR") && playerInfo.playerstate != Config.PLAYER_STATE_DEAD)
        {
            Cursor.visible = false;
            playerInfo.playerstate = Config.PLAYER_STATE_COMMON;
        }
        else if (Input.GetButton("KeyL") && playerInfo.playerstate != Config.PLAYER_STATE_DEAD)
        {
            Message msg = new MsgCSBuy(playerInfo.GetPlayerId(), Config.BUY_COST_EXP, 0);
            gameController.SendMessage(ref msg);
        }
        else if (Input.GetButton("Cancel"))
        {
            
        }
    }


}
