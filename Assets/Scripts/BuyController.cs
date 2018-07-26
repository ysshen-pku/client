using System.Collections;
using System;
using UnityEngine;

public class BuyController : MonoBehaviour {

    PlayerInfo playerInfo;
    GameController gameController;
    const float buyCD = 1.0f;
    private float buytimer;

	// Use this for initialization
	void Start () {
        playerInfo = PlayerInfo.getinstance();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }
	
	// Update is called once per frame
	void Update () {
        buytimer += Time.deltaTime;
		if (playerInfo.playerstate == Config.PLAYER_STATE_BUYING)
        {
            if(Input.GetButton("Key1") && (playerInfo.GetMoney() >= Config.TRAP_COST) && buytimer>buyCD)
            {
                TryBuyTrap(1);
            }
            else if (Input.GetButton("Key2") && (playerInfo.GetMoney() >= Config.TRAP_COST) && buytimer > buyCD)
            {
                TryBuyTrap(2);
            }
        }
	}

    void TryBuyTrap(UInt16 traptype)
    {
        buytimer = 0;
        Message msg = new MsgCSBuyTrap(playerInfo.GetPlayerId(), traptype);
        gameController.SendMessage(ref msg);
    }


}
