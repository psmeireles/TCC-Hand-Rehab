using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float damage;
    public int element;
    // Start is called before the first frame update
    void Start()
    {
        if (damage == 0) {
            damage = 25;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision) {
        GameObject obj = collision.gameObject;
        if (obj.CompareTag("Player") || obj.CompareTag("Enemy")) {
            Character character = obj.GetComponent<Character>();
            character.Hit(damage, (Element)element);
        }
        Destroy(this.gameObject, 3);
    }
}
