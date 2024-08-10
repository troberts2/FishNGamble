using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
        theDeal = StartCoroutine(TheDeal());

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
        playerTurnUI.enabled = false;
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
    private Coroutine theDeal;
    public GameObject splitPairUI;
    private List<GameObject> yourCardHandA = new List<GameObject>();
    private List<GameObject> yourCardHandB = new List<GameObject>();
    public Transform yourHandPositionA;
    public Transform yourHandPositionB;
    private bool splitPair = false;
    private bool doubleDown = false;
    public GameObject doubleDownUI;
    public void DoubleDownYes(){
        doubleDown = true;
        betAmount *=2;
        doubleDownUI.SetActive(false);
        DrawPlayerCard(playerCardHand, playerCardPosition);
    }
    public void DoubleDownNo(){
        doubleDownUI.SetActive(false);
    }

    public void SplitPairYes(){
        splitPair = true;
        betAmount *= 2;
        numOfCardsDrawn = 1;
        splitPairUI.SetActive(false);
        StartCoroutine(FlashFadeHand1Panel());
    }
    public void SplitPairNo(){
        splitPairUI.SetActive(false);
    }
    public GameObject insuranceUI;
    bool insurance = false;
    public void InsuranceYes(){
        insurance = true;
        insuranceUI.SetActive(false);
        if(dealerScore == 21 && yourScore == 21){
            gameOver = true;
            StartCoroutine(Tie());
        }else if(dealerScore == 21){
            GameManager.Instance.gamblingMoney += betAmount;
            StartCoroutine(DealerBlackjack());
        }else{
            GameManager.Instance.gamblingMoney -= betAmount;
        }
    }
    public void InsuranceNo(){
        insuranceUI.SetActive(false);
    }
    bool gameOver = false;
    bool splitPairAces = false;

    public IEnumerator TheDeal(){
        //initial deal
        for(int i = 0; i < 2; i++){
            DrawPlayerCard(playerCardHand, playerCardPosition);
            if(yourScore == 21){
                yield return new WaitForSeconds(.3f);
                playerTurnUI.enabled = false;
                StartCoroutine(PlayerBlackjack());
                StopCoroutine(theDeal);
                break;
            }
            yield return new WaitForSeconds(.3f);
            DrawDealerCard();
            yield return new WaitForSeconds(.3f);
        }
        //check for insurance
        if(dealerCardHand[0].GetComponent<SpriteRenderer>().sprite.name.Length >= 3 && dealerCardHand[0].GetComponent<SpriteRenderer>().sprite.name.Substring(0, 3) == "Ace" && GameManager.Instance.gamblingMoney >= betAmount * 2){
            insuranceUI.SetActive(true);
        }
        while(insuranceUI.activeInHierarchy){
            yield return null;
        }
        yield return new WaitForSeconds(.3f);
        if(dealerScore == 21){
                yield return new WaitForSeconds(.3f);
                playerTurnUI.enabled = false;
                backOfCard.SetActive(false);
                StartCoroutine(DealerBlackjack());
            }

        //check for double down
        if(GameManager.Instance.gamblingMoney >= betAmount * 2){
            if(yourScore == 9 || yourScore == 10 || yourScore == 11){
                doubleDownUI.SetActive(true);
            }else if(playerHasAce && (yourScore == 16 || yourScore == 17 || yourScore == 18)){
                doubleDownUI.SetActive(true);
            }
        }

        while(doubleDownUI.activeInHierarchy){
            yield return null;
        }
        //check for split pair
        if(GameManager.Instance.gamblingMoney >= betAmount * 2 && !doubleDown){
            if(CheckCardValue(playerCardHand[0]) == CheckCardValue(playerCardHand[1])) {
                if(CheckCardValue(playerCardHand[0]) == 1){
                    Debug.Log("ace split pair");
                    splitPairUI.SetActive(true);
                }
                else if(CheckCardValue(playerCardHand[0]) == 10){
                    if(SplitPair10Check(playerCardHand[0]) == SplitPair10Check(playerCardHand[1])){
                        Debug.Log("split pair 10s");
                        splitPairUI.SetActive(true);
                    }  
                }else{
                    Debug.Log("split pair normal");
                    splitPairUI.SetActive(true);
                }
            }
        }
        

        while(splitPairUI.activeInHierarchy){
            yield return null;
        }
        // if split pair aces, draw one card in each hand and be done
        if(splitPairAces){
            DrawPlayerCard(yourCardHandA, yourHandPositionA);
            yield return new WaitForSeconds(.5f);
            DrawPlayerCard(yourCardHandB, yourHandPositionB);
        }

        //player chooses to stand or hit until they want
        if(!gameOver && !splitPairAces && !doubleDown){
            PlayerTurn();
        }
        
        //loop waits for player to end turn
        while(playerTurnUI.enabled){
            if(yourScore == 21){
                playerTurnUI.enabled=false;
            }
            if(yourScore > 21){
                yield return new WaitForSeconds(.3f);
                playerTurnUI.enabled = false;
                StopCoroutine(theDeal);
                StartCoroutine(PlayerBust());
            }
            yield return null;
        }
        //when done with turn stand button turns player ui off
        backOfCard.SetActive(false);
        yield return new WaitForSeconds(1f);
        //dealer draws cards until more than 16
        while(dealerScore < 17 && yourScore <= 21 && (yourHandAScore <= 21 || yourHandBScore <= 21)){
            yield return new WaitForSeconds(.3f);
            DrawDealerCard();
            if(dealerHasAce && dealerScore > 21){
                dealerHasAce = false;
                dealerScore -= 10;
                dealerScoreText.text = "Your Score: " + yourScore;
            }
        }
        if(!gameOver){
           Invoke(nameof(EndGame), .3f); 
        }
    }
    public void CardHandSingleButton(){
        DrawPlayerCard(playerCardHand, playerCardPosition);
    }
    public void CardHand1HitButton(){
        DrawPlayerCard(yourCardHandA, yourHandPositionA);
    }
    public void CardHand2HitButton(){
        DrawPlayerCard(yourCardHandB, yourHandPositionB);
    }
    private int standClicks = 0;
    public Button hitButton;
    private int yourHandAScore = 0;
    private int yourHandBScore = 0;
    private void PlayerTurn(){
        playerTurnUI.gameObject.SetActive(true);
        playerTurnUI.enabled = true;
        if(splitPair){
            yourCardHandA.Add(Instantiate(playerCardHand[0]));
            yourHandAScore += CheckCardValue(yourCardHandA[0]);
            yourCardHandB.Add(Instantiate(playerCardHand[1]));
            yourHandBScore += CheckCardValue(yourCardHandB[0]);
            foreach(var card in playerCardHand){
                Destroy(card);
            }
            playerCardHand.Clear();
            StartCoroutine(SpaceCardsEvenly(yourCardHandA, 2f, yourHandPositionA.position.x));
            StartCoroutine(SpaceCardsEvenly(yourCardHandB, 2f, yourHandPositionB.position.x));
            hitButton.onClick.RemoveAllListeners();
            hitButton.onClick.AddListener(CardHand1HitButton);
        }
        else{
            hitButton.onClick.RemoveAllListeners();
            hitButton.onClick.AddListener(CardHandSingleButton);
        }
    }
    //button to draw card
    private List<GameObject> playerCardHand = new List<GameObject>();
    public void DrawPlayerCard(List<GameObject> playerHand, Transform handTransform){
        //for a split pair
        if(splitPair){
            if(standClicks == 0){
                GameObject drawnCard = Instantiate(DrawCard(), cardStackPostion.position, Quaternion.identity);
                if(numOfCardsDrawn < 1){
                    StartCoroutine(MoveCardOverTime(drawnCard.transform, cardStackPostion.position, yourHandPositionA.position, .15f));
                }
                numOfCardsDrawn++;
                yourHandAScore += CheckCardValue(drawnCard);
                yourCardHandA.Add(drawnCard);
                if(numOfCardsDrawn > 0){
                    StartCoroutine(SpaceCardsEvenly(yourCardHandA, 2f, yourHandPositionA.position.x));
                }
                if(playerHasAce && yourHandAScore > 21){
                    playerHasAce = false;
                    yourHandAScore -= 10;
                    yourScoreText.text = "Your Score: " + yourScore;
                }
                if(yourHandAScore > 21){
                    //this hand bust
                    StartCoroutine(HandABust());
                    GameManager.Instance.gamblingMoney -= betAmount/2;
                    standClicks++;
                    StartCoroutine(FlashFadeHand2Panel());
                    hitButton.onClick.RemoveAllListeners();
                    hitButton.onClick.AddListener(CardHand2HitButton);
                }
                if(yourHandAScore == 21){
                    standClicks++;
                    StartCoroutine(HandAWin());
                    GameManager.Instance.gamblingMoney += betAmount/2;
                    StartCoroutine(FlashFadeHand2Panel());
                    hitButton.onClick.RemoveAllListeners();
                    hitButton.onClick.AddListener(CardHand2HitButton);
                }
                
            }else if(standClicks == 1){
                GameObject drawnCard = Instantiate(DrawCard(), cardStackPostion.position, Quaternion.identity);
                if(numOfCardsDrawn < 1){
                    StartCoroutine(MoveCardOverTime(drawnCard.transform, cardStackPostion.position, yourHandPositionB.position, .15f));
                }
                numOfCardsDrawn++;
                yourHandBScore += CheckCardValue(drawnCard);
                yourCardHandB.Add(drawnCard);
                if(numOfCardsDrawn > 0){
                    StartCoroutine(SpaceCardsEvenly(yourCardHandB, 2f, yourHandPositionB.position.x));
                }
                if(playerHasAce && yourHandBScore > 21){
                    playerHasAce = false;
                    yourHandBScore -= 10;
                    yourScoreText.text = "Your Score: " + yourScore;
                }
                if(yourHandBScore > 21 && yourHandAScore > 21){
                    //both bust end game before dealer last turn
                    playerTurnUI.enabled = false;
                    StartCoroutine(HandBBust());
                    GameManager.Instance.gamblingMoney -= betAmount/2;
                    StopCoroutine(theDeal);
                    Invoke(nameof(EndGame), .3f);
                }else if(yourHandBScore > 21){
                    playerTurnUI.enabled = false;
                    Invoke(nameof(EndGame), .3f);
                    GameManager.Instance.gamblingMoney -= betAmount/2;
                }else if(yourHandBScore == 21){
                    StartCoroutine(HandBWin());
                    GameManager.Instance.gamblingMoney += betAmount/2;
                    playerTurnUI.enabled = false;
                }
            }
        //normal single hand
        }else{
            GameObject drawnCard = Instantiate(DrawCard(), cardStackPostion.position, Quaternion.identity);
            if(numOfCardsDrawn < 1){
                StartCoroutine(MoveCardOverTime(drawnCard.transform, cardStackPostion.position, playerCardPosition.position, .15f));
            }
            numOfCardsDrawn++;
            yourScore += CheckCardValue(drawnCard);
            playerCardHand.Add(drawnCard);
            if(numOfCardsDrawn > 0){
                StartCoroutine(SpaceCardsEvenly(playerCardHand, 2f, handTransform.position.x));
            }
            yourScoreText.text = "Your Score: " + yourScore;
            if(playerHasAce && yourScore > 21){
                playerHasAce = false;
                yourScore -= 10;
                yourScoreText.text = "Your Score: " + yourScore;
            }
        }

    }
    //button to end game
    public void Stand(){
        
        standClicks++;
        if(standClicks == 1 && splitPair){
            numOfCardsDrawn = 1;
            hitButton.onClick.RemoveAllListeners();
            StartCoroutine(FlashFadeHand2Panel());
            hitButton.onClick.AddListener(CardHand2HitButton);
        }else{
            playerTurnUI.enabled = false;
        }
    }
    private void EndGame(){
        backOfCard.GetComponent<SpriteRenderer>().enabled = false;
        playerTurnUI.gameObject.SetActive(false);
        gameOver = true;
        //split pair end game
        if(splitPair){
            if(dealerScore > 21){
                StartCoroutine(DealerBust());
                return;
            }else if(dealerScore == yourHandAScore){
                StartCoroutine(TieHandA());
            }else if(yourHandAScore > 21){
                StartCoroutine(HandABust());
            }else if(yourHandAScore > dealerScore){
                StartCoroutine(HandAWin());
            }else{
                StartCoroutine(HandALoss());
            }
            
            if(dealerScore == yourHandBScore){
                StartCoroutine(TieHandB());
            }else if(yourHandBScore > 21){
                StartCoroutine(HandBBust());
            }else if(yourHandBScore > dealerScore){
                StartCoroutine(HandBWin());
            }else{
                StartCoroutine(HandBLoss());
            }
            Invoke(nameof(ResetGame), 2f);
            
            //normal end game
        }else{
            if(dealerScore == yourScore){
                StartCoroutine(Tie());
            }
            else if(dealerScore > 21){
                StartCoroutine(DealerBust());
            }
            else if(yourScore > dealerScore){
                StartCoroutine(PlayerWin());  
            }else if(dealerScore > yourScore){
                StartCoroutine(DealerWin());
            }
        }
        
        
    }
    private void ResetGame(){
        hitButton.onClick.RemoveAllListeners();
        GameManager.Instance.UpdateGamblingMoney();
        gameOver = false;
        numOfCardsDrawn = 0;
        dealerCardsDrawn = 0;
        standClicks = 0;
        yourScore = 0;
        yourHandAScore = 0;
        yourHandBScore = 0;
        dealerScore = 0;
        betAmount = 0;
        backOfCard.transform.parent = null;
        backOfCard.SetActive(false);
        dealerScoreText.text = "Dealer Score: 0";
        yourScoreText.text = "Your Score: 0";
        foreach(GameObject card in dealerCardHand){
            Destroy(card);
        } 
        dealerCardHand.Clear();
        if(splitPair){
            foreach(GameObject card in yourCardHandA){
                Destroy(card);
            }
            yourCardHandA.Clear();
            foreach(GameObject card in yourCardHandB){
                Destroy(card);
            }
            yourCardHandB.Clear();
        }else{
            foreach(GameObject card in playerCardHand){
                Destroy(card);
            } 
            playerCardHand.Clear();
        }
        splitPair = false;
        doubleDown = false;
        gameCards.Clear();
        playerHasAce = false;
        dealerHasAce = false;
        GameManager.Instance.UpdateGamblingMoney();
        Bet();
    }
    private void ResetGameTie(){
        hitButton.onClick.RemoveAllListeners();
        GameManager.Instance.UpdateGamblingMoney();
        gameOver = false;
        numOfCardsDrawn = 0;
        dealerCardsDrawn = 0;
        standClicks = 0;
        yourScore = 0;
        yourHandAScore = 0;
        yourHandBScore = 0;
        dealerScore = 0;
        backOfCard.transform.parent = null;
        backOfCard.SetActive(false);
        dealerScoreText.text = "Dealer Score: 0";
        yourScoreText.text = "Your Score: 0";
        foreach(GameObject card in dealerCardHand){
            Destroy(card);
        } 
        dealerCardHand.Clear();
        if(splitPair){
            foreach(GameObject card in yourCardHandA){
                Destroy(card);
            }
            yourCardHandA.Clear();
            foreach(GameObject card in yourCardHandB){
                Destroy(card);
            }
            yourCardHandB.Clear();
        }else{
            foreach(GameObject card in playerCardHand){
                Destroy(card);
            } 
            playerCardHand.Clear();
        }
        splitPair = false;
        doubleDown = false;
        gameCards.Clear();
        playerHasAce = false;
        dealerHasAce = false;
        BlackjackGame();
    }

    IEnumerator SpaceCardsEvenly(List<GameObject> cardsToSpace, float spacing, float xOffset){
        yield return new WaitForSeconds(.15f);
            // Calculate the total width of all objects combined with spacing
        float totalWidth = (cardsToSpace.Count - 1) * spacing;

        // Loop through each GameObject and position it
        for (int i = 0; i < cardsToSpace.Count; i++)
        {
            // Calculate the position of the current GameObject
            float xPos = -totalWidth / 2.0f + i * spacing + xOffset;
            
            Vector2 newPosition = new Vector3(xPos, cardsToSpace[0].transform.position.y);

            // Set the new position of the GameObject
            StartCoroutine(MoveCardOverTime(cardsToSpace[i].transform, cardsToSpace[i].transform.position, newPosition, 0.15f));
            yield return new WaitForSeconds(.1f);
        }
    }
    public CanvasGroup canvasGroup;
    public Image hand1Panel;
    private IEnumerator FlashFadeHand1Panel(){
        hand1Panel.enabled = true;
        float startAlpha = canvasGroup.alpha;
        float endAlpha = 0f;
        float elapsedTime = 0f;
        while(elapsedTime < 1f){
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / 1f);
            yield return null;
        }
        hand1Panel.enabled = false;
        canvasGroup.alpha = 1f;
    }
    public Image hand2Panel;
    private IEnumerator FlashFadeHand2Panel(){
        hand2Panel.enabled = true;
        float startAlpha = canvasGroup.alpha;
        float endAlpha = 0f;
        float elapsedTime = 0f;
        while(elapsedTime < 1f){
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / 1f);
            yield return null;
        }
        hand2Panel.enabled = false;
        canvasGroup.alpha = 1f;
    }

    #region End Conditions
        
    private float endGameTextWaitTime = 1f;
    //split endings
    public TextMeshProUGUI tieHandAText;
    private IEnumerator TieHandA(){
        tieHandAText.enabled = true;
        yield return new WaitForSeconds(endGameTextWaitTime);
        tieHandAText.enabled = false;
    }
    public TextMeshProUGUI handABustText;
    private IEnumerator HandABust(){
        handABustText.enabled = true;
        yield return new WaitForSeconds(endGameTextWaitTime);
        handABustText.enabled = false;
    }
    public TextMeshProUGUI handAWinText;
    private IEnumerator HandAWin(){
        handAWinText.enabled = true;
        yield return new WaitForSeconds(endGameTextWaitTime);
        handAWinText.enabled = false;
    }
    public TextMeshProUGUI handALossText;
    private IEnumerator HandALoss(){
        handALossText.enabled = true;
        GameManager.Instance.gamblingMoney -= betAmount/2;
        yield return new WaitForSeconds(endGameTextWaitTime);
        handALossText.enabled = false;
    }

    public TextMeshProUGUI tieHandBText;
    private IEnumerator TieHandB(){
        tieHandBText.enabled = true;
        yield return new WaitForSeconds(endGameTextWaitTime);
        tieHandBText.enabled = false;
    }
    public TextMeshProUGUI handBBustText;
    private IEnumerator HandBBust(){
        handBBustText.enabled = true;
        yield return new WaitForSeconds(endGameTextWaitTime);
        handBBustText.enabled = false;
    }
    public TextMeshProUGUI handBWinText;
    private IEnumerator HandBWin(){
        handBWinText.enabled = true;
        yield return new WaitForSeconds(endGameTextWaitTime);
        handBWinText.enabled = false;
    }
    public TextMeshProUGUI handBLossText;
    private IEnumerator HandBLoss(){
        handBLossText.enabled = true;
        GameManager.Instance.gamblingMoney -= betAmount/2;
        yield return new WaitForSeconds(endGameTextWaitTime);
        handBLossText.enabled = false;
    }


    //normal endings
    public TextMeshProUGUI playerBustText;
    private IEnumerator PlayerBust(){
        playerBustText.enabled = true;
        gameOver = true;
        GameManager.Instance.gamblingMoney -= betAmount;
        yield return new WaitForSeconds(endGameTextWaitTime);
        playerBustText.enabled = false;
        Invoke(nameof(ResetGame), 2f);
    }
    public TextMeshProUGUI dealerBustText;
    private IEnumerator DealerBust(){
        dealerBustText.enabled = true;
        gameOver = true;
        GameManager.Instance.gamblingMoney += betAmount;
        yield return new WaitForSeconds(endGameTextWaitTime);
        dealerBustText.enabled = false;
        Invoke(nameof(ResetGame), 2f);
    }
    public TextMeshProUGUI playerWinText;
    private IEnumerator PlayerWin(){
        playerWinText.enabled = true;
        gameOver = true;
        GameManager.Instance.gamblingMoney += betAmount;
        yield return new WaitForSeconds(endGameTextWaitTime);
        playerWinText.enabled = false;
        Invoke(nameof(ResetGame), 2f);
    }
    public TextMeshProUGUI dealerWinText;
    private IEnumerator DealerWin(){
        dealerWinText.enabled = true;
        gameOver = true;
        GameManager.Instance.gamblingMoney -= betAmount;
        yield return new WaitForSeconds(endGameTextWaitTime);
        dealerWinText.enabled = false;
        Invoke(nameof(ResetGame), 2f);
    }
    public TextMeshProUGUI playerBlackjackText;
    private IEnumerator PlayerBlackjack(){
        StopCoroutine(theDeal);
        gameOver = true;
        playerTurnUI.gameObject.SetActive(false);
        playerBlackjackText.enabled = true;
        if(numOfCardsDrawn == 2){
            GameManager.Instance.gamblingMoney += (int)(betAmount * 1.5f);
        }else{
            GameManager.Instance.gamblingMoney += (int)betAmount;
        }
        
        yield return new WaitForSeconds(endGameTextWaitTime);
        playerBlackjackText.enabled = false;
        Invoke(nameof(ResetGame), 2f);
    }
    public TextMeshProUGUI dealerBlackjackText;
    private IEnumerator DealerBlackjack(){
        StopCoroutine(theDeal);
        gameOver = true;
        playerTurnUI.gameObject.SetActive(false);
        dealerBlackjackText.enabled = true;
        GameManager.Instance.gamblingMoney -= betAmount;
        yield return new WaitForSeconds(endGameTextWaitTime);
        dealerBlackjackText.enabled = false;
        Invoke(nameof(ResetGame), 2f);
    }
    public TextMeshProUGUI tieWinText;
    private IEnumerator Tie(){
        tieWinText.enabled = true;
        yield return new WaitForSeconds(endGameTextWaitTime);
        tieWinText.enabled = false;
        Invoke(nameof(ResetGameTie), 2f);
        
    }
    #endregion
    private bool isDealersTurn = false;
    private List<GameObject> dealerCardHand = new List<GameObject>();
    private bool dealerHasAce = false;
    private int dealerCardsDrawn = 0;
    public void DrawDealerCard(){
        isDealersTurn = true;
        GameObject drawnCard = Instantiate(DrawCard(), cardStackPostion.position, Quaternion.identity);
        if(dealerCardsDrawn < 1){
            StartCoroutine(MoveCardOverTime(drawnCard.transform, cardStackPostion.position, dealerCardPosition.position, .15f));
        }
        dealerScore += CheckCardValue(drawnCard);
        dealerCardsDrawn++;
        dealerCardHand.Add(drawnCard);
        if(dealerCardsDrawn > 0){
            StartCoroutine(SpaceCardsEvenly(dealerCardHand, 2f, 0));
        }
        dealerScoreText.text = "Dealer Score: " + dealerScore;
        if(dealerCardHand.Count == 2){
            backOfCard.SetActive(true);
            backOfCard.transform.position = dealerCardHand[1].transform.position;
            backOfCard.transform.parent = dealerCardHand[1].transform;
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
                    if(dealerScore < 11){
                        dealerHasAce = true;
                        return 11;
                    }
                    else{
                        return 1;
                    }
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
    private string SplitPair10Check(GameObject drawnCard){
        switch(drawnCard.GetComponent<SpriteRenderer>().sprite.name){
            case "10S": case "10C": case "10D": case "10H":
                return "10";
            case "JackC": case "JackD": case "JackS": case "JackH":
                return "Jack";
            case "QueenC": case "QueenD": case "QueenS": case "QueenH":
                return "Queen";
            case "KingC": case "KingD": case "KingS": case "KingH":
                return "King";
            default:
                return "";
        }
    }
}
