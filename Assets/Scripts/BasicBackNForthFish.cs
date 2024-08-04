using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFish : MonoBehaviour
{
    public FishData fishData;
    public float fishSpeed;
    private bool fishClamp;

    private int fishWallBounceCounter = 0;

    private Rigidbody2D rb;
    private bool isRight = true;
    public bool isCaught = false;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isRight = Random.value < 0.5f;
        fishSpeed = fishData.fishSpeed;
        fishClamp = fishData.fishClamp;
        if(!isRight) spriteRenderer.flipX = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x > 20 || transform.position.x < -20){
            Destroy(gameObject);
        }

        if(fishClamp){
            if(isRight){
                if(fishSpeed < 0) {
                    spriteRenderer.flipX = false;
                    fishSpeed *= -1;
                }
            }
            else{
                if(fishSpeed > 0){
                    spriteRenderer.flipX = true;
                    fishSpeed *= -1;
                } 
            }
            if(transform.position.x <= -7.5 && fishSpeed < 0){
                fishWallBounceCounter++;
                isRight = true;
            }
            if(transform.position.x >= 7.5 && fishSpeed > 0){
                fishWallBounceCounter++;
                isRight = false;
            }
            if(fishWallBounceCounter >= fishData.fishWallBounces){
                fishClamp = false;
            }
        }  

    }
    private void FixedUpdate() {
        if(!isCaught){
            rb.velocity = new Vector2(fishSpeed, 0);
        }
    }
}
