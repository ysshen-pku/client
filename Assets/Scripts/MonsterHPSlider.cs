using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHPSlider : MonoBehaviour {

    private Slider HpSlider;
    private AICharactorController monster;

	// Use this for initialization
	void Start () {
        HpSlider = GetComponentInChildren<Slider>();
        monster = GetComponentInParent<AICharactorController>();
        HpSlider.maxValue = 100;
	}

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Camera.main.gameObject.transform.rotation;
        HpSlider.value = monster.health;
    }
}
