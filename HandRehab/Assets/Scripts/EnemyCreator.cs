using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharType;

public class EnemyCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;

    private GameObject _player;
    private Terrain _terrain;
    // Start is called before the first frame update

    void Start()
    {
        _player = GameObject.Find("Player");
        if (_player == null)
            Debug.LogError("Player is null");

        _terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        if (_terrain == null)
            Debug.LogError("Terrain is null");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemies(Element element, float spawnTime, int numberOfEnemies)
    {
        StartCoroutine(SpawnEnemiesRoutine(element, spawnTime, numberOfEnemies));
    }

    private IEnumerator SpawnEnemiesRoutine(Element element, float spawnTime, int numberOfEnemies)
    {
        yield return new WaitForSeconds(spawnTime);

        GameObject copy = GameObject.Instantiate(_enemyPrefab);
        var enemyInstance = copy.GetComponent<Enemy>();
        enemyInstance.type = new CharType(element);
        copy.GetComponent<Renderer>().material.color = enemyInstance.type.color;
        copy.transform.position = _player.transform.position + Random.onUnitSphere * 20;
        Vector3 enemyPosition = copy.transform.position;
        enemyPosition.y = _terrain.SampleHeight(enemyPosition) + _terrain.transform.position.y + 1;
        copy.transform.position = enemyPosition;
    }
}
