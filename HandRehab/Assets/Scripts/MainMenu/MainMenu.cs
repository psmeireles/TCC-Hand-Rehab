using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private GameObject _mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        _mainCamera.transform.Rotate(Vector3.up, 5*Time.deltaTime, Space.World);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }
}
