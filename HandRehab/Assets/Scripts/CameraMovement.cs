using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    Camera mycam;
    bool movementEnabled = false;
    // Start is called before the first frame update
    void Start()
    {
        mycam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float sensitivity = 0.05f;
        if (Input.GetMouseButtonDown(0)) {
            movementEnabled = !movementEnabled;
        }

        if (movementEnabled) {
            Vector3 vp = mycam.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mycam.nearClipPlane));
            vp.x -= 0.5f;
            vp.y -= 0.5f;
            vp.x *= sensitivity;
            vp.y *= sensitivity;
            vp.x += 0.5f;
            vp.y += 0.5f;
            Vector3 sp = mycam.ViewportToScreenPoint(vp);

            Vector3 v = mycam.ScreenToWorldPoint(sp);
            transform.parent.transform.LookAt(v, Vector3.up);

        }
    }
}
