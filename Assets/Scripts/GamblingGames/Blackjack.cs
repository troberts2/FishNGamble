using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class Blackjack : MonoBehaviour
{
    public List<GameObject> cardDeck = new List<GameObject>();
    public Transform dealerCardPosition;
    public Transform playerCardPosition;
    public Transform cardStackPostion;
    public GameObject backOfCard;
    public Canvas playerTurnUI;
    private List<GameObject> gameCards = new List<GameObject>();



    private void Start() {
        betSlider.onValueChanged.AddListener(delegate {ChangeBetText();});
    }
    private int dealerScore = 0;
    private int yourScore = 0;
    public TextMeshProUGUI dealerScoreText;
    public TextMeshProUGUI yourScoreText;
    public void BlackjackGame(){
        //Bet()
        StartCoroutine(DrawDealerCards());

    }
    public TextMeshProUGUI betAmountText;
    private float[] snapPoints = new float[1];
    public void ChangeBetText(){
        if(betAmountText != null){
            betAmountText.text = "$" + (int)betSlider.value;
        }
                // Find the closest snap point to the current value
        float closestSnapPoint = snapPoints[0];
        float smallestDifference = Mathf.Abs(betSlider.value - snapPoints[0]);

        for (int i = 1; i < snapPoints.Length; i++)
        {
            float difference = Mathf.Abs(betSlider.value - snapPoints[i]);
            if (difference < smallestDifference)
            {
                closestSnapPoint = snapPoints[i];
                smallestDifference = difference;
            }
        }

        // Set the slider value to the closest snap point
        betSlider.value = closestSnapPoint;
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
    public Slider betSlider;
    private int betAmount = 0;
    public GameObject placeBetButton;
    public GameObject backOutOfBetButton;
    public void Bet(){
        betSlider.gameObject.SetActive(true);
        placeBetButton.SetActive(true);
        backOutOfBetButton.SetActive(true);
        betSlider.minValue = GameManager.Instance.gamblingMoney/10;
        betSlider.maxValue = GameManager.Instance.gamblingMoney;
        SetSnapPoints(betSlider.minValue, betSlider.maxValue, 5);
        betSlider.value = betSlider.minValue;
    }
    public void PlaceBet(){
        betSlider.gameObject.SetActive(false);
        placeBetButton.SetActive(false);
        backOutOfBetButton.SetActive(false);
        betAmount = (int)betSlider.value;
        BlackjackGame();
    }
    public GameObject playGameButton;
    public void BackOffTable(){
        betSlider.gameObject.SetActive(false);
        placeBetButton.SetActive(false);
        backOutOfBetButton.SetActive(false);
        playGameButton.SetActive(true);
        backToFishinButton.SetActive(true);
    }
    public GameObject backToFishinButton;
    public void PlayGameButton(){
        if(GameManager.Instance.gamblingMoney <= 0){
            return;
        }
        Bet();
        backToFishinButton.SetActive(false);
        playGameButton.SetActive(false);
    }
    public void BackToFishin(){
        StartCoroutine(GameManager.Instance.levelLoader.LoadLevel("FishingAnimation", 0f));
    }
    private int numOfCardsDrawn = 0;
    IEnumerator DrawPlayerCards(){
        playerTurnUI.enabled = true;
        for(int i = 0; i < 2; i++){
            GameObject drawnCard = Instantiate(DrawCard(), cardStackPostion.position, Quaternion.identity);
            StartCoroutine(MoveCardOverTime(drawnCard.transform, cardStackPostion.position, playerCardPosition.position, .1f));
            yourScore += CheckCardValue(drawnCard);
            playerCardHand.Add(drawnCard);
            StartCoroutine(SpaceCardsEvenly(playerCardHand, 2f));
            yourScoreText.text = "Player Score: " + yourScore;
            yield return new WaitForSeconds(.1f);
        }
        if(yourScore == 21){
            playerTurnUI.enabled = false;
            EndGame();
        }

        //brings up in game buttons
    }
    //button to draw card
    private List<GameObject> playerCardHand = new List<GameObject>();
    public void DrawPlayerCard(){
        GameObject drawnCard = Instantiate(DrawCard(), cardStackPostion.position, Quaternion.identity);
        StartCoroutine(MoveCardOverTime(drawnCard.transform, cardStackPostion.position, playerCardPosition.position, .1f));
        numOfCardsDrawn++;
        yourScore += CheckCardValue(drawnCard);
        playerCardHand.Add(drawnCard);
        StartCoroutine(SpaceCardsEvenly(playerCardHand, 2f));
        yourScoreText.text = "Your Score: " + yourScore;
        if(playerHasAce && yourScore > 21){
            playerHasAce = false;
            yourScore -= 10;
            yourScoreText.text = "Your Score: " + yourScore;
        }
        if(yourScore == 21){
            playerTurnUI.enabled = false;
            EndGame();
        }
        if(yourScore > 21){
            playerTurnUI.enabled = false;
            EndGame();
        }
    }
    //button to end game
    public void Stand(){
        playerTurnUI.enabled = false;
        EndGame();
    }
    private void EndGame(){
        backOfCard.GetComponent<SpriteRenderer>().enabled = false;
        if(dealerScore > 21){
            StartCoroutine(DealerBust());
        }
        else if(yourScore > 21){
            StartCoroutine(PlayerBust());
        }
        else if(yourScore > dealerScore){
            if(yourScore == 21){
                StartCoroutine(PlayerBlackjack());
            }else{
                StartCoroutine(PlayerWin());
            }   
        }else if(dealerScore > yourScore){
            if(dealerScore == 21){
                StartCoroutine(DealerBlackjack());
            }else{
                StartCoroutine(DealerWin());
            } 
        }else{
            StartCoroutine(Tie());
        }
        
    }
    private void ResetGame(){
        yourScore = 0;
        dealerScore = 0;
        betAmount = 0;
        dealerScoreText.text = "Dealer Score: 0";
        yourScoreText.text = "Your Score: 0";
        foreach(GameObject card in dealerCardHand){
            Destroy(card);
        } 
        dealerCardHand.Clear();
        foreach(GameObject card in playerCardHand){
            Destroy(card);
        } 
        playerCardHand.Clear();
        gameCards.Clear();
        playerHasAce = false;
        GameManager.Instance.UpdateGamblingMoney();
        Bet();
    }
    private void ResetGameTie(){
        yourScore = 0;
        dealerScore = 0;
        dealerScoreText.text = "Dealer Score: 0";
        yourScoreText.text = "Your Score: 0";
        foreach(GameObject card in dealerCardHand){
            Destroy(card);
        } 
        dealerCardHand.Clear();
        foreach(GameObject card in playerCardHand){
            Destroy(card);
        } 
        playerCardHand.Clear();
        gameCards.Clear();
        playerHasAce = false;
        BlackjackGame();
    }
    IEnumerator SpaceCardsEvenly(List<GameObject> cardsToSpace, float spacing){
        yield return new WaitForSeconds(.1f);
            // Calculate the total width of all objects combined with spacing
        float totalWidth = (cardsToSpace.Count - 1) * spacing;

        // Loop through each GameObject and position it
        for (int i = 0; i < cardsToSpace.Count; i++)
        {
            // Calculate the position of the current GameObject
            float xPos = -totalWidth / 2.0f + i * spacing;
            Vector2 newPosition = new Vector3(xPos, cardsToSpace[i].transform.position.y);

            // Set the new position of the GameObject
            StartCoroutine(MoveCardOverTime(cardsToSpace[i].transform, cardsToSpace[i].transform.position, newPosition, 0.05f));
            cardsToSpace[i].transform.position = newPosition;
            yield return null;
        }
    }
    private float endGameTextWaitTime = 2f;
    public TextMeshProUGUI playerBustText;
    private IEnumerator PlayerBust(){
        playerBustText.enabled = true;
        GameManager.Instance.gamblingMoney -= betAmount;
        yield return new WaitForSeconds(endGameTextWaitTime);
        playerBustText.enabled = false;
        Invoke(nameof(ResetGame), 3f);
    }
    public TextMeshProUGUI dealerBustText;
    private IEnumerator DealerBust(){
        dealerBustText.enabled = true;
        GameManager.Instance.gamblingMoney += betAmount;
        yield return new WaitForSeconds(endGameTextWaitTime);
        dealerBustText.enabled = false;
        Invoke(nameof(ResetGame), 3f);
    }
    public TextMeshProUGUI playerWinText;
    private IEnumerator PlayerWin(){
        playerWinText.enabled = true;
        GameManager.Instance.gamblingMoney += betAmount;
        yield return new WaitForSeconds(endGameTextWaitTime);
        playerWinText.enabled = false;
        Invoke(nameof(ResetGame), 3f);
    }
    public TextMeshProUGUI dealerWinText;
    private IEnumerator DealerWin(){
        dealerWinText.enabled = true;
        GameManager.Instance.gamblingMoney -= betAmount;
        yield return new WaitForSeconds(endGameTextWaitTime);
        dealerWinText.enabled = false;
        Invoke(nameof(ResetGame), 3f);
    }
    public TextMeshProUGUI playerBlackjackText;
    private IEnumerator PlayerBlackjack(){
        playerBlackjackText.enabled = true;
        GameManager.Instance.gamblingMoney += betAmount;
        yield return new WaitForSeconds(endGameTextWaitTime);
        playerBlackjackText.enabled = false;
        Invoke(nameof(ResetGame), 3f);
    }
    public TextMeshProUGUI dealerBlackjackText;
    private IEnumerator DealerBlackjack(){
        dealerBlackjackText.enabled = true;
        GameManager.Instance.gamblingMoney -= betAmount;
        yield return new WaitForSeconds(endGameTextWaitTime);
        dealerBlackjackText.enabled = false;
        Invoke(nameof(ResetGame), 3f);
    }
    public TextMeshProUGUI tieWinText;
    private IEnumerator Tie(){
        tieWinText.enabled = true;
        yield return new WaitForSeconds(endGameTextWaitTime);
        tieWinText.enabled = false;
        Invoke(nameof(ResetGameTie), 3f);
        
    }
    private bool isDealersTurn = false;
    private List<GameObject> dealerCardHand = new List<GameObject>();
    IEnumerator DrawDealerCards(){
        isDealersTurn = true;
        for(int i = 0; i < 2; i++){
            GameObject drawnCard = Instantiate(DrawCard(), dealerCardPosition.position, Quaternion.identity);
            StartCoroutine(MoveCardOverTime(drawnCard.transform, cardStackPostion.position, dealerCardPosition.position, .1f));
            dealerScore += CheckCardValue(drawnCard);
            dealerCardHand.Add(drawnCard);
            StartCoroutine(SpaceCardsEvenly(dealerCardHand, 2f));
            dealerScoreText.text = "Dealer Score: " + dealerScore;
            backOfCard.SetActive(true);
            backOfCard.GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(.1f);
        }
        isDealersTurn = false;
        yield return new WaitForSeconds(.1f);
        StartCoroutine(DrawPlayerCards());
    }
    private GameObject DrawCard(){
        GameObject cardDrawn = cardDeck[Random.Range(0, cardDeck.Count)];
        while(gameCards.Contains(cardDrawn)){
            cardDrawn = cardDeck[Random.Range(0, cardDeck.Count)];
        }
        gameCards.Add(cardDrawn);
        return cardDrawn;
    }
    IEnumerator MoveCardOverTime(Transform cardTransform, Vector3 fromPosition, Vector3 toPosition, float time)
    {
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            // Calculate the new position using Lerp
            cardTransform.position = Vector3.Lerp(fromPosition, toPosition, elapsedTime / time);

            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the final position is exactly the target position
        cardTransform.position = toPosition;
    }
    private bool playerHasAce = false;

    private int CheckCardValue(GameObject drawnCard){
        switch(drawnCard.GetComponent<SpriteRenderer>().sprite.name){
            case "2C": case "2D": case "2S": case "2H":
                return 2;
            case "3C": case "3D": case "3S": case "3H":
                return 3;
            case "4C": case "4D": case "4S": case "4H":
                return 4;
            case "5C": case "5D": case "5S": case "5H":
                return 5;
            case "6C": case "6D": case "6S": case "6H":
                return 6;
            case "7C": case "7D": case "7S": case "7H":
                return 7;
            case "8C": case "8D": case "8S": case "8H":
                return 8;
            case "9C": case "9D": case "9S": case "9H":
                return 9;
            case "10S": case "10C": case "10D": case "10H": case "JackC": case "JackD": case "JackS": case "JackH": case "QueenC": case "QueenD": case "QueenS": case "QueenH": case "KingC": case "KingD": case "KingS": case "KingH":
                return 10;
            case "AceC": case "AceD": case "AceS": case "AceH":
                if(isDealersTurn){
                    return 11;
                }else{
                    if(yourScore < 11){
                        playerHasAce = true;
                        return 11;
                    }else{
                        return 1;
                    }
                }
            default:
                return 0;
        }
    }
}
