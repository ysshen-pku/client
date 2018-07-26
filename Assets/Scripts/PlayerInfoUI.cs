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
    private GameObject buyUI;
    private Text spikeTrap;
    private Text freezeTrap;

	// Use this for initialization
	void Start () {
        hpSlider = GetComponentInChildren<Slider>();
        Text[] texts = GetComponentsInChildren<Text>();
        foreach (Text text in texts)
        {
            if (text.name == "Exp")
            {
                expText = text;
            }
            else if (text.name == "Money")
            {
                moneyText = text;
            }
            else if (text.name == "SpikeTrapNum")
            {
                spikeTrap = text;
            }
            else if (text.name == "FreezeTrapNum")
            {
                freezeTrap = text;
            }
        }
        playerInfo = PlayerInfo.getinstance();
        buyUI = GameObject.Find("BuyUI");
        buyUI.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (playerInfo.playerstate == Config.PLAYER_STATE_BUYING)
        {
            buyUI.SetActive(true);
        }
        else
        {
            buyUI.SetActive(false);
        }
        Int16 hp= 0;
        UInt32 coin= 0, exp= 0;
        playerInfo.GetInfo(ref hp, ref coin, ref exp);
        hpSlider.value = hp;
        moneyText.text = "Money: "+coin.ToString();
        expText.text = "Exp: "+ exp.ToString();
        spikeTrap.text = playerInfo.spikeTrapRemain.ToString();
        freezeTrap.text = playerInfo.freezeTrapRemain.ToString();
	}
}
