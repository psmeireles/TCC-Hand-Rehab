using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExerciseDetector : MonoBehaviour {
    public GameObject playerRig;
    public GameObject leftHandObject;
    public GameObject camera;
    public GameObject aim;

    LeapProvider provider;
    bool closedHand = false;
    bool rotatingHand = false;
    GameObject sphere;

    float GRAB_TRESHHOLD = 0.063f;
    float ROTATION_UPPER_TRESHHOLD = 3f;
    float ROTATION_LOWER_TRESHHOLD = 0.1f;



    // Start is called before the first frame update
    void Start() {
        provider = FindObjectOfType<LeapProvider>();
        aim = GameObject.Instantiate(aim);
        aim.SetActive(false);
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
            bool isPerformingFistExercise = processFistExercise(leftHand);

            if (!isPerformingFistExercise) {
                processRotationExercise(leftHand);
            }
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

    bool processFistExercise(Hand hand) {
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
        else if (closedHand && isHandOpened(hand)){
            closedHand = false;
            if(sphere != null) {
                sphere.GetComponent<Rigidbody>().AddForce(playerRig.transform.rotation * hand.Arm.Direction.ToVector3() * 1000f);
                sphere = null;
            }
        }

        if(sphere != null) {
            sphere.transform.position = hand.PalmPosition.ToVector3();
        }
        return closedHand;
    }

    void cameraAim() {
        RaycastHit hit;
        int layerMask = 1 << 8; // Only collides with ground
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, Mathf.Infinity, layerMask)) {
            Debug.Log("Raycast hit!");
            aim.transform.position = hit.point;
            if (!aim.activeSelf) {
                aim.SetActive(true);
            }
        }
        else {
            aim.SetActive(false);
        }
    }

    GameObject summonBoulder(GameObject aim) {
        GameObject boulder = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        boulder.AddComponent<Rigidbody>();
        boulder.GetComponent<Renderer>().material.color = Color.red;
        boulder.transform.position = aim.transform.position;
        boulder.transform.SetLocalY(25f);
        float radius = aim.transform.localScale.x;
        boulder.transform.localScale *= radius;
        return boulder;
    }

    bool processRotationExercise(Hand hand) {
        if (rotatingHand) {
            if (aim.transform.localScale.x < 3) {
                aim.transform.localScale += new Vector3(0.01f, 0, 0.01f);
            }
            cameraAim();
        }
        if (Mathf.Abs(hand.PalmNormal.Roll) > ROTATION_UPPER_TRESHHOLD) {
            Debug.Log("Started rotation exercise");
            rotatingHand = true;
        }
        else if (rotatingHand && Mathf.Abs(hand.PalmNormal.Roll) < ROTATION_LOWER_TRESHHOLD){
            Debug.Log("Finished rotation exercise");
            rotatingHand = false;
            aim.SetActive(false);
            summonBoulder(aim);
            aim.transform.localScale = new Vector3(0.1f, 0.05f, 0.1f);
            
        }

        return rotatingHand;
    }
}
