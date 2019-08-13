using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public float maxHp;
    public float hp;
    public Slider hpBar;
    public Image fill;
    // Start is called before the first frame update
    void Start()
    {
        // Fallback hp
        if (maxHp == 0) {
            maxHp = 100;
        }
        hp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        if (hpBar != null) {
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
    }

    public void Hit(int damage) {
        hp -= damage;
        if (hp < 0) {
            hp = 0;
        }
        if (hp == 0) {
            if (this.tag != "Player") {
                this.gameObject.SetActive(false);
                CancelInvoke();
                GameObject.Destroy(this, 5);
            }
        }
    }
}
