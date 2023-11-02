using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause_Menu : MonoBehaviour
{
    // Start is called before the first frame update
    public static bool paused = false;
    public GameObject pausedUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (paused) {
                Resume();
            }
            else {
                Pause();
            }
        }
    }
    public void Resume() {
        pausedUI.SetActive(false);
        Time.timeScale = 1f;
        paused = false;
    }
    void Pause() {
        pausedUI.SetActive(true);
        Time.timeScale = 0f;
        paused = true;
    }
    public void LoadMenu() {
        Time.timeScale = 1f;
        Debug.Log("Loading Menu...");
        SceneManager.LoadScene("Main_Menu");
    }
    public void Quit() {
        Debug.Log("Quitting!");
        Application.Quit();
    }
}
