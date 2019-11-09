using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float damage;
    public Element element;
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
            character.Hit(damage, element);
        }
        if (this.tag.Equals("Boulder")) {
            var audio = GetComponent<AudioSource>();
            if (!audio.isPlaying) {
                audio.Play();
            }
        }
        Destroy(this.gameObject, 3);
    }

    private void OnTriggerStay(Collider collision) {
        GameObject obj = collision.gameObject;
        if (obj.CompareTag("Player") || obj.CompareTag("Enemy")) {
            Character character = obj.GetComponent<Character>();
            character.Hit(5f, element);
        }
    }

    public void Shoot(Vector3 origin, Vector3 target)
    {
        Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();
        var direction = target - origin;
        this.gameObject.transform.position = origin + direction.normalized * 2;
        rb.AddForce(direction * 150);
        GameObject.Destroy(this.gameObject, 5);
    }
}
