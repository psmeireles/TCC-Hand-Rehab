using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Character
{
    public GameObject player;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Invoke("Shoot", GenerateNextShotInterval());
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    void Shoot() {
        this.transform.LookAt(player.transform);
        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.GetComponent<Renderer>().material.color = Color.blue;
        Rigidbody rb = bullet.AddComponent<Rigidbody>();
        bullet.transform.position = this.transform.position + this.transform.forward;
        rb.AddForce((player.transform.position - this.transform.position) * 150);
        GameObject.Destroy(bullet, 5);
        Invoke("Shoot", GenerateNextShotInterval());
    }

    float GenerateNextShotInterval() {
        return Random.Range(3, 10);
    }
}
