using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void PlayGame(){
        StartCoroutine(GameManager.Instance.levelLoader.LoadLevel("FishingAnimation", 0f));
    }

    public void ExitGame(){
        Application.Quit();
    }
}
