using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float range = 100f; // alcance max   
    public int totalBullets = 30; //bala por pente
    public int bulletLeft = 90; //total de balas por pente
    public int currentBullets; //numero de balas no pente atual

    public float fireRate = 0.1f;

    private float fireTimer;

    public Transform shootPoint;
    public ParticleSystem fireEffect;

    public GameObject hitEffect;
    public GameObject bullletImpact;

    private Animator anim;

    private bool isReloading;

    public AudioClip shootSound;
    private AudioSource audioSource;

    public int damage;

    public enum ShootMode
    {
        Auto,
        Semi
    }

    public ShootMode shootMode;
    private bool shootInput;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        currentBullets = totalBullets;
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetButton("Fire1"))
        //{
        //    if(currentBullets > 0)
        //    {
        //        //execute fire
        //        Fire();
        //    } 
        //    else if(bulletLeft > 0)
        //    {
        //        DoReload();
        //    }
            
        //}

        switch(shootMode)
        {
            case ShootMode.Auto:
            shootInput = Input.GetButton("Fire1");
            break;

            case ShootMode.Semi:
            shootInput = Input.GetButtonDown("Fire1");
            break;
        }

        if(shootInput)
        {
            if (currentBullets > 0)
            {
                Fire();
            }
            else if(bulletLeft > 0)
            {
                DoReload();
            }

        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            if (currentBullets < totalBullets && bulletLeft > 0)
            {
                DoReload();
            }
        }

        if(fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;
        }
    }

    private void Fire()
    {
        if (fireTimer < fireRate || isReloading || currentBullets <= 0)
        {
            return;
        }

        RaycastHit hit;

        if(Physics.Raycast(shootPoint.position, shootPoint.transform.forward, out hit, range))
        {
            //Debug.Log(hit.transform.name);
            GameObject hitParticle = Instantiate(hitEffect, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            GameObject bullet = Instantiate(bullletImpact, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
            bullet.transform.SetParent(hit.transform);

            Destroy(hitParticle, 1f);
            Destroy(bullet, 3f);

            if(hit.transform.GetComponent<ObjectHealth>())
            {
                hit.transform.GetComponent<ObjectHealth>().ApplyDamage(damage);
            }
        }
        anim.CrossFadeInFixedTime("Fire", 0.01f);
        fireEffect.Play();
        PlayShootSound();
        currentBullets--;
        fireTimer = 0f;
    }

    void FixedUpdate()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        isReloading = info.IsName("Reload"); //isreloading recebe o valor da execução da animação reload

    }

    void DoReload()
    {
        if(isReloading)
        {
            return;
        }
        anim.CrossFadeInFixedTime("Reload", 0.01f);
    }

    public void Reload()
    {
        if(bulletLeft <= 0)
        {
            return;
        }

        int bulletsToLoad = totalBullets - currentBullets;
        int bulletsToDeduct = (bulletLeft >= bulletsToLoad) ? bulletsToLoad : bulletLeft;

        bulletLeft -= bulletsToDeduct;
        currentBullets += bulletsToDeduct;

    }

    void PlayShootSound()
    {
        audioSource.PlayOneShot(shootSound);
    }
}
