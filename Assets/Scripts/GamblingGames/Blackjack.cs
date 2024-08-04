using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Blackjack : MonoBehaviour
{
    public List<GameObject> cardDeck = new List<GameObject>();
    public Transform dealerCardPosition;
    public Transform playerCardPosition;
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
        DrawDealerCards();
        DrawPlayerCards();

    }
    public TextMeshProUGUI betAmountText;
    public void ChangeBetText(){
        if(betAmountText != null){
            betAmountText.text = "$" + (int)betSlider.value;
        }
    }
    public Slider betSlider;
    private int betAmount = 0;
    public void Bet(){
        betSlider.gameObject.SetActive(true);
        betSlider.minValue = GameManager.Instance.gamblingMoney/10;
        betSlider.maxValue = GameManager.Instance.gamblingMoney;
        betSlider.value = betSlider.minValue;
    }
    public void PlaceBet(){
        betSlider.gameObject.SetActive(false);
        betAmount = (int)betSlider.value;
        BlackjackGame();
    }
    public GameObject playGameButton;
    public void BackOffTable(){
        betSlider.gameObject.SetActive(false);
        playGameButton.SetActive(true);
        backToFishinButton.SetActive(true);
    }
    public GameObject backToFishinButton;
    public void PlayGameButton(){
        Bet();
        backToFishinButton.SetActive(false);
        playGameButton.SetActive(false);
    }
    public void BackToFishin(){
        SceneManager.LoadScene("FishingAnimation");
    }
    private bool playersTurn = false;
    private int numOfCardsDrawn = 0;
    public void DrawPlayerCards(){
        playersTurn = true;
        playerTurnUI.enabled = true;
        for(int i = 0; i < 2; i++){
            GameObject drawnCard = Instantiate(DrawCard(), playerCardPosition.position, Quaternion.identity);
            yourScore += CheckCardValue(drawnCard);
            playerCardHand.Add(drawnCard);
            SpaceCardsEvenly(playerCardHand, 2f);
            yourScoreText.text = "Player Score: " + yourScore;
        }
        if(yourScore == 21){
            playersTurn = false;
            playerTurnUI.enabled = false;
            EndGame();
        }

        //brings up in game buttons
    }
    //button to draw card
    private List<GameObject> playerCardHand = new List<GameObject>();
    public void DrawPlayerCard(){
        GameObject drawnCard = Instantiate(DrawCard(), playerCardPosition.position, Quaternion.identity);
        numOfCardsDrawn++;
        yourScore += CheckCardValue(drawnCard);
        playerCardHand.Add(drawnCard);
        SpaceCardsEvenly(playerCardHand, 2f);
        yourScoreText.text = "Your Score: " + yourScore;
        if(playerHasAce && yourScore > 21){
            playerHasAce = false;
            yourScore -= 10;
            yourScoreText.text = "Your Score: " + yourScore;
        }
        if(yourScore == 21){
            playersTurn = false;
            playerTurnUI.enabled = false;
            EndGame();
        }
        if(yourScore > 21){
            playersTurn = false;
            playerTurnUI.enabled = false;
            EndGame();
        }
    }
    //button to end game
    public void Stand(){
        playersTurn = false;
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
        BlackjackGame();
    }
    private void SpaceCardsEvenly(List<GameObject> cardsToSpace, float spacing){
            // Calculate the total width of all objects combined with spacing
        float totalWidth = (cardsToSpace.Count - 1) * spacing;

        // Loop through each GameObject and position it
        for (int i = 0; i < cardsToSpace.Count; i++)
        {
            // Calculate the position of the current GameObject
            float xPos = -totalWidth / 2.0f + i * spacing;
            Vector2 newPosition = new Vector3(xPos, cardsToSpace[i].transform.position.y);

            // Set the new position of the GameObject
            cardsToSpace[i].transform.position = newPosition;
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
    public void DrawDealerCards(){
        isDealersTurn = true;
        for(int i = 0; i < 2; i++){
            GameObject drawnCard = Instantiate(DrawCard(), dealerCardPosition.position, Quaternion.identity);
            dealerScore += CheckCardValue(drawnCard);
            dealerCardHand.Add(drawnCard);
            SpaceCardsEvenly(dealerCardHand, 2f);
            dealerScoreText.text = "Dealer Score: " + dealerScore;
            backOfCard.SetActive(true);
            backOfCard.GetComponent<SpriteRenderer>().enabled = true;
        }
        isDealersTurn = false;
    }
    private GameObject DrawCard(){
        GameObject cardDrawn = cardDeck[Random.Range(0, cardDeck.Count)];
        while(gameCards.Contains(cardDrawn)){
            cardDrawn = cardDeck[Random.Range(0, cardDeck.Count)];
        }
        gameCards.Add(cardDrawn);
        return cardDrawn;
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
