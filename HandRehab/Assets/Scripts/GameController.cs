﻿using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameController : MonoBehaviour
{
    public GameObject enemy;
    public GameObject player;
    public Terrain terrain;
    public Text gameOver;
    public Text elapsedTime;
    public List<VideoClip> tutorialVideos;
    public GameObject tv;
   
    float time;
    int stageNumber;
    bool requireOk;
    bool stageIsTimeBased;
    bool infiniteStage;
    int enemiesHordeSize;
    LeapProvider provider;
    VideoClip currentVideo;
    VideoPlayer videoPlayer;

    // Start is called before the first frame update
    void Start()
    {
        requireOk = true;
        stageIsTimeBased = false;
        infiniteStage = false;
        stageNumber = 1;
        enemiesHordeSize = 5;
        provider = FindObjectOfType<LeapProvider>();
        InvokeRepeating("CheckEndStage", 10, 10);
        if(tutorialVideos.Count > 0)
        {
            currentVideo = tutorialVideos[0];
        }
        videoPlayer = tv.GetComponentInChildren<VideoPlayer>();
        videoPlayer.clip = currentVideo;
    }

    // Update is called once per frame
    void Update()
    {
        if (stageIsTimeBased) {
            time -= Time.deltaTime;
            elapsedTime.text = $"Survive for {(int)time} seconds";
        }
        if (requireOk) {
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

            if (rightHand != null) {
                int extendedFingers = 0;
                for (int i = 0; i < rightHand.Fingers.Count; i++) {
                    Finger f = rightHand.Fingers[i];
                    if (f.IsExtended)
                        extendedFingers++;
                }
                if (extendedFingers == 1 && rightHand.Fingers[0].IsExtended) {
                    requireOk = false;
                    tv.SetActive(false);
                    ExerciseDetector.availableMagics.Clear();
                    StartStage(stageNumber);
                }
            }
        }

        if (player.GetComponent<Character>().hp == 0) {
            GameOver();
        }
    }

    void StartStage(int nextStage) {

        if (nextStage < 5) {
            StreamReader reader = File.OpenText($"Assets/Stages/stage{nextStage}.txt");
            string line;
            while ((line = reader.ReadLine()) != null) {
                string[] items = line.Split(' ');
                if (items[0] == "TIME") {
                    float stageTime = float.Parse(items[1]);
                    if (stageTime == -1) {
                        stageIsTimeBased = false;
                        elapsedTime.text = "Defeat all enemies";
                    }
                    else {
                        stageIsTimeBased = true;
                        time = stageTime;
                        Invoke("EndTimeBasedStage", stageTime);
                    }
                }
                else if (items[0] == "MAGIC") {
                    for (int i = 1; i < items.Length; i++) {
                        ExerciseDetector.availableMagics.Add((ExerciseType) System.Enum.Parse(typeof(ExerciseType), items[i]));
                    }
                }
                else {
                    Element element = (Element)System.Enum.Parse(typeof(Element), items[0]);
                    int numberOfEnemies = int.Parse(items[1]);
                    float spawnTime = float.Parse(items[2]);
                    for (int i = 0; i < numberOfEnemies; i++) {
                        StartCoroutine(SpawnEnemy(element, spawnTime));
                    }
                }
            }

        }
        else {
            infiniteStage = true;
            time = 0;
            elapsedTime.gameObject.SetActive(true);
            elapsedTime.text = $"Round {stageNumber}";
            StartInfiniteStage();
        }

        stageNumber = nextStage + 1;
    }

    IEnumerator SpawnEnemy(Element element, float spawnTime) {
        yield return new WaitForSeconds(spawnTime);

        GameObject copy = GameObject.Instantiate(enemy);
        var enemyInstance = copy.GetComponent<Enemy>();
        enemyInstance.type = new CharType(element);
        copy.GetComponent<Renderer>().material.color = enemyInstance.type.color;
        copy.transform.position = player.transform.position + Random.onUnitSphere * 20;
        Vector3 enemyPosition = copy.transform.position;
        enemyPosition.y = terrain.SampleHeight(enemyPosition) + terrain.transform.position.y + 1;
        copy.transform.position = enemyPosition;
    }
    
    void StartInfiniteStage() {
        AddAllAvailableMagics();
        for (int i = 0; i < enemiesHordeSize; i++) {
            StartCoroutine(SpawnEnemy((Element)Random.Range(0, 4), 0));
        }
        enemiesHordeSize++;
    }

    void CheckEndStage() {
        if (!stageIsTimeBased) {
            if (infiniteStage && GameObject.FindGameObjectsWithTag("Enemy").Length == 0) {
                requireOk = true;
                ExerciseDetector.availableMagics.Clear();
                elapsedTime.text = "Thumbs up when you're ready!";
            }
            else {
                requireOk = GameObject.FindGameObjectsWithTag("Enemy").Length == 0;
                if (requireOk) {
                    ExerciseDetector.availableMagics.Clear();
                    elapsedTime.text = "Thumbs up when you're ready!";
                    NextVideo();
                    ExerciseDetector.availableMagics.Add((ExerciseType)stageNumber - 1);
                }
            }
        }
    }

    void EndTimeBasedStage() {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies) {
            DestroyImmediate(enemy);
        }

        stageIsTimeBased = false;
        requireOk = true;
        NextVideo();
        
        elapsedTime.text = "Thumbs up when you're ready!";
        ExerciseDetector.availableMagics.Clear();
    }

    void AddAllAvailableMagics() {
        ExerciseDetector.availableMagics.Add(ExerciseType.FINGER_CURL);
        ExerciseDetector.availableMagics.Add(ExerciseType.FIST);
        ExerciseDetector.availableMagics.Add(ExerciseType.ROTATION);
        ExerciseDetector.availableMagics.Add(ExerciseType.WRIST_CURL);
    }

    void NextVideo()
    {
        if (stageNumber - 1 < tutorialVideos.Count)
        {
            tv.SetActive(true);
            currentVideo = tutorialVideos[stageNumber - 1];
            videoPlayer.clip = currentVideo;
        }
    }

    void GameOver() {
        gameOver.gameObject.SetActive(true);
        elapsedTime.gameObject.SetActive(true);

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies) {
            DestroyImmediate(enemy);
        }

        requireOk = false;
    }
}
