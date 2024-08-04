using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator animator;
    public string levelToLoad;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.Mouse1)){
        //     StartCoroutine(LoadLevel());
        // }
    }

    public IEnumerator LoadLevel(string levelToLoad, float delay){
        yield return new WaitForSeconds(delay);
        
        animator.SetTrigger("Start");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(levelToLoad);
    }
}
