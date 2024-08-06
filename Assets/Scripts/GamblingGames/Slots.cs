using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slots : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float sizeChangeAmount;
    public float extraSecondsToStop;
    public bool stopped = true;
    private SlotsManager slotsManager;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        slotsManager = FindObjectOfType<SlotsManager>();
    }
    public void StartSpin(){
        spriteRenderer.size = new Vector2 (spriteRenderer.size.x, Random.Range(8.2f, 25.8f));
        sizeChangeAmount = Random.Range(0.25f, 1f);
        slotsManager.betAmount = (int)slotsManager.slotsSlider.value;
        stopped = false;
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if(!slotsManager.playSlotsButton.activeSelf){
    //         if(Input.GetKeyDown(KeyCode.Space)){
    //             if(!stopped){
    //                 slotsManager.StopButton();
    //             }else{
    //                 slotsManager.BetButton();
    //             }
    //         }
    //     }
        
    // }
    private void FixedUpdate() {
        if(spriteRenderer != null && !stopped){
            spriteRenderer.size = new Vector2 (spriteRenderer.size.x, spriteRenderer.size.y + sizeChangeAmount);
            if(spriteRenderer.size.y > 25.8f){
                spriteRenderer.size = new Vector2(spriteRenderer.size.x, 8.2f);
            } 
        }

    }
    public IEnumerator StopSlot(float time){
        float elapsedTime = 0;
        float sizeChangeAmountStart = sizeChangeAmount;
        yield return new WaitForSeconds(extraSecondsToStop);

        while (elapsedTime < time)
        {
            // Calculate the new position using Lerp
            sizeChangeAmount = Mathf.Lerp(sizeChangeAmountStart, 0, elapsedTime / time);

            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }
        sizeChangeAmount = 0;
        StartCoroutine(MoveSlotOverTime(spriteRenderer.size.y, CheckSlotLandingSnap(), .5f));
        stopped = true;

    }
    private float CheckSlotLandingSnap(){
        float spriteEndHeight = spriteRenderer.size.y;
        if(spriteEndHeight >= 8.2 && spriteEndHeight <= 10.25){
            //cherry
            return 8.96f;
        }
        else if(spriteEndHeight > 10.25 && spriteEndHeight <= 13f){
            //bell
            return 11.6f;
        }
        else if(spriteEndHeight > 13f && spriteEndHeight <= 15.25f){
            //bar
            return 14.25f;
        }
        else if(spriteEndHeight > 15.25f && spriteEndHeight <= 18f){
            return 16.8f;
        }
        else if(spriteEndHeight > 18f && spriteEndHeight <= 20.6f){
            return 19.25f;
        }
        else if(spriteEndHeight > 20.6f && spriteEndHeight <= 23f){
            return 21.75f;
        }
        else if(spriteEndHeight > 23f && spriteEndHeight <= 25.8f){
            return 24.5f;
        }
        Debug.Log("slot error");
        return 0;
    }
    IEnumerator MoveSlotOverTime(float fromPosition, float toPosition, float time)
    {
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            // Calculate the new position using Lerp
            spriteRenderer.size = new Vector2(spriteRenderer.size.x, Mathf.Lerp(fromPosition, toPosition, elapsedTime / time));

            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the final position is exactly the target position
        spriteRenderer.size = new Vector2(spriteRenderer.size.x, toPosition);
        winMultiplierSlot = CheckWinMultiplier(spriteRenderer.size.y);
    }
    public int winMultiplierSlot;
    public int CheckWinMultiplier(float endNum){
        switch(endNum){
            case 8.96f:
                //cherry
                return 2;
            case 11.6f:
                //bell
                return 1;
            case 14.25f:
                //bar
                return 7;
            case 16.8f:
                //7
                return 6;
            case 19.25f:
                //watermelon
                return 5;
            case 21.75f:
                //lemon
                return 4;
            case 24.5f:
                //grape
                return 3;
            default:
                return 0;
        }
    }

}
