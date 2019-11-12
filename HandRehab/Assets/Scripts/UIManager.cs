using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _gameOverText;

    [SerializeField]
    private Text _auxiliarText;

    [SerializeField]
    private Slider _okChargeBar;

    [SerializeField]
    private GameController _gameController;

    // Start is called before the first frame update
    void Start()
    {
        _gameController = GameObject.Find("GameController")?.GetComponent<GameController>();
        if (_gameController == null)
            Debug.LogError("GameController is null");

        _gameOverText.gameObject.SetActive(false);
        _okChargeBar.gameObject.SetActive(false);

        _okChargeBar.maxValue = 1;
        _okChargeBar.value = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_okChargeBar.gameObject.activeSelf)
        {
            _okChargeBar.value += Time.deltaTime;
            if(_okChargeBar.value >= _okChargeBar.maxValue)
            {
                _okChargeBar.gameObject.SetActive(false);
                _gameController.NextStage();
            }
        }
    }

    public void OK()
    {
        if (!_okChargeBar.gameObject.activeSelf)
        {
            _okChargeBar.gameObject.SetActive(true);
            _okChargeBar.value = 0;
        }
    }

    public void CancelOK()
    {
        if (_okChargeBar.gameObject.activeSelf)
        {
            _okChargeBar.gameObject.SetActive(false);
            _okChargeBar.value = 0;
        }
    }

    public void RequireOK()
    {
        _auxiliarText.text = "¡Pulgares arriba cuando estés listo!";
    }

    public void SurviveFor(int seconds)
    {
        _auxiliarText.text = $"Sobrevive por {seconds} segundos";
    }

    public void DefeatAllEnemies()
    {
        _auxiliarText.text = "Derrota a todos los enemigos";
    }

    public void SetStage(int number)
    {
        _auxiliarText.text = $"Ronda {number}";
    }

    public void GameOver()
    {
        _auxiliarText.text = "¡Pulgares arriba para reiniciar!";
        _gameOverText.gameObject.SetActive(true);
    }
}
