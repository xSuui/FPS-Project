using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float range = 100f; // alcance max   
    public int totalBullets = 30; //bala por pente
    public int bulletLeft; //total de balas por pente
    public int currentBullets; //numero de balas no pente atual

    public float fireRate = 0.1f;

    private float fireTimer;

    public Transform shootPoint;
    public ParticleSystem fireEffect;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        currentBullets = totalBullets;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Fire1"))
        {
            if(currentBullets > 0)
            {
                //execute fire
                Fire();
            }
            
        }

        if(fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;
        }
    }

    private void Fire()
    {
        if (fireTimer < fireRate)
        {
            return;
        }

        RaycastHit hit;

        if(Physics.Raycast(shootPoint.position, shootPoint.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
        }
        anim.CrossFadeInFixedTime("Fire", 0.01f);
        fireEffect.Play();
        currentBullets--;
        fireTimer = 0f;
    }
}
