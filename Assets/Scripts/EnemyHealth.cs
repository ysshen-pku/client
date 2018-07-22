using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    //public AudioClip deathClip;

    Animator anim;
    AudioSource enemyAudio;
    bool isDead;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyAudio = GetComponent<AudioSource>();

        currentHealth = startingHealth;
    }


    public void TakeDamage(int amount)
    {
        if (isDead)
        {
            return;
        }

        enemyAudio.Play();

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        isDead = true;
        anim.SetBool("Death", true);
    }

}

