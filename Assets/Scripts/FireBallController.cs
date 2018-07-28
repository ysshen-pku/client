using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallController : MonoBehaviour {

    public GameObject boom;
    private PlayerInfo playerInfo;
    private GameController gameController;
    private float boomTime = 0.3f;

	// Use this for initialization
	void Start () {
        playerInfo = PlayerInfo.getinstance();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        Vector3 pos = transform.position;
        Message msg = new MsgCSMonsterDamage(playerInfo.GetPlayerId(), 0, playerInfo.rightShootDamage, Config.FIREBALL_RANGE,
            (double)pos.x, (double)pos.z);
        Vector3 boompos = pos;
        boompos.y = 0;
        Quaternion rot = new Quaternion();
        GameObject obj = Instantiate(boom, boompos, rot);
        gameController.SendMessage(ref msg);
        StartCoroutine("Free", obj);
        gameObject.GetComponent<ParticleSystem>().Stop();
        gameObject.GetComponent<Collider>().enabled = false;
        //Debug.Log("fireball hit");
    }

    IEnumerator Free(GameObject obj)
    {
        yield return new WaitForSeconds(boomTime);
        Destroy(obj);
        Destroy(gameObject);
    }

}
