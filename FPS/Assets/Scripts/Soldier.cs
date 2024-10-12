using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent navMesh;

    public ParticleSystem fireEffect;

    private GameObject Player;

    public float atkDistance = 10f;
    public float followDistance = 20f;
    public float atkProbality;

    public int damage = 20; //total de dano q dá
    public int health = 100; //total de vida q ele tem

    public Transform shootPoint;
    public float range = 100f;

    public float fireRate = 0.5f;
    private float fireTimer;

    public AudioClip shootAudio;
    private AudioSource audioSource;

    private bool isDead;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        navMesh = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(navMesh.enabled)
        {
            float dist = Vector3.Distance(Player.transform.position, transform.position);
            bool shoot = false;
            bool follow = (dist < followDistance);

            if(follow)
            {
                if(dist < atkDistance)
                {
                    shoot = true;
                    Fire();
                }

                navMesh.SetDestination(Player.transform.position);
                transform.LookAt(Player.transform);
            }

            if(!follow || shoot)
            {
                navMesh.SetDestination(transform.position);
            }

            anim.SetBool("shoot", shoot);
            anim.SetBool("run", follow);
        }

        if(fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;
        }
    }

    public void Fire()
    {
        if(fireTimer < fireRate)
        {
            return;
        }

        RaycastHit hit;

         if(Physics.Raycast(shootPoint.position,shootPoint.forward, out hit, range))
        {
            if(hit.transform.GetComponent<PlayerHealth>())
            {
                hit.transform.GetComponent<PlayerHealth>().ApplyDamage(damage);
            }
        }

        fireEffect.Play();
        PlayShootAudio();
        
        fireTimer = 0;
    }

    public void ApplyDamage(int damage)
    {
        health -= damage;
        //Debug.Log("Hit");

        if(health <= 0 && !isDead)
        {
            navMesh.enabled = false;
            anim.SetBool("shoot", false);
            anim.SetBool("run", false);
            anim.SetTrigger("die");
            isDead = true;
        }
    }

    public void PlayShootAudio()
    {
        audioSource.PlayOneShot(shootAudio);
    }
}
