using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChainLightning : MonoBehaviour {
    List<GameObject> targets;
    public GameObject lightningBolt;
    public float range;
    public float damage;
    // Start is called before the first frame update
    void Start() {
        targets = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        range = range == 0 ? 10 : range;
        damage = damage == 0 ? 10 : damage;
    }

    // Update is called once per frame
    void Update() {

    }

    public void LightItUp(Hand hand) {

        GameObject firstEnemy = FindFirstEnemy(hand, range);
        if (firstEnemy != null) {
            GameObject nextEnemy = firstEnemy;
            Vector3 currentPosition;
            Vector3 nextPosition = hand.PalmPosition.ToVector3();

            List<Vector3> positions = new List<Vector3> {
                nextPosition
            };

            // Finding enemies in chain
            while (nextEnemy != null) {
                nextEnemy.GetComponent<Enemy>().Hit(damage);
                currentPosition = nextPosition;
                nextPosition = nextEnemy.transform.position;
                positions.Add(nextPosition);
                nextEnemy = FindNextEnemy(nextEnemy.transform.position, range);
            }

            // Drawing lightning
            for (int i = 0; i < positions.Count - 1; i++) {
                GameObject lightning = GameObject.Instantiate(lightningBolt);
                var lightningPositions = lightning.GetComponentsInChildren<Transform>();
                lightningPositions[1].position = positions[i];
                lightningPositions[2].position = positions[i + 1];
                Destroy(lightning, 3);
            }
        }
        else { // miss, but cast lightning anyway
            GameObject lightning = GameObject.Instantiate(lightningBolt);
            var lightningPositions = lightning.GetComponentsInChildren<Transform>();
            lightningPositions[1].position = hand.PalmPosition.ToVector3();
            lightningPositions[2].position = hand.PalmPosition.ToVector3() + hand.PalmNormal.ToVector3() * 10;
            Destroy(lightning, 3);
        }

    }

    GameObject FindNextEnemy(Vector3 startPosition, float range) {
        float squaredRange = range * range;
        GameObject closestEnemy = null;
        if (targets.Count > 0) {
            closestEnemy = targets.Where(t => (t.transform.position - startPosition).sqrMagnitude < squaredRange)?
                                  .OrderBy(t => (t.transform.position - startPosition).sqrMagnitude)?
                                  .First();
        }
        if (closestEnemy != null) {
            targets.Remove(closestEnemy);
        }

        return closestEnemy;
    }

    GameObject FindFirstEnemy(Hand hand, float range) {
        float squaredRange = range * range;
        Vector3 startPosition = hand.PalmPosition.ToVector3();
        GameObject closestEnemy = null;
        if (targets.Count > 0) {
            var closeEnemies = targets.Where(t => (t.transform.position - startPosition).sqrMagnitude < squaredRange).ToList();
            if (closeEnemies.Count > 0) {
                var enemiesInFrontOfMe = closeEnemies.Where(t => Vector3.Angle(t.transform.position - startPosition, hand.PalmNormal.ToVector3()) < 30).ToList();
                if (enemiesInFrontOfMe.Count > 0) {
                    closestEnemy =enemiesInFrontOfMe.OrderBy(t => (t.transform.position - startPosition).sqrMagnitude).First();
                }
            }
        }
        if (closestEnemy != null) {
            targets.Remove(closestEnemy);
        }

        return closestEnemy;
    }
}
