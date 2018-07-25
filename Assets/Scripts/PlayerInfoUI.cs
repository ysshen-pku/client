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
        }
        playerInfo = PlayerInfo.getinstance();
	}
	
	// Update is called once per frame
	void Update () {
        Int16 hp= 0;
        UInt32 coin= 0, exp= 0;
        playerInfo.GetInfo(ref hp, ref coin, ref exp);
        hpSlider.value = hp;
        moneyText.text = "Money: "+coin.ToString();
        expText.text = "Exp: "+ exp.ToString();
	}
}
