using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeField]
    public Text KillCount;
    [SerializeField]
    public Text Timer;
    [SerializeField]
    public Text Life;


    private float GameDuration;

    public void Start()
    {
        GameDuration = 0f;
    }

    public void Update()
    {
        KillCount.text = GameManager.Instance.KillCount.ToString();
        Life.text = GameManager.Instance.LifePercentage.ToString();

        if (!GameManager.Instance.GamePaused)
        {
            GameDuration += Time.deltaTime;
        }
        Timer.text = GameDuration.ToString();
    }
}
