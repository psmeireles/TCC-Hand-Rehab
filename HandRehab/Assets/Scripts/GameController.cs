using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public List<TextAsset> stages;
   
    float time;
    int stageNumber;
    bool requireOk;
    bool stageIsTimeBased;
    bool infiniteStage;
    bool canCheckEndStage;
    bool gameIsOver;
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
        stageNumber = 0;
        enemiesHordeSize = 5;
        provider = FindObjectOfType<LeapProvider>();
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
        if(stageNumber == 0 && ExerciseDetector.availableMagics != null && ExerciseDetector.availableMagics.Count == 0) {
            ExerciseDetector.availableMagics.Add(ExerciseType.ROTATION);
        }

        if(canCheckEndStage)
            CheckEndStage();

        if (stageIsTimeBased) {
            time -= Time.deltaTime;
            elapsedTime.text = $"Sobrevive por {(int)time} segundos";
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
                    if (gameIsOver)
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    }
                    else
                    {
                        requireOk = false;
                        tv.SetActive(false);
                        ExerciseDetector.availableMagics.Clear();
                        StartStage(stageNumber);
                    }
                }
            }
        }

        if (player.GetComponent<Character>().hp == 0) {
            GameOver();
        }
    }

    void StartStage(int nextStage) {
        float lastEnemyTime = 0.1f;
        if (nextStage < stages.Count) {
            string[] lines = stages[nextStage].text.Split('\n');
            foreach (string line in lines) {
                string[] items = line.Split(' ');
                if (items[0] == "TIME") {
                    float stageTime = float.Parse(items[1]);
                    if (stageTime == -1) {
                        stageIsTimeBased = false;
                        elapsedTime.text = "Derrota a todos los enemigos";
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
                    lastEnemyTime = spawnTime;
                }
            }

            if(!stageIsTimeBased) Invoke("ToggleCheckEndOfStage", lastEnemyTime+5);
        }
        else {
            infiniteStage = true;
            time = 0;
            elapsedTime.gameObject.SetActive(true);
            elapsedTime.text = $"Ronda {stageNumber + 1}";
            StartInfiniteStage();
            Invoke("ToggleCheckEndOfStage", 5);
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
        if (gameIsOver) return;
        if (!stageIsTimeBased) {
            if (infiniteStage && GameObject.FindGameObjectsWithTag("Enemy").Length == 0) {
                requireOk = true;
                ExerciseDetector.availableMagics.Clear();
                elapsedTime.text = "¡Pulgares arriba cuando estés listo!";
            }
            else {
                requireOk = GameObject.FindGameObjectsWithTag("Enemy").Length == 0;
                if (requireOk) {
                    ExerciseDetector.availableMagics.Clear();
                    elapsedTime.text = "¡Pulgares arriba cuando estés listo!";
                    NextVideo();
                    ExerciseDetector.availableMagics.Add((ExerciseType)stageNumber);
                }
            }
            canCheckEndStage = !requireOk;
        }
    }

    void EndTimeBasedStage() {
        if (gameIsOver) return;
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies) {
            DestroyImmediate(enemy);
        }

        stageIsTimeBased = false;
        requireOk = true;
        NextVideo();
        
        elapsedTime.text = "¡Pulgares arriba cuando estés listo!";
        ExerciseDetector.availableMagics.Clear();
        ExerciseDetector.availableMagics.Add((ExerciseType)stageNumber);
    }

    void AddAllAvailableMagics() {
        ExerciseDetector.availableMagics.Add(ExerciseType.FINGER_CURL);
        ExerciseDetector.availableMagics.Add(ExerciseType.FIST);
        ExerciseDetector.availableMagics.Add(ExerciseType.ROTATION);
        ExerciseDetector.availableMagics.Add(ExerciseType.WRIST_CURL);
    }

    void NextVideo()
    {
        if (stageNumber < tutorialVideos.Count)
        {
            tv.SetActive(true);
            currentVideo = tutorialVideos[stageNumber];
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

        requireOk = true;
        gameIsOver = true;
        elapsedTime.text = "¡Pulgares arriba para reiniciar!";
    }

    void ToggleCheckEndOfStage() {
        canCheckEndStage = true;
    }
}
