using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public TextAsset stageFile;
    public GameObject enemy;
    public GameObject player;
    public Terrain terrain;
    float time;
    // Start is called before the first frame update
    void Start()
    {
        time = 0;

        if (stageFile != null) {
            StreamReader reader = File.OpenText("Assets/Stages/" + stageFile.name + ".txt");
            string line;
            while ((line = reader.ReadLine()) != null) {
                string[] items = line.Split(' ');
                Element element = (Element) System.Enum.Parse(typeof(Element), items[0]);
                int numberOfEnemies = int.Parse(items[1]);
                float spawnTime = float.Parse(items[2]);
                StartCoroutine(SpawnEnemy(element, spawnTime));
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

    }

    IEnumerator SpawnEnemy(Element element, float spawnTime) {
        yield return new WaitForSeconds(spawnTime);

        GameObject copy = GameObject.Instantiate(enemy);
        var enemyInstance = copy.GetComponent<Enemy>();
        enemyInstance.type = new CharType(element);
        copy.GetComponent<Renderer>().material.color = enemyInstance.type.color;
        copy.transform.position = player.transform.position + Random.onUnitSphere * 20;
        Vector3 enemyPosition = copy.transform.position;
        enemyPosition.y = terrain.SampleHeight(enemyPosition) + terrain.transform.position.y + 1;
        copy.transform.position = enemyPosition;
    }
}
