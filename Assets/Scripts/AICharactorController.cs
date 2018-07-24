using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;


[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class AICharactorController : MonoBehaviour {

    public UnityEngine.AI.NavMeshAgent agent { get; private set; }
    public Vector3 destination;
    public int health;

    // Use this for initialization
    void Start () {
        agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();

        agent.updateRotation = true;
        agent.updatePosition = true;

	}
	
	// Update is called once per frame
	void Update () {
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
    }

    public void OnDeath()
    {
        //death
        Destroy(gameObject);
    }

    public void OnShoot()
    {

    }

}
