using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FishingHook : MonoBehaviour
{
    public float hookSpeedX = 3;
    public float hookSpeedY = 2;

    public bool caught = false;
    private bool reeling = false;
    private Rigidbody2D rb;
    public LevelLoader levelLoader;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveHookBeforeCatch();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "fish"){
            CatchFish(other.gameObject);
        }
    }

    private void CatchFish(GameObject gameObject)
    {
        caught = true;
        Debug.Log("caught");
        GetComponent<BoxCollider2D>().enabled = false;
        GameManager.Instance.caughtFishCounter++;
        GameManager.Instance.gamblingMoney += gameObject.GetComponent<BasicFish>().fishData.fishWorth;
        gameObject.transform.position = transform.position;
        gameObject.transform.parent = transform;
        gameObject.GetComponent<BasicFish>().isCaught = true;
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        StartCoroutine(PullOut(gameObject));
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.Space) && !caught && !reeling){
            reeling = true;
            rb.velocity = new Vector2(0, -hookSpeedY);
        }
    }

    void MoveHookBeforeCatch(){
        if(!caught){
            if(!reeling){
                Vector3 x_Input = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
                rb.MovePosition(transform.position + x_Input * Time.deltaTime * hookSpeedX);
            }
            if(reeling)
            {
                if(transform.position.y <= -3.6f)
                {
                    rb.velocity = new Vector2(0, hookSpeedY);
                }

                if(transform.position.y > 3.7f)
                {
                    reeling = false;
                    rb.velocity = Vector2.zero;
                    transform.position = new Vector3(transform.position.x, 3.7f, 0);
                }
            }
        }
    }
    IEnumerator PullOut(GameObject fish){
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(1f);

        while(transform.position.y < 4){
            rb.velocity = new Vector2(0, hookSpeedY * 2);
            fish.GetComponent<Rigidbody2D>().velocity = rb.velocity;
            yield return null;
        }

        //trigger caught and scene change?
        yield return new WaitForSeconds(0);
        fish.SetActive(false);
        StartCoroutine(levelLoader.LoadLevel("FishingAnimation", 0f));
    }
}
