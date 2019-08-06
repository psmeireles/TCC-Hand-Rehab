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
    public float timeStarted;
    public float timeFinished;

    public Exercise() {
        ResetExercise();
    }

    public void StartExercise(ExerciseType type) {
        this.type = type;
        this.hasStarted = true;
        this.timeStarted = Time.time;
    }

    public void FinishExercise() {
        this.hasFinished = true;
        this.timeFinished = Time.time;
    }

    public void ResetExercise() {
        this.type = ExerciseType.UNDEFINED;
        this.hasStarted = false;
        this.hasFinished = false;
        this.timeStarted = -1;
        this.timeFinished = -1;
    }
}

public class ExerciseDetector : MonoBehaviour {
    public GameObject playerRig;
    public GameObject leftHandObject;
    public GameObject aim;
    public GameObject shield;
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
        shield.SetActive(false);
        currentExercise = new Exercise();
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
            Debug.Log(Mathf.Abs(leftHand.PalmNormal.Roll));
            Debug.Log(currentExercise.type.ToString());
            if (currentExercise.hasStarted && !currentExercise.hasFinished) {
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
                currentExercise.ResetExercise();
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

    bool IsHandPointingForward(Hand hand) {
        float dot = Vector3.Dot(hand.PalmNormal.ToVector3(), camera.transform.forward);
        return dot > 0;
    }

    void ProcessFistExercise(Hand hand) {
        if (IsHandClosed(hand) && IsHandPointingForward(hand)) {
            if (currentExercise.type != ExerciseType.FIST) {
                currentExercise.StartExercise(ExerciseType.FIST);
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
        else if (currentExercise.hasStarted && IsHandOpened(hand) && IsHandPointingForward(hand)){
            currentExercise.FinishExercise();
            if (sphere != null) {
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
        if (!currentExercise.hasStarted && Mathf.Abs(hand.PalmNormal.Roll) > ROTATION_UPPER_TRESHHOLD && IsHandOpened(hand)) {
            currentExercise.StartExercise(ExerciseType.ROTATION);
            shield.SetActive(true);
        }
        if (currentExercise.hasStarted && Mathf.Abs(hand.PalmNormal.Roll) < ROTATION_LOWER_TRESHHOLD && IsHandOpened(hand)){
            currentExercise.FinishExercise();
            shield.SetActive(false);
        }
    }

    void ProcessWristCurlExercise(Hand hand) {
        float angle = Vector3.SignedAngle(hand.Arm.Direction.ToVector3(), hand.Direction.ToVector3(), hand.RadialAxis());
        if (currentExercise.hasStarted) {
            if (aim.transform.localScale.x < 3) {
                aim.transform.localScale += new Vector3(0.01f, 0, 0.01f);
            }
            CameraAim();
        }
        else if (!currentExercise.hasStarted && angle < WRIST_CURL_LOWER_TRESHHOLD && !IsHandPointingForward(hand) && hand.GrabStrength > 0.5) {
            currentExercise.StartExercise(ExerciseType.WRIST_CURL);
        }

        if (currentExercise.hasStarted && angle > WRIST_CURL_UPPER_TRESHHOLD && !IsHandPointingForward(hand) && hand.GrabStrength > 0.5) {
            currentExercise.FinishExercise();
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