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

    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private EnemyCreator _enemyCreator;

    [SerializeField]
    private TutorialManager _tutorialManager;

    // Start is called before the first frame update
    void Start()
    {
        requireOk = true;
        stageIsTimeBased = false;
        infiniteStage = false;
        stageNumber = 0;
        enemiesHordeSize = 5;
        provider = FindObjectOfType<LeapProvider>();

        _uiManager = GameObject.Find("PlayerCanvas")?.GetComponent<UIManager>();
        if (_uiManager == null)
            Debug.LogError("UIManager is null");

        _enemyCreator = GameObject.Find("EnemyCreator")?.GetComponent<EnemyCreator>();
        if (_enemyCreator == null)
            Debug.LogError("EnemyCreator is null");

        _tutorialManager = GameObject.Find("FlatScreenTV")?.GetComponent<TutorialManager>();
        if (_enemyCreator == null)
            Debug.LogError("TutorialManager is null");
    }

    // Update is called once per frame
    void Update()
    {
        if(stageNumber == 0 && ExerciseDetector.availableMagics?.Count == 0) {
            ExerciseDetector.availableMagics.Add(ExerciseType.ROTATION);
        }

        if(canCheckEndStage)
            CheckEndStage();

        if (stageIsTimeBased) {
            time -= Time.deltaTime;
            _uiManager.SurviveFor((int)time);
        }
        if (requireOk) {
            Hand rightHand = null;
            Hand leftHand = null;
            Frame frame = provider.CurrentFrame;

            if (frame.Hands.Capacity > 0) {
                foreach (Hand h in frame.Hands) {
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
                        _tutorialManager.DisableTV();
                        ExerciseDetector.availableMagics.Clear();
                        StartStage(stageNumber);
                    }
                }
            }
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
                        _uiManager.DefeatAllEnemies();
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
                    _enemyCreator.SpawnEnemies(element, spawnTime, numberOfEnemies);
                    lastEnemyTime = spawnTime;
                }
            }

            if(!stageIsTimeBased) Invoke("ToggleCheckEndOfStage", lastEnemyTime+5);
        }
        else {
            infiniteStage = true;
            time = 0;
            _uiManager.Stage(stageNumber + 1);
            StartInfiniteStage();
            Invoke("ToggleCheckEndOfStage", 5);
        }

        stageNumber = nextStage + 1;
    }

    
    void StartInfiniteStage() {
        AddAllAvailableMagics();
        for (int i = 0; i < enemiesHordeSize; i++) {
            _enemyCreator.SpawnEnemies((Element)Random.Range(0, 4), 5*i, 1);
        }
        enemiesHordeSize++;
    }

    void CheckEndStage() {
        if (gameIsOver) return;
        requireOk = Enemy.numberOfEnemies == 0;
        if (!stageIsTimeBased && requireOk) {
            if (infiniteStage) {
                ExerciseDetector.availableMagics.Clear();
                _uiManager.RequireOK();
            }
            else {
                ExerciseDetector.availableMagics.Clear();
                _uiManager.RequireOK();
                _tutorialManager.NextVideo();
                ExerciseDetector.availableMagics.Add((ExerciseType)stageNumber);
            }
            canCheckEndStage = !requireOk;
        }
    }

    void EndTimeBasedStage() {
        if (gameIsOver) return;
        Enemy.DestroyAllEnemies();

        stageIsTimeBased = false;
        requireOk = true;
        _tutorialManager.NextVideo();

        _uiManager.RequireOK();
        ExerciseDetector.availableMagics.Clear();
        ExerciseDetector.availableMagics.Add((ExerciseType)stageNumber);
    }

    void AddAllAvailableMagics() {
        ExerciseDetector.availableMagics.Add(ExerciseType.FINGER_CURL);
        ExerciseDetector.availableMagics.Add(ExerciseType.FIST);
        ExerciseDetector.availableMagics.Add(ExerciseType.ROTATION);
        ExerciseDetector.availableMagics.Add(ExerciseType.WRIST_CURL);
    }

    public void GameOver() {
        Enemy.DestroyAllEnemies();
        requireOk = true;
        gameIsOver = true;
        _uiManager.GameOver();
    }

    void ToggleCheckEndOfStage() {
        canCheckEndStage = true;
    }
}
