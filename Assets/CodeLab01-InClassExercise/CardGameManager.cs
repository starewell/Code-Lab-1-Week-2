using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameManager : MonoBehaviour {

    float cardWidth = 4;

    [SerializeField] int cardCount;
    [SerializeField] GameObject cardPrefab;

    List<GameObject> cards = new List<GameObject>();

    public enum GameState { CreateCards, Select, CardSelected }
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
                        -cardWidth * cardCount/2 + (i * cardWidth) + cardWidth/2,
                        gameObject.transform.position.y,
                        gameObject.transform.position.z);
                    newCard.transform.position = newPos;
                    newCard.GetComponent<CardBehavior>().CardFlippedCallback += SingleCardSelected;
                }
                TransitionStates(GameState.Select);
                break;
            case GameState.Select:


                break;
        }     
    }
}
