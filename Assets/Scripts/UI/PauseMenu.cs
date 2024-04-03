using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void OpenMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
