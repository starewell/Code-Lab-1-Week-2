using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGameManager : MonoBehaviour {

    float cardWidth = 4;

    [SerializeField] int cardCount;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Vector3 handStartPos;

    List<GameObject> cards = new List<GameObject>();

    public float enemyHealth;
    [SerializeField] Text enemyText;

    public enum GameState { CreateCards, Deal, Select, CardSelected, Resolve }
    public GameState currentState;

    private static CardGameManager instance;
    public static CardGameManager GetInstance() {
        return instance;
    }
    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }     
    }

    private void Start() { 
        TransitionStates(GameState.CreateCards);
        enemyText.text = "HEALTH: " + enemyHealth.ToString();
    }

    void Update() {
        RunState();
    }

    void SingleCardSelected() {
        TransitionStates(GameState.CardSelected);
    }

    public void TransitionStates(GameState state) {
        currentState = state;
        switch (state) {
            default:
                break;
            case GameState.CreateCards:
                cards = new List<GameObject>();
                for(int i = 0; i < cardCount; i++) {
                    GameObject newCard = Instantiate(cardPrefab);
                    Vector3 newPos = new Vector3(
                        handStartPos.x,
                        handStartPos.y,
                        -handStartPos.z);
                    newCard.transform.position = newPos;
                    newCard.GetComponent<CardBehavior>().CardFlippedCallback += SingleCardSelected;
                    cards.Add(newCard);
                }
                TransitionStates(GameState.Deal);
                break;
            case GameState.Deal:

                break;
            case GameState.Select:


                break;
        }     
    }

    void RunState() { 
        switch (currentState) {
            case GameState.Deal:
                for (int i = 0; i < cards.Count; i++) {
                    float step = 5.0f * Time.deltaTime;
                    Vector3 newPos = new Vector3(-cardWidth * cardCount / 2 + (i * cardWidth) + cardWidth / 2, handStartPos.y, handStartPos.z);
                    cards[i].transform.position = Vector3.MoveTowards(cards[i].transform.position, newPos, step);
                    if (i == cards.Count - 1 && Vector3.Distance(cards[i].transform.position, newPos) < 0.1f)
                        TransitionStates(GameState.Select);
                }
                break;

        }
    }
}


/*Vector3 newPos = new Vector3(
                        -cardWidth * cardCount/2 + (i * cardWidth) + cardWidth/2,
                        gameObject.transform.position.y,
                        gameObject.transform.position.z);*/