using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    public static StartButton Instance { get; private set; }
    public delegate void StartGame();
    public static event StartGame onStartGame;

    public void ClickedStartGame()
    {
        onStartGame();
    }
}
