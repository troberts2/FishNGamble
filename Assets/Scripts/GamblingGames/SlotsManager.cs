using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SlotsManager : MonoBehaviour
{
    public Slots slotObject1;
    public Slots slotObject2;
    public Slots slotObject3;
    private int slot1;
    private int slot2;
    private int slot3;
    private bool gameInProgress = false;
    private void Start() {
        slotsSlider.onValueChanged.AddListener(delegate {ChangeBetText();});
    }
    public bool gameStopped = false;
    void Update()
    {
        if(slotObject1 != null && slotObject1.stopped && !gameStopped && betAmount > 0 && gameInProgress) {
            StartCoroutine(EndGame());
            gameStopped = true;
        }
        if(!playSlotsButton.activeSelf){
            if(Input.GetKeyDown(KeyCode.Space)){
                if(!slotObject1.stopped){
                    if(gameInProgress) StopButton();
                }else{
                    if(!gameInProgress) BetButton();
                }
            }
        }
        
    }
    public bool AreSlotsSame(){
        slot1 = slotObject1.winMultiplierSlot;
        slot2 = slotObject2.winMultiplierSlot;
        slot3 = slotObject3.winMultiplierSlot;
        if(slot1 != slot2 || slot1 != slot3 || slot2 != slot3){
            return false;
        }else{
            winMultiplier = slot1;
            return true;
        }
    }
    public TextMeshProUGUI slotWinText;
    public int betAmount;
    public int winMultiplier = 0;

    public IEnumerator EndGame(){
        yield return new WaitForSeconds(3f);
        slotWinText.gameObject.SetActive(true);
        if(AreSlotsSame()){
            if(winMultiplier > 0 && winMultiplier != 7){
                GameManager.Instance.gamblingMoney += betAmount* winMultiplier;
                slotWinText.text = winMultiplier + "X Multiplier"; 
            }else if( winMultiplier == 7){
                GameManager.Instance.gamblingMoney += betAmount * 10;
                slotWinText.text = "JACKPOT";
            }
        }else{
            slotWinText.text = "Loss";
            GameManager.Instance.gamblingMoney -= betAmount;
        }
        winMultiplier =0;
        GameManager.Instance.UpdateGamblingMoney();
        //flash the win text
        for (int i = 0; i < 3; i++){
            yield return new WaitForSeconds(.3f);
            slotWinText.gameObject.SetActive(false);
            yield return new WaitForSeconds(.1f);
            slotWinText.gameObject.SetActive(true);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        slotWinText.gameObject.SetActive(false);
        gameInProgress = false;
        PlaySlots();
    }
    public Slider slotsSlider;
    public GameObject betButton;
    public GameObject backButton;
    public GameObject backToFishinButton;
    public GameObject playSlotsButton;
    public GameObject stopButton;
    public void PlaySlots(){
        if(GameManager.Instance.gamblingMoney <= 0){
            return;
        }
        playSlotsButton.SetActive(false);
        backToFishinButton.SetActive(false);
        betButton.SetActive(true);
        backButton.SetActive(true);
        slotsSlider.gameObject.SetActive(true);
        slotsSlider.minValue = GameManager.Instance.gamblingMoney/10;
        slotsSlider.maxValue = GameManager.Instance.gamblingMoney;
        SetSnapPoints(slotsSlider.minValue, slotsSlider.maxValue, 5);
    }
    public void BetButton(){
        gameInProgress = true;
        backButton.SetActive(false);
        gameStopped = false;
        betAmount = (int)slotsSlider.value;
        slotObject1.StartSpin();
        slotObject2.StartSpin();
        slotObject3.StartSpin();
        slotsSlider.gameObject.SetActive(false);
        betButton.SetActive(false);
        stopButton.SetActive(true);

    }
    public void StopButton(){
        StartCoroutine(slotObject1.StopSlot(1f));
        StartCoroutine(slotObject2.StopSlot(1f));
        StartCoroutine(slotObject3.StopSlot(1f));
        stopButton.SetActive(false);
    }
    public void BackButton(){
        playSlotsButton.SetActive(true);
        backToFishinButton.SetActive(true);
        betButton.SetActive(false);
        slotsSlider.gameObject.SetActive(false);
        backButton.SetActive(false);
    }
    public void BackToFishin(){
        SceneManager.LoadScene("FishingAnimation");
    }
    public TextMeshProUGUI betAmountText;
    private float[] snapPoints = new float[1];
    public void ChangeBetText(){
        if(betAmountText != null){
            betAmountText.text = "$" + (int)slotsSlider.value;
        }
                // Find the closest snap point to the current value
        float closestSnapPoint = snapPoints[0];
        float smallestDifference = Mathf.Abs(slotsSlider.value - snapPoints[0]);

        for (int i = 1; i < snapPoints.Length; i++)
        {
            float difference = Mathf.Abs(slotsSlider.value - snapPoints[i]);
            if (difference < smallestDifference)
            {
                closestSnapPoint = snapPoints[i];
                smallestDifference = difference;
            }
        }

        // Set the slider value to the closest snap point
        slotsSlider.value = closestSnapPoint;
    }
    public void SetSnapPoints(float minValue, float maxValue, int numberOfPoints)
    {
        // Ensure there's at least 2 snap points (min and max)
        numberOfPoints = Mathf.Max(2, numberOfPoints);

        // Calculate the step between each snap point
        float step = maxValue / (numberOfPoints - 1);

        // Create the snapPoints array with the appropriate size
        snapPoints = new float[numberOfPoints];

        // Fill the snapPoints array with calculated values
        for (int i = 0; i < numberOfPoints; i++)
        {
            if(i == 0){
                snapPoints[i] = minValue;
            }else{
                snapPoints[i] = step * i;
            }
        }
    }
}
