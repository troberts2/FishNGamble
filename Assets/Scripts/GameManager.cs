using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public LevelLoader levelLoader;
    public List<GameObject> typesOfFish;
    public float gamblingMoney = 0f;
    public TextMeshProUGUI gamblingMoneyText;
    public int caughtFishCounter = 0;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        // start of new code
        if (Instance == null)
        {
            Instance = this;
        }
        else{
            Destroy(gameObject);
        }
        
    }
    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public void UpdateGamblingMoney(){
        gamblingMoneyText.enabled = true;
        gamblingMoneyText.text = "$" + gamblingMoney;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        if(scene.name == "FishingAnimation" || scene.name == "FishingGame" || scene.name == "Blackjack" || scene.name == "Slots"){
            UpdateGamblingMoney();
        }
        else{
            gamblingMoneyText.enabled = false;
        }
        levelLoader = FindObjectOfType<LevelLoader>();
    }
}
