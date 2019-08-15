using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChainLightning : MonoBehaviour
{
    List<GameObject> targets;
    public float range;
    // Start is called before the first frame update
    void Start()
    {
        targets = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        range = range == 0 ? 10 : range;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LightItUp(Hand hand) {

        GameObject nextEnemy = FindNextEnemy(hand.PalmPosition.ToVector3(), range);
        Vector3 currentPosition;
        Vector3 nextPosition = hand.PalmPosition.ToVector3();
        while (nextEnemy != null) {
            currentPosition = nextPosition;
            nextPosition = nextEnemy.transform.position;
            GameObject myLine = new GameObject();
            myLine = new GameObject();
            myLine.AddComponent<LineRenderer>();
            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.startColor = Color.blue;
            lr.endColor = Color.blue;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.SetPosition(0, currentPosition);
            lr.SetPosition(1, nextPosition);
            nextEnemy = FindNextEnemy(nextEnemy.transform.position, range);
        }
    }

    GameObject FindNextEnemy(Vector3 startPosition, float range) {
        float squaredRange = range * range;
        GameObject closestEnemy = targets.Where(t => (t.transform.position - startPosition).sqrMagnitude < squaredRange)?
                                         .OrderBy(t => (t.transform.position - startPosition).sqrMagnitude)?
                                         .First();
        if (closestEnemy != null) {
            targets.Remove(closestEnemy);
        }

        return closestEnemy;
    }
}
