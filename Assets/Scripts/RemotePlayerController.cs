using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RemotePlayerController : MonoBehaviour {

    // this class used to control remote player with msgs from server

    private int health;
    private Quaternion rot;
    private Vector3 pos;
    private Animator animator;
    //private NavMeshAgent agent;

    // Use this for initialization
    void Start () {
        //agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Move(double x, double z, double ry)
    {
        Vector3 newpos = new Vector3((float)x, 0, (float)z);
        Quaternion rot = new Quaternion(0, (float)ry, 0, 0);
        if (Vector3.Distance(newpos,transform.position)<0.3f)
        {
            animator.SetBool("IsWalking", false);
        }
        transform.position = newpos;
        transform.rotation = rot;
        animator.SetBool("IsWalking", true);
        
    }

    public void ChangeHP(int hp)
    {
        health = hp;
    }

    public void OnDeath()
    {
        //
        animator.SetTrigger("Die");
        Destroy(gameObject);
    }

}
