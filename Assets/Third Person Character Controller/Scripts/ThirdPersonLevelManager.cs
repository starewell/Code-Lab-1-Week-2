using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThirdPersonLevelManager : MonoBehaviour {
    
    #region Singleton
    public static ThirdPersonLevelManager instance;
    private void Awake() {
        if (instance != null) {
            Debug.LogWarning("More than one instance of ThirdPersonLevelManager found!");
            return;
        }
        instance = this;
    }
    #endregion

    ThirdPersonPlayerController player;
    ThirdPersonGameManager gameManager;

    [SerializeField] GameObject levelResetScreen, levelWinScreen;

    void Start() {
        // assign and get our variables in our player instance
        if (ThirdPersonPlayerController.instance != null) { 
            player = ThirdPersonPlayerController.instance;
            player.PlayerDeathCallback += TriggerLevelReset;
            player.PlayerWinCallback += TriggerLevelWinScreen;
        }
        if (ThirdPersonGameManager.instance != null) {
            gameManager = ThirdPersonGameManager.instance;
        }
        levelResetScreen.SetActive(false);
        levelWinScreen.SetActive(false);
    }

    // function to trigger our win screen
    public void TriggerLevelWinScreen(int dumby) {
        levelWinScreen.SetActive(true);
    }

    // function to trigger our reset, called by an event, triggers coroutine
    public void TriggerLevelReset(int dumby) {
        StartCoroutine(TriggerGameManagerReset());
    }

    // level reset coroutine called by TriggerLevelReset()
    IEnumerator TriggerGameManagerReset() {
        yield return new WaitForSeconds(1);
        if (gameManager) {
            ToggleResetScreen(true);
            yield return new WaitForSeconds(1.5f);
            gameManager.LevelReset();
        }
    }

    // show/hide our blue screen of death screen
    public void ToggleResetScreen(bool active) {
        levelResetScreen.SetActive(active);
    }

}
