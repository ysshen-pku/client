using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPlayer : MonoBehaviour {

    private GameController gamelogic;
    private PlayerInfo playerInfo;

    float time = 1f;
    const float syncCycle = 0.05f;

    // Use this for initialization
    void Start () {
        gamelogic = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        if (gamelogic == null)
        {
            Debug.Log("Cannot find GameController.");
        }
        playerInfo = PlayerInfo.getinstance();
    }

    // Update is called once per frame
    void Update () {
        if (playerInfo.playerstate == Config.PLAYER_STATE_DEAD)
            return;
        time += Time.deltaTime;
		if (time > syncCycle)
        {
            SyncPosition();

        }
	}

    void SyncPosition()
    {
        Message msg = new MsgCSMoveto(transform.position.x, transform.position.z,transform.rotation.eulerAngles.y);

        gamelogic.SendMessage(ref msg);
    }

}
