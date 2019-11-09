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
    public CharType type;

    private GameController _gameController;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        _gameController = GameObject.Find("GameController")?.GetComponent<GameController>();
        if (_gameController == null)
            Debug.LogError("GameController is null");

        // Fallback hp
        if (maxHp == 0) {
            maxHp = 100;
        }
        hp = maxHp;
        if (hpBar != null) {
            hpBar.maxValue = maxHp;
            hpBar.value = hp;
            fill.color = Color.green;
        }
        if (type == null) {
            type = new CharType(Element.NORMAL);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
    }

    public void Hit(float damage, Element element) {
        float damageMultiplier = 1;
        if (element == type.getWeakness()) {
            damageMultiplier = 2;
        }
        else if (element == type.element) {
            damageMultiplier = 0.5f;
        }

        hp -= damage * damageMultiplier;
        if (hp < 0) {
            hp = 0;
        }
        if (hp == 0) {
            if (this.tag == "Enemy") {
                this.gameObject.SetActive(false);
                CancelInvoke();
                Enemy.numberOfEnemies--;
                GameObject.Destroy(this);
            }
            else if (this.tag == "Player")
            {
                _gameController.GameOver();
            }
        }
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
}
