using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FishingSceneManager : MonoBehaviour
{
    public Animator animator;
    public LevelLoader levelLoader;
    bool cast = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public GameObject goToGambleButton;
    public GameObject spaceToFishText;
    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.gamblingMoney >= 500){
            goToGambleButton.SetActive(true);
        }else{
            goToGambleButton.SetActive(false);
        }
        if(Input.GetKeyDown(KeyCode.Space)){
            if(!cast){
                cast = true;
                animator.SetTrigger("CastBobber");
                spaceToFishText.SetActive(false);
                StartCoroutine(levelLoader.LoadLevel("FishingGame", 2f));
            }  
        }
    }
    public void GoToGamble(){
        SceneManager.LoadScene("ChooseGamble");
    }
    public void MainMenu(){
        SceneManager.LoadScene("MainMenu");
    }
}
