using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Main_Menu : MonoBehaviour
{
    // Start is called before the first frame update
    public void StartGame() {
        SceneManager.LoadScene("Level_1");
    }
    public void Back() {
        SceneManager.LoadScene("Main_Menu");
    }
    public void Help() {
        SceneManager.LoadScene("Help_Menu");
    }
    public void Quit() {
        Debug.Log("Quitting!");
        Application.Quit();
    }
}
