using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExerciseDetector : MonoBehaviour {
    public GameObject playerRig;
    public GameObject leftHandObject;

    LeapProvider provider;
    bool closedHand = false;
    GameObject sphere;

    float GRAB_TRESHHOLD = 0.063f;



    // Start is called before the first frame update
    void Start() {
        provider = FindObjectOfType<LeapProvider>();
    }

    // Update is called once per frame
    void Update() {
        Hand rightHand = null;
        Hand leftHand = null;
        Frame frame = provider.CurrentFrame;

        if (frame.Hands.Capacity > 0) {
            foreach (Hand h in frame.Hands) {
                //Debug.Log(h);
                if (h.IsLeft)
                    leftHand = h;
                if (h.IsRight)
                    rightHand = h;
            }
        }

        if(leftHand != null) {
            processClosedHand(leftHand);
        }
    }

    bool isHandClosed(Hand hand) {
        int fingersGrasping = 0;

        if (hand != null) {
            Vector3 palm = new Vector3(hand.PalmPosition.x, hand.PalmPosition.y, hand.PalmPosition.z);
            foreach (Finger f in hand.Fingers) {
                Vector3 tip = new Vector3(f.TipPosition.x, f.TipPosition.y, f.TipPosition.z);
                var dist = Vector3.Distance(tip, palm);
                if (dist < GRAB_TRESHHOLD) {
                    fingersGrasping++;
                }
            }
        }

        return fingersGrasping == 5;
    }

    bool isHandOpened(Hand hand) {
        int fingersExtended = 0;

        if (hand != null) {
            foreach (Finger f in hand.Fingers) {
                if (f.IsExtended) {
                    fingersExtended++;
                }
            }
        }

        return fingersExtended == 5;
    }

    void processClosedHand(Hand hand) {
        if (isHandClosed(hand)) {
            if (!closedHand) {
                closedHand = true;
                sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.AddComponent<Rigidbody>();
                var renderer = sphere.GetComponent<Renderer>();
                renderer.material.color = Color.red;
                sphere.transform.localScale = Vector3.one / 100f;
            }
            else {
                if (sphere.transform.localScale.x < 0.12)
                    sphere.transform.localScale *= (sphere.transform.localScale.x + 0.001f) / sphere.transform.localScale.x;
            }
        }
        else if (isHandOpened(hand)){
            closedHand = false;
            if(sphere != null) {
                sphere.GetComponent<Rigidbody>().AddForce(playerRig.transform.rotation * hand.Arm.Direction.ToVector3() * 1000f);
                sphere = null;
            }
        }

        if(sphere != null) {
            sphere.transform.position = hand.PalmPosition.ToVector3();
        }
    }
}
