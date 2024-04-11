using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    int[] points;

    [SerializeField] MainMenu menu;

    PlayerProperties playerProperties;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        points = new int[4];
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerProperties = player.GetComponent<PlayerProperties>();
        }
    }

    

    private void OnLevelWasLoaded(int level)
    {
        Debug.Log("OnLevelLoad");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerProperties = player.GetComponent<PlayerProperties>();
        }

        if (level != 0 && playerProperties != null)
        {
            playerProperties.SetStats(points);
        }

    }

    private void OnEnable()
    {
        Interactable.onTravel += LoadFightScene;
    }

    private void OnDisable()
    {
        Interactable.onTravel -= LoadFightScene;
    }

    public void LoadEquipmentScene()
    {
        menu.GetPoints().CopyTo(points, 0);
        
        SceneManager.LoadScene(1);
    }

    public void LoadFightScene()
    {
        SceneManager.LoadScene(2);
    }

}
