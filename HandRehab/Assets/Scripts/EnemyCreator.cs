using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCreator : MonoBehaviour
{
    public GameObject enemy;
    public GameObject player;
    public int numberOfEnemies;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numberOfEnemies; i++) {
            GameObject copy = GameObject.Instantiate(enemy);
            copy.transform.position = player.transform.position + player.transform.forward * 20 + Vector3.up * 3;
            copy.transform.RotateAround(player.transform.position, Vector3.up, 360 / numberOfEnemies * i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
