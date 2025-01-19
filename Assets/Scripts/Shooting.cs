using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    // Start is called before the first frame update
    public string GunName;
    public float BulletForce;
    public float Innacuracy;
    public float ShotDelay;
    public float Recoil;
    public float Kickback;
    public int maxAmmo;
    public bool Automatic;
    public int shots;

    public Transform FirePoint;
    public GameObject bulletPrefab;
    public ParticleSystem Flash;
    public GameObject ReloadUIpos;
    public GameObject ReloadUI;
    public Animator ReloadAnim;
    public AudioSource GunShot;
    public AudioSource ReloadOpen;
    public AudioSource ReloadClose;
    AudioSource LoadBullet;

    int LoadedAmmo;
    float shootTime;
    bool ReloadState;
    public float NormalSpeed;
    public Animator animator;

    public PlayerBaseScript Base;

    private void OnEnable()
    {
        ReloadState = false;
    }

    private void OnDisable()
    {
        if (ReloadUIpos != null)
        {
            ReloadUI.GetComponent<RectTransform>().position = new Vector2(ReloadUIpos.transform.position.x, -20 + ReloadUIpos.transform.position.y);
        }
        
    }

    private void Start()
    {
        LoadedAmmo = maxAmmo;
        shootTime = Time.time;
        LoadBullet = gameObject.GetComponent<AudioSource>();
        ReloadState = false;
        animator = gameObject.GetComponent<Animator>();
        Base = gameObject.GetComponent<PlayerBaseScript>();
    }

    // Update is called once per frame
    void Update()
    {
        ReloadAnim.SetInteger("Ammo", LoadedAmmo);
        
        
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ReloadState = !ReloadState;
            if (ReloadState)
            {
                ReloadOpen.Play();
            }
            else
            {
                ReloadClose.Play();
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && ReloadState == false)
        {
            ReloadState = true;
            ReloadOpen.Play();
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && ReloadState == true)
        {
            ReloadState = false;
            ReloadClose.Play();
        }


        if (ReloadState)
        {
            animator.SetBool("IsReloading", true);
            Base.speed = NormalSpeed * 0.6f;
            
            if (Automatic == false)
            {
                if (Input.GetKeyDown(KeyCode.R) && LoadedAmmo < maxAmmo)
                {
                    LoadBullet.volume = 0.2f;
                    LoadBullet.pitch = 1.2f;
                    LoadedAmmo = LoadedAmmo + 1;
                    LoadBullet.Play();
                }
            }
            else if (Automatic == true)
            {
                if (Input.GetKeyDown(KeyCode.R) && LoadedAmmo < maxAmmo)
                {
                    LoadBullet.volume = 0.2f;
                    LoadBullet.pitch = 1.2f;
                    LoadedAmmo = maxAmmo;
                    LoadBullet.Play();
                }
            }
            

            ReloadUI.GetComponent<Rigidbody2D>().position = ReloadUIpos.transform.position;
        }
        else
        {
            animator.SetBool("IsReloading", false);
            if (Base.IsDodging == false)
            {
                Base.speed = NormalSpeed;
            }

            if (Input.GetButtonDown("Fire1") && LoadedAmmo == 0)
            {
                LoadBullet.pitch = 2f;
                LoadBullet.volume = 0.1f;
                LoadBullet.Play();
            }

            if (((Input.GetButtonDown("Fire1") && Automatic == false) || (Input.GetButton("Fire1") && Automatic == true)) && LoadedAmmo > 0 && shootTime < Time.time)
            {
                Shoot();
            }

            ReloadUI.GetComponent<RectTransform>().position = new Vector2(ReloadUIpos.transform.position.x, -20 + ReloadUIpos.transform.position.y);
        }
        
    }

    public void Shoot()
    {
        for (int i = 1; i <= shots; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, FirePoint.position, FirePoint.rotation);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
            if (shots > 1)
            {
                bullet.transform.localScale = bullet.transform.localScale * 0.7f;
            }
            bulletScript.Weapon = GunName;
            bulletRB.AddForce((FirePoint.up + Random.Range(-Innacuracy, Innacuracy) * FirePoint.right) * BulletForce, ForceMode2D.Impulse);
            Destroy(bullet, 3f);            
        }
        
        Flash.Play();
        GunShot.pitch = Random.Range(0.85f, 0.95f);
        GunShot.Play();
        Innacuracy = Innacuracy * Recoil;
        Invoke("RegainAccuracy", 0.3f);

        

        LoadedAmmo = LoadedAmmo - 1;
        shootTime = Time.time + ShotDelay;

        Base.Kickback(Kickback);
    }
    void RegainAccuracy()
    {
        Innacuracy = Innacuracy / Recoil;
    }
}
