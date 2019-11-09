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

    // Start is called before the first frame update
    void Start()
    {
        _gameOverText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void Stage(int number)
    {
        _auxiliarText.text = $"Ronda {number}";
    }

    public void GameOver()
    {
        _auxiliarText.text = "¡Pulgares arriba para reiniciar!";
        _gameOverText.gameObject.SetActive(true);
    }
}
