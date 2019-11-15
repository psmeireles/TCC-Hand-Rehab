using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Character
{
    public static int numberOfEnemies = 0;
    public GameObject player;
    public GameObject projectile;
    
    Slider attackBar;
    float timeToNextShot;
    AudioSource shootSound;

    Canvas canvas;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        numberOfEnemies++;
        timeToNextShot = GenerateNextShotInterval();
        StartCoroutine(Shoot());
        canvas = this.hpBar.GetComponentInParent<Canvas>();
        attackBar = GetComponentsInChildren<Slider>().Where(c => c.name == "Attack Bar").ToArray()[0];
        if(attackBar != null) {
            attackBar.maxValue = timeToNextShot;
            attackBar.value = 0;
        }
        shootSound = GetComponent<AudioSource>();
    }

    private GameObject CreateBulletObject()
    {
        var bullet = GameObject.Instantiate(projectile);
        bullet.GetComponent<Renderer>().material.color = this.type.color;
        bullet.GetComponent<Attack>().element = this.type.element;
        return bullet;
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

    IEnumerator Shoot() {
        while (true)
        {
            yield return new WaitForSeconds(timeToNextShot);
            this.transform.LookAt(player.transform);
            GameObject newBullet = CreateBulletObject();
            newBullet.GetComponent<Attack>().Shoot(this.transform.position, player.transform.position);
            shootSound.Play();

            timeToNextShot = GenerateNextShotInterval();
            if (attackBar != null) {
                attackBar.maxValue = timeToNextShot;
                attackBar.value = 0;
            }
        }
    }

    float GenerateNextShotInterval() {
        return Random.Range(5, 20);
    }

    public static void DestroyAllEnemies()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
            numberOfEnemies--;
        }
    }
}
