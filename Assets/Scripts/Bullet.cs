using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public string Weapon = "";
    Quaternion StartRotation;
    void Start()
    {
        StartRotation = transform.rotation;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Weapon == "Revolver")
        {
            
        }
        if (Weapon == "Shotgun")
        {
            
        }
        if (Weapon == "SMG")
        {
            
        }
        if (Weapon == "Rifle")
        {

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.gameObject.tag == "Enemy")
        {
            Color OGcolor = collision.collider.gameObject.GetComponent<SpriteRenderer>().color;
            ParticleSystem ps = collision.collider.GetComponentInChildren<ParticleSystem>();
            ps.transform.rotation = new Quaternion(0, 0, StartRotation.z + Random.Range(-0.2f, 0.2f), Quaternion.identity.w);
            ps.gameObject.transform.position = transform.position;
            ps.Play();
            


        }
        
        Destroy(gameObject);
    }
}
