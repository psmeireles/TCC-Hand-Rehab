using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    public static Difficulty gameDifficulty;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(gameDifficulty);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
