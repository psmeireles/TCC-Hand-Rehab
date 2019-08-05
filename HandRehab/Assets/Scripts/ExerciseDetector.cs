using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExerciseType {
    FIST,
    ROTATION,
    WRIST_CURL,
    UNDEFINED
}

public class Exercise {
    public ExerciseType type;
    public bool hasStarted;
    public bool hasFinished;
}

public class ExerciseDetector : MonoBehaviour {
    public GameObject playerRig;
    public GameObject leftHandObject;
    public GameObject aim;
    public Camera camera;

    LeapProvider provider;
    GameObject sphere;
    Exercise currentExercise;

    float GRAB_TRESHHOLD = 0.063f;
    float ROTATION_UPPER_TRESHHOLD = 3f;
    float ROTATION_LOWER_TRESHHOLD = 0.1f;
    float WRIST_CURL_LOWER_TRESHHOLD = -25f;
    float WRIST_CURL_UPPER_TRESHHOLD = 40f;


    // Start is called before the first frame update
    void Start() {
        provider = FindObjectOfType<LeapProvider>();
        aim = GameObject.Instantiate(aim);
        aim.SetActive(false);
        currentExercise = new Exercise {
            type = ExerciseType.UNDEFINED,
            hasStarted = false,
            hasFinished = false
        };
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

            Debug.DrawLine(leftHand.WristPosition.ToVector3(), (leftHand.WristPosition + leftHand.Direction).ToVector3() * 10, Color.green);
            Debug.DrawLine(leftHand.Arm.Center.ToVector3(), (leftHand.Arm.Center + leftHand.Arm.Direction).ToVector3() * 10, Color.red);
            ;            if (currentExercise.hasStarted && !currentExercise.hasFinished) {
                switch (currentExercise.type) {
                    case ExerciseType.FIST:
                        ProcessFistExercise(leftHand);
                        break;
                    case ExerciseType.ROTATION:
                        ProcessRotationExercise(leftHand);
                        break;
                    case ExerciseType.WRIST_CURL:
                        ProcessWristCurlExercise(leftHand);
                        break;
                }
            }
            else {
                currentExercise.type = ExerciseType.UNDEFINED;
                currentExercise.hasStarted = false;
                currentExercise.hasFinished = false;
                ProcessExercises(leftHand);
            }
        }

    }

    bool IsHandClosed(Hand hand) {
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

    bool IsHandOpened(Hand hand) {
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

    void ProcessFistExercise(Hand hand) {
        if (IsHandClosed(hand)) {
            if (currentExercise.type != ExerciseType.FIST) {
                currentExercise.type = ExerciseType.FIST;
                currentExercise.hasStarted = true;
                sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.AddComponent<Rigidbody>();
                var renderer = sphere.GetComponent<Renderer>();
                renderer.material.color = Color.red;
                sphere.transform.localScale = Vector3.one / 100f;
            }
            else if (sphere?.transform.localScale.x < 0.12) {
                sphere.transform.localScale *= (sphere.transform.localScale.x + 0.001f) / sphere.transform.localScale.x;
            }
        }
        else if (currentExercise.hasStarted && IsHandOpened(hand)){
            currentExercise.hasFinished = true;
            if(sphere != null) {
                sphere.GetComponent<Rigidbody>().AddForce(playerRig.transform.rotation * hand.Arm.Direction.ToVector3() * 1000f);
                sphere = null;
            }
        }

        if(sphere != null) {
            sphere.transform.position = hand.PalmPosition.ToVector3();
        }
    }

    void CameraAim() {
        RaycastHit hit;
        int layerMask = 1 << 8; // Only collides with ground
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, Mathf.Infinity, layerMask)) {
            aim.transform.position = hit.point;
            if (!aim.activeSelf) {
                aim.SetActive(true);
            }
        }
        else {
            aim.SetActive(false);
        }
    }

    GameObject SummonBoulder(GameObject aim) {
        GameObject boulder = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        boulder.AddComponent<Rigidbody>();
        boulder.GetComponent<Renderer>().material.color = Color.red;
        boulder.transform.position = aim.transform.position;
        boulder.transform.SetLocalY(25f);
        float radius = aim.transform.localScale.x;
        boulder.transform.localScale *= radius;
        return boulder;
    }

    void ProcessRotationExercise(Hand hand) {
        if (currentExercise.hasStarted) {
            if (aim.transform.localScale.x < 3) {
                aim.transform.localScale += new Vector3(0.01f, 0, 0.01f);
            }
            CameraAim();
        }
        else if (Mathf.Abs(hand.PalmNormal.Roll) > ROTATION_UPPER_TRESHHOLD && IsHandOpened(hand)) {
            currentExercise.type = ExerciseType.ROTATION;
            currentExercise.hasStarted = true;
        }
        if (currentExercise.hasStarted && Mathf.Abs(hand.PalmNormal.Roll) < ROTATION_LOWER_TRESHHOLD && IsHandOpened(hand)){
            currentExercise.hasFinished = true;
            aim.SetActive(false);
            SummonBoulder(aim);
            aim.transform.localScale = new Vector3(0.1f, 0.05f, 0.1f);
            
        }
    }

    void ProcessWristCurlExercise(Hand hand) {
        Vector3 planeNormal = Vector3.Cross(hand.Arm.Direction.ToVector3(), hand.Direction.ToVector3());
        Vector3 handDir = Vector3.ProjectOnPlane(hand.Direction.ToVector3(), planeNormal);
        Vector3 armDir = Vector3.ProjectOnPlane(hand.Arm.Direction.ToVector3(), planeNormal);
        float angle = Vector3.SignedAngle(hand.Arm.Direction.ToVector3(), hand.Direction.ToVector3(), hand.RadialAxis());
        Debug.Log(angle);
        if (currentExercise.hasStarted) {
            if (aim.transform.localScale.x < 3) {
                aim.transform.localScale += new Vector3(0.01f, 0, 0.01f);
            }
            CameraAim();
        }
        else if (!currentExercise.hasStarted && angle < WRIST_CURL_LOWER_TRESHHOLD) {
            currentExercise.type = ExerciseType.WRIST_CURL;
            currentExercise.hasStarted = true;
        }

        if (currentExercise.hasStarted && angle > WRIST_CURL_UPPER_TRESHHOLD) {
            currentExercise.hasFinished = true;
            aim.SetActive(false);
            SummonBoulder(aim);
            aim.transform.localScale = new Vector3(0.1f, 0.05f, 0.1f);
        }
    }

    void ProcessExercises(Hand hand) {
        ProcessFistExercise(hand);
        if (!currentExercise.hasStarted) {
            ProcessRotationExercise(hand);
            if (!currentExercise.hasStarted) {
                ProcessWristCurlExercise(hand);
            }
        }
    }
}