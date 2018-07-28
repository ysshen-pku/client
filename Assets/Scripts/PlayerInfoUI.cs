using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour {

    private Slider hpSlider;
    private Text moneyText;
    private Text expText;
    private Mask aoeskillMask;
    private PlayerInfo playerInfo;
    private GameObject buyUI,trapUI;
    private Text spikeTrap;
    private Text freezeTrap;
    private Text round;
    private Text remainMonster;
    private Text upHead;
    private Text levelUpTip;
    private Text levelInfo;

    // Use this for initialization
    void Awake () {
        hpSlider = GetComponentInChildren<Slider>();
        Text[] texts = GetComponentsInChildren<Text>();
        foreach (Text text in texts)
        {
            if (text.name == "Exp")
                expText = text;
            else if (text.name == "Money")
                moneyText = text;
            else if (text.name == "SpikeTrapNum")
                spikeTrap = text;
            else if (text.name == "FreezeTrapNum")
                freezeTrap = text;
            else if (text.name == "Round")
                round = text;
            else if (text.name == "Remain")
                remainMonster = text;
            else if (text.name == "UpHead")
                upHead = text;
            else if (text.name == "LevelUp")
                levelUpTip = text;
            else if (text.name == "LevelInfo")
                levelInfo = text;
        }
        playerInfo = PlayerInfo.getinstance();
        buyUI = GameObject.Find("BuyUI");
        buyUI.SetActive(false);
        trapUI = GameObject.Find("TrapUI");
        trapUI.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (playerInfo.playerstate == Config.PLAYER_STATE_BUYING)
        {
            buyUI.SetActive(true);
            trapUI.SetActive(false);
        }
        else if (playerInfo.playerstate == Config.PLAYER_STATE_TRAPING)
        {
            trapUI.SetActive(true);
            buyUI.SetActive(false);
        }
        else
        {
            buyUI.SetActive(false);
            trapUI.SetActive(false);
        }
        Int16 hp= 0;
        UInt32 coin= 0, exp= 0;
        playerInfo.GetInfo(ref hp, ref coin, ref exp);
        hpSlider.value = hp;
        moneyText.text = "Money: "+coin.ToString();
        expText.text = "Exp: "+ exp.ToString();
        if (exp >= Config.LEVELUP_COST)
            levelUpTip.enabled = true;
        else
            levelUpTip.enabled = false;
        spikeTrap.text = playerInfo.spikeTrapRemain.ToString();
        freezeTrap.text = playerInfo.freezeTrapRemain.ToString();
        round.text = "Round: " + playerInfo.round.ToString();
        remainMonster.text = "Remain: "+ playerInfo.remainMonster.ToString();
        if (playerInfo.playerstate == Config.PLAYER_STATE_DEAD)
            upHead.text = "You dead, Press 'ESC'.";
        else if (playerInfo.gameState == Config.GAME_STATE_WAIT)
            upHead.text = "Waiting Enemy...";
        else if (playerInfo.gameState == Config.GAME_STATE_PLAY)
            upHead.text = "Base HP: " + playerInfo.baseHP.ToString();
        else if (playerInfo.gameState == Config.GAME_STATE_LOSE)
            upHead.text = "You Lose...";
        else if (playerInfo.gameState == Config.GAME_STATE_WIN)
            upHead.text = "You Win!";

        levelInfo.text = " Level: " + playerInfo.locallevel.ToString() +
                           "\n LDamage: " + playerInfo.leftShootDamage.ToString() +
                           "\n RDamage: " + playerInfo.rightShootDamage.ToString();
	}
}
