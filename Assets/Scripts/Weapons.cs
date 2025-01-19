using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public Shooting[] Guns = new Shooting[2];
    public int Equip;
    public TextMesh EquipUI;
    public Animator PlayerAnim;
    void Start()
    {
        PlayerAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i <= 9; i++)
        {
            if (i <= Guns.Length && i > 0)
            {
                if (Input.GetKeyDown(i.ToString()))
                {
                    Equip = i-1;
                }
            }
        }

        for (int i = 0; i < Guns.Length; i++)
        {
            if (i == Equip)
            {
                Guns[i].enabled = true;
            }
            else
            {
                Guns[i].enabled = false;
            }
        }

        if (Guns[Equip].GunName == "Revolver")
        {
            PlayerAnim.SetBool("RevolverEquip", true);
        }
        else
        {
            PlayerAnim.SetBool("RevolverEquip", false);
        }

        if (Guns[Equip].GunName == "SMG")
        {
            PlayerAnim.SetBool("SMGEquip", true);
        }
        else
        {
            PlayerAnim.SetBool("SMGEquip", false);
        }

        if (Guns[Equip].GunName == "Shotgun")
        {
            PlayerAnim.SetBool("ShotgunEquip", true);
        }
        else
        {
            PlayerAnim.SetBool("ShotgunEquip", false);
        }

        if (Guns[Equip].GunName == "Rifle")
        {
            PlayerAnim.SetBool("RifleEquip", true);
        }
        else
        {
            PlayerAnim.SetBool("RifleEquip", false);
        }

        EquipUI.text = ($"{Equip + 1}- {Guns[Equip].GunName}");

    }
}
