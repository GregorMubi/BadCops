using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverDialogController : MonoBehaviour
{
    [SerializeField]
    public Text KillCountText;
    [SerializeField]
    public Text TimeText;

    public void Restart() { }
    public void Exit() { }
    public void Init(int killCount, float timeInSeconds) {
        KillCountText.text = killCount.ToString();
        TimeText.text = timeInSeconds.ToString();
    }
}
