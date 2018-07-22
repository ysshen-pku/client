using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;


[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
[RequireComponent(typeof(ThirdPersonCharacter))]
public class AICharactorController : MonoBehaviour {

    public UnityEngine.AI.NavMeshAgent agent { get; private set; }
    public ThirdPersonCharacter character { get; private set; }
    private Transform target;
    private Vector3 destination;

    // Use this for initialization
    void Start () {
        agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
        character = GetComponent<ThirdPersonCharacter>();

        agent.updateRotation = false;
        agent.updatePosition = true;

	}
	
	// Update is called once per frame
	void Update () {
        if (target != null)
            agent.SetDestination(target.position);
        else if (destination != null)
            agent.SetDestination(destination);
        // finish the former movement 
        if (agent.remainingDistance > agent.stoppingDistance)
            character.Move(agent.desiredVelocity, false, false);
        else
            character.Move(Vector3.zero, false, false);
    }

    public void SetTarget(string targetName)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            if(player.name == targetName)
            {
                this.target = player.transform;
                break;
            }
        }
    }

    public void SetDestination(Vector3 pos)
    {
        // check pos is possible....
        if (pos != null)
            this.destination = pos;
    }

}
