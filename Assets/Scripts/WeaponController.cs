using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System;

public class WeaponController : MonoBehaviour
{
    public Transform shootPoint;
    public GameObject fireBall;
    private PlayerInfo playerInfo;
    private GameController gameController;
    float shoottimer;                                    // A timer to determine when to fire.

    Ray shootRay = new Ray();                       // A ray from the gun end forwards.
    RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
    int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
    ParticleSystem gunParticles;                    // Reference to the particle system.
    LineRenderer gunLine;                           // Reference to the line renderer.
    AudioSource gunAudio;                           // Reference to the audio source.
     //   Light gunLight;                                 // Reference to the light component.
     //   public Light faceLight;								// Duh
    float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.
    float aoeSkillTimer;
    float ballExistTime = 5f;


        void Awake()
        {
        playerInfo = PlayerInfo.getinstance();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        // Create a layer mask for the Shootable layer.
        shootableMask = LayerMask.GetMask("Floor");
        Cursor.visible = false;
        // Set up the references.
        gunParticles = GetComponent<ParticleSystem>();
            gunLine = GetComponent<LineRenderer>();
            gunAudio = GetComponent<AudioSource>();
         //   gunLight = GetComponent<Light>();
            //faceLight = GetComponentInChildren<Light> ();
        }


    void Update()
    {

        // Add the time since Update was last called to the timer.
        aoeSkillTimer += Time.deltaTime;
        shoottimer += Time.deltaTime;
        // remove the box shader

        if (playerInfo.playerstate == Config.PLAYER_STATE_COMMON)
        {
            // If the Fire1 button is being press and it's time to fire...
            if (Input.GetButton("Fire1") && shoottimer >= playerInfo.leftShootCD && Time.timeScale != 0)
            {
                // ... shoot the gun.
                Shoot();
                //TrySkillShoot();
            }
            if (Input.GetButton("Fire2") && aoeSkillTimer >= playerInfo.rightShootCD && Time.timeScale != 0)
            {
                //TrySkillShoot();
                Fireball();
            }
        }
        // setting trap state

        if (shoottimer >= playerInfo.leftShootCD * effectsDisplayTime)
        {
            // ... disable the effects.
            DisableEffects();
        }
    }


    public void DisableEffects()
    {

        // Disable the line renderer and the light.
        
            gunLine.enabled = false;
         //   faceLight.enabled = false;
          //  gunLight.enabled = false;
    }


    void Shoot()
        {
            shoottimer = 0f;
            gunAudio.Play();

            // Enable the lights.
         //   gunLight.enabled = true;
        //    faceLight.enabled = true;
            // Stop the particles from playing if they were, then start the particles.
            gunParticles.Stop();
            gunParticles.Play();
            // Enable the line renderer and set it's first position to be the end of the gun.
            gunLine.enabled = true;
            gunLine.SetPosition(0, shootPoint.position);

            // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
            shootRay.origin = shootPoint.position;
        // trying camera forward
        shootRay.direction = Camera.main.transform.forward;

            // Perform the raycast against gameobjects on the shootable layer and if it hits something...
        if (Physics.Raycast(shootRay, out shootHit, playerInfo.shootRange , shootableMask))
        {
            // Try and find an EnemyHealth script on the gameobject hit.
            GameObject monster = shootHit.collider.gameObject;
            // If the EnemyHealth component exist...
            if (monster.tag == "Monster")
            {
                // ... the enemy should take damage.
                    string midstr = monster.name.Substring(1);
                    UInt32 mid = 0;
                    if (UInt32.TryParse(midstr, out mid))
                    {
                        Message msg = new MsgCSMonsterDamage(playerInfo.GetPlayerId(), mid, playerInfo.leftShootDamage,0, 0, 0);
                        gameController.SendMessage(ref msg);
                    }
                monster.GetComponent<AICharactorController>().IsHit(shootHit.point);
            }
                // Set the second position of the line renderer to the point the raycast hit.
                gunLine.SetPosition(1, shootHit.point);
        }
            // If the raycast didn't hit anything on the shootable layer...
        else
        {
                // ... set the second position of the line renderer to the fullest extent of the gun's range.
                gunLine.SetPosition(1, shootRay.origin + shootRay.direction * playerInfo.shootRange);
        }
    }


        void Fireball()
        {
            aoeSkillTimer = 0;

            GameObject fireball = Instantiate(fireBall, shootPoint.position, shootPoint.rotation);

            fireball.GetComponent<Rigidbody>().velocity = shootPoint.TransformDirection(Vector3.forward * 10);

            StartCoroutine("FireBallFree", fireball);
        }

        IEnumerator FireBallFree(GameObject obj)
        {
            yield return new WaitForSeconds(ballExistTime);
            Destroy(obj);
        }
}

