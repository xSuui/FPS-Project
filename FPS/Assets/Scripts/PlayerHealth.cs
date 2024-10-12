using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int health = 100;
    public Image bloodImage;
    public Image redImage;

    public int recoveryFactor = 20;

    public float recoveryRate = 5f;
    private float recoveryTimer;

    private Color alphaAmount;

    public bool isDead;

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        recoveryTimer += Time.deltaTime;

        if(recoveryTimer > recoveryRate)
        {
            StartCoroutine(RecoveryHealth());
        }
    }


    public void ApplyDamage(int damage)
    {
        health -= damage;

        alphaAmount = bloodImage.color;
        alphaAmount.a += ((float)damage / 100);

        bloodImage.color = alphaAmount;

        if(redImage.color.a < 0.1f)
        { 
            
            redImage.color = new Color(255f, 0f,0f, alphaAmount.a);
        }

        if (health <= 0)
        {
            GameController.GC.ShowGameOver();
            isDead = true;
            Debug.Log("game over!");
        }

        recoveryTimer = 0f;
    }

    IEnumerator RecoveryHealth()
    {
        while(health < maxHealth)
        {
            health += recoveryFactor;

            alphaAmount.a -= ((float)recoveryFactor / 100);

            bloodImage.color = alphaAmount;
            redImage.color = new Color(255f, 0f, 0f, alphaAmount.a);
            yield return new WaitForSeconds(2f);
        }


    }
}
