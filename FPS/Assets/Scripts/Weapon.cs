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

    private Animator anim;

    private bool isReloading;

    public AudioClip shootSound;
    private AudioSource audioSource;

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
        if(Input.GetButton("Fire1"))
        {
            if(currentBullets > 0)
            {
                //execute fire
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
            Debug.Log(hit.transform.name);
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
