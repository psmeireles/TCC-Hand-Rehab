using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Character
{
    public GameObject player;
    public GameObject projectile;
    Slider attackBar;

    float timeToNextShot;

    Canvas canvas;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        timeToNextShot = GenerateNextShotInterval();
        Invoke("Shoot", timeToNextShot);
        canvas = this.hpBar.GetComponentInParent<Canvas>();
        attackBar = GetComponentsInChildren<Slider>().Where(c => c.name == "Attack Bar").ToArray()[0];
        if(attackBar != null) {
            attackBar.maxValue = timeToNextShot;
            attackBar.value = 0;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        canvas.transform.LookAt(player.transform);
        if (attackBar != null && timeToNextShot != 0) {
            attackBar.value += Time.deltaTime;
        }
    }

    void Shoot() {
        this.transform.LookAt(player.transform);
        GameObject bullet = GameObject.Instantiate(projectile);
        bullet.GetComponent<Renderer>().material.color = this.type.color;
        bullet.GetComponent<Attack>().element = this.type.element;
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        bullet.transform.position = this.transform.position + this.transform.forward*2;
        rb.AddForce((player.transform.position - this.transform.position) * 150);
        GameObject.Destroy(bullet, 5);
        timeToNextShot = GenerateNextShotInterval();
        if (attackBar != null) {
            attackBar.maxValue = timeToNextShot;
            attackBar.value = 0;
        }
        Invoke("Shoot", timeToNextShot);
    }

    float GenerateNextShotInterval() {
        return Random.Range(5, 20);
    }
}
