using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System;

public class WeaponController : MonoBehaviour
{
    public Transform shootPoint;
    public GameObject [] trap;

    public int damagePerShot = 20;                  // The damage inflicted by each bullet.
        public float timeBetweenBullets = 0.15f;        // The time between each shot.
        public float range = 100f;                      // The distance the gun can fire.
        public float aoeSkillCD = 0.15f;
        public GameObject bomb;

    private GameController gameController;
        float shoottimer;                                    // A timer to determine when to fire.
    float placetimer;
        Ray shootRay = new Ray();                       // A ray from the gun end forwards.
        RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
        int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
        ParticleSystem gunParticles;                    // Reference to the particle system.
        LineRenderer gunLine;                           // Reference to the line renderer.
        AudioSource gunAudio;                           // Reference to the audio source.
     //   Light gunLight;                                 // Reference to the light component.
     //   public Light faceLight;								// Duh
        float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.

        GameObject lastObject;

        float aoeSkillTimer;
        float bombExistTime = 3f;

        bool isTrapping = false;

        void Awake()
        {
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
        placetimer += Time.deltaTime;
        aoeSkillTimer += Time.deltaTime;
        shoottimer += Time.deltaTime;
        // remove the box shader
        lastObject = GameObject.FindGameObjectWithTag("Temp");
        if (lastObject != null)
        {
            DestroyObject(lastObject);
        }
        if (!isTrapping)
        {

            if (Input.GetButton("Trap"))
            {
                Cursor.visible = true;
                isTrapping = true;
                return;
            }
            // If the Fire1 button is being press and it's time to fire...
            if (Input.GetButton("Fire1") && shoottimer >= timeBetweenBullets && Time.timeScale != 0)
            {
                // ... shoot the gun.
                Shoot();
                //TrySkillShoot();
            }
            if (Input.GetButton("Fire2") && aoeSkillTimer >= aoeSkillCD && Time.timeScale != 0)
            {
               // TrySkillShoot();
            }
        }
        // setting trap state
        else
        {
            //Debug.Log("is Traping");
            if (Input.GetButton("Trap"))
            {
                Cursor.visible = false;
                isTrapping = false;
                return;
            }
            if (Input.GetButton("Fire1") && placetimer >= 2f && Time.timeScale != 0)
            {
                TryPlaced();
            }
            shootRay.origin = shootPoint.position;
            shootRay.direction = shootPoint.forward;
            // Perform the raycast against gameobjects on the shootable layer and if it hits something...
            if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
            {
                //Debug.Log("Did Hit");
                if (shootHit.collider.gameObject.tag == "Ground")
                {
                    //Debug.Log("Did Hit Ground");
                    // ... the enemy should take damage.
                    Vector3 placePosition = shootHit.point;
                    placePosition.y += 0.01f;
                    Quaternion rot = new Quaternion();
                    Instantiate(trap[0], placePosition, rot);
                }

            }
        }
        if (shoottimer >= timeBetweenBullets * effectsDisplayTime)
        {
            // ... disable the effects.
            DisableEffects();
        }
    }

    void TryPlaced()
    {
        placetimer = 0;
        shootRay.origin = shootPoint.position;
        shootRay.direction = shootPoint.forward;
        // Perform the raycast against gameobjects on the shootable layer and if it hits something...
        if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
        {
            //Debug.Log("Did Hit");
            if (shootHit.collider.gameObject.tag == "Ground")
            {
                //Debug.Log("Did Hit Ground");
                // ... the enemy should take damage.
                Vector3 pos = shootHit.point;
                Message msg = new MsgCSTrapPlace(gameController.localPlayerId, 0, (double)pos.x, (double)pos.z);
                gameController.SendMessage(ref msg);
                //placePosition.y += 0.01f;
                //Instantiate(trap[1], placePosition, Quaternion.identity);
            }

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
            // Reset the timer.
            shoottimer = 0f;

            // Play the gun shot audioclip.
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
        //shootRay.direction = shootPoint.forward;
        shootRay.direction = Camera.main.transform.forward;

            // Perform the raycast against gameobjects on the shootable layer and if it hits something...
            if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
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
                        Message msg = new MsgCSMonsterDamage(gameController.localPlayerId, mid, (UInt16)damagePerShot, 0,0, 0, 0);
                        gameController.SendMessage(ref msg);
                    }
                    else
                    {// parse error 
                        //Debug.Log("parsing midstr to mid error."+midstr);
                    }
                }
                
                // Set the second position of the line renderer to the point the raycast hit.
                gunLine.SetPosition(1, shootHit.point);
            }
            // If the raycast didn't hit anything on the shootable layer...
            else
            {
                // ... set the second position of the line renderer to the fullest extent of the gun's range.
                gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
            }
        }

        void TrySkillShoot()
        {
        Debug.Log("TryShoot Bomb.", gameObject);

                SkillShoot();
        }

        void SkillShoot()
        {
            aoeSkillTimer = 0;

            GameObject mbomb = Instantiate(bomb, shootPoint.position, shootPoint.rotation);

            mbomb.GetComponent<Rigidbody>().velocity = shootPoint.TransformDirection(Vector3.forward * 10);

            // StartCoroutine("BombFree");
        }

        IEnumerator BombFree()
        {
            yield return new WaitForSeconds(bombExistTime);
            Destroy(bomb);
        }
}

