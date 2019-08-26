using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharType;

public class EnemyCreator : MonoBehaviour
{
    public GameObject enemy;
    public GameObject player;
    public int numberOfEnemies;
    public Terrain terrain;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numberOfEnemies; i++) {
            GameObject copy = GameObject.Instantiate(enemy);
            var enemyInstance = copy.GetComponent<Enemy>();
            enemyInstance.type = new CharType((Element)Random.Range((int)0, (int)4));
            copy.transform.position = player.transform.position + player.transform.forward * 20;
            copy.transform.RotateAround(player.transform.position, Vector3.up, 360 / numberOfEnemies * i);
            Vector3 enemyPosition = copy.transform.position;
            enemyPosition.y = terrain.SampleHeight(enemyPosition) + terrain.transform.position.y + 1;
            copy.transform.position = enemyPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
