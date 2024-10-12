using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    [Header("Properties")]
    public int damage = 20;
    public float range = 100f; // alcance max   
    public int totalBullets = 30; //bala por pente
    public int bulletLeft = 90; //total de balas por pente
    public int currentBullets; //numero de balas no pente atual

    public float fireRate = 0.1f;

    public float spreadFactor;

    private float fireTimer;

    [Header("Shoot Config")]
    public Transform shootPoint;
    public ParticleSystem fireEffect;

    public GameObject hitEffect;
    public GameObject bullletImpact;

    private Animator anim;

    private bool isReloading;

    [Header("Sounds")]
    public AudioClip shootSound;
    private AudioSource audioSource;

    [Header("Aim")]
    public Vector3 aimpos;
    public float aimSpeed;
    private Vector3 originalPos;

    [Header("UI")]
    public Text ammoText;

    public enum ShootMode
    {
        Auto,
        Semi
    }

    public ShootMode shootMode;
    private bool shootInput;

    void OnEnable()
    {
        UpdateAmmoText();
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        currentBullets = totalBullets;
        originalPos = transform.localPosition;

        UpdateAmmoText();
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

        ToAim();
    }

    private void Fire()
    {
        if (fireTimer < fireRate || isReloading || currentBullets <= 0)
        {
            return;
        }

        RaycastHit hit;

        Vector3 shootDireciton = shootPoint.transform.forward;
        shootDireciton = shootDireciton + shootPoint.TransformDirection(new Vector3(Random.Range(-spreadFactor, spreadFactor), Random.Range(-spreadFactor, spreadFactor)));

        if(Physics.Raycast(shootPoint.position, shootDireciton, out hit, range))
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
            
            if (hit.transform.GetComponent<Soldier>())
            {
                Destroy(bullet);
                hit.transform.GetComponent<Soldier>().ApplyDamage(damage);
            }
        }
        anim.CrossFadeInFixedTime("Fire", 0.01f);
        fireEffect.Play();
        PlayShootSound();
        UpdateAmmoText();
        currentBullets--;
        fireTimer = 0f;
    }

    public void ToAim()
    {
        if(Input.GetButton("Fire2") && !isReloading)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, aimpos, Time.deltaTime * aimSpeed);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, Time.deltaTime * aimSpeed);
        }
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
        UpdateAmmoText();
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

        UpdateAmmoText();

    }

    void PlayShootSound()
    {
        audioSource.PlayOneShot(shootSound);
    }

    void UpdateAmmoText()
    {
        ammoText.text = currentBullets + " / " + bulletLeft;
    }
}
