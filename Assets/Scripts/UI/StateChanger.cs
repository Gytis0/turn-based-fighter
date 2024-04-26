using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChanger : MonoBehaviour
{
    public static StateChanger Instance { get; private set; }
    public delegate void StartGame();
    public static event StartGame onStartGame;
    public delegate void EndGame();
    public static event EndGame onContinueGame;

    public void ClickedStartGame()
    {
        onStartGame();
    }

    public void ClickedContinueGame()
    {
        onContinueGame();
    }
}
