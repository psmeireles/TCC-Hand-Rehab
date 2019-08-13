using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Character
{
    public GameObject player;
    public Slider hpBar;
    public Image fill;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Shoot", 5, 5);
        hpBar.maxValue = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        hpBar.value = this.hp;
        float hpRatio = hp / maxHp;
        if (hpRatio > 0.5) {
            fill.color = Color.green;
        }
        else if (hpRatio > 0.25) {
            fill.color = Color.yellow;
        }
        else {
            fill.color = Color.red;
        }
    }

    void Shoot() {
        this.transform.LookAt(player.transform);
        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.GetComponent<Renderer>().material.color = Color.blue;
        Rigidbody rb = bullet.AddComponent<Rigidbody>();
        bullet.transform.position = this.transform.position + this.transform.forward;
        rb.AddForce((player.transform.position - this.transform.position) * 250);
        GameObject.Destroy(bullet, 5);
    }
}
