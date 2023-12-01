using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Win_Screen : MonoBehaviour
{
    public void Menu() {
        SceneManager.LoadScene("Main_Menu");
    }
    
    public void Quit() {
        Debug.Log("Quitting!");
        Application.Quit();
    }
}
