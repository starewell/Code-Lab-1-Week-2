using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBehavior : MonoBehaviour {

    CardGameManager myManager;

    [SerializeField] MeshRenderer myRenderer;
    [SerializeField] Color faceColor;
    [SerializeField] Color backColor;

    public enum State { FaceUp, FaceDown};
    State currentState;

    public delegate void OnCardInteraction();
    public event OnCardInteraction CardFlippedCallback;

    void Start() {
        TransitionState(State.FaceDown);

        myManager = CardGameManager.GetInstance();
    }

    void OnMouseDown() { 
        if (myManager.currentState == CardGameManager.GameState.Select) { 
            switch (currentState) {
                case State.FaceDown:
                    TransitionState(State.FaceUp);
                    break;
                case State.FaceUp:
                    TransitionState(State.FaceDown);
                    break;       
            }
            CardFlippedCallback?.Invoke();
        }
    }

    void TransitionState(State state) { 
        switch (currentState) {
            case State.FaceDown:
                currentState = State.FaceUp;
                myRenderer.material.color = faceColor;
                currentState = state;
                break;
            case State.FaceUp:
                currentState = State.FaceDown;
                myRenderer.material.color = backColor;
                currentState = state;
                break;       
        }
    }
}
