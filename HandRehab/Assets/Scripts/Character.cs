using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float maxHp;
    public float hp;
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
