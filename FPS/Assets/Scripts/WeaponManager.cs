using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{

    public GameObject[] weapons;
    public float switchDelay = 1f;
    private int index;
    private bool isSwitching;

    // Start is called before the first frame update
    void Start()
    {
        initializeWeapons();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && !isSwitching)
        {
            index++;

            if(index >= weapons.Length)
            {
                index = 0;
            }

            StartCoroutine(switchWeaponDelay(index));
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && !isSwitching)
        {
            index--;

            if(index < 0)
            {
                index = weapons.Length - 1;
            }

            StartCoroutine(switchWeaponDelay(index));
        }
    }

    IEnumerator switchWeaponDelay(int newIndex)
    {
        isSwitching = true;
        yield return new WaitForSeconds(switchDelay);
        isSwitching = false;
        SwitchWeapons(newIndex);
    }

    void initializeWeapons()
    {
        for(int i = 0; i <weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }

        weapons[0].SetActive(true);
    }

    void SwitchWeapons(int newIndex)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }

        weapons[newIndex].SetActive(true);
    }
}
