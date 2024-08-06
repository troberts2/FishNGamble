using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionScreen : MonoBehaviour
{
    public void LoadBlackjack(){
        SceneManager.LoadScene("Blackjack");
    }
    public void LoadSlots(){
        SceneManager.LoadScene("Slots");
    }
    public void BackToFishin(){
        SceneManager.LoadScene("FishingAnimation");
    }
}
