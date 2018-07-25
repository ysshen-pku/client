using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class AICharactorController : MonoBehaviour {

    public UnityEngine.AI.NavMeshAgent agent { get; private set; }
    public Vector3 destination;
    public Int16 health = 100;
    public UInt16 state = 0;
    public bool isDead;                                // Whether the enemy is dead.

    private Animator animator;
    AudioSource enemyAudio;                     // Reference to the audio source.
    ParticleSystem hitParticles;                // Reference to the particle system that plays when the enemy is damaged.
    CapsuleCollider capsuleCollider;            // Reference to the capsule collider.


    // Use this for initialization
    void Awake () {
        agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyAudio = GetComponent<AudioSource>();
        hitParticles = GetComponentInChildren<ParticleSystem>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        agent.updateRotation = true;
        agent.updatePosition = true;

	}
	
	// Update is called once per frame
	void Update () {
        if (isDead)
        {
            return;
        }
        if (destination != null)
        {
            agent.SetDestination(destination);
        }
            
        // finish the former movement 
        //if (agent.remainingDistance > agent.stoppingDistance)
        //    character.Move(agent.desiredVelocity, false, false);
        //else
        //    character.Move(Vector3.zero, false, false);
    }

    public void SetDestination(Vector3 pos)
    {
        // check pos is possible....
        if (pos != null)
             this.destination = pos;
        //transform.position = pos;
    }

    public void OnDeath()
    {
        // The enemy is dead.
        isDead = true;
        health = 0;

        // Turn the collider into a trigger so shots can pass through it.
        capsuleCollider.isTrigger = true;

        // Tell the animator that the enemy is dead.
        animator.SetTrigger("Dead");

    }

    public void StartSinking()
    {
        agent.enabled = false;
        Destroy(gameObject, 2f);
    }

    public void UpdateState(Int16 hp, UInt16 s)
    {
        health = hp;
        state = s;
    }

}
