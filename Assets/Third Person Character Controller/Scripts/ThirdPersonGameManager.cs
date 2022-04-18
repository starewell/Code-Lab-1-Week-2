using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ThirdPersonGameManager : MonoBehaviour {

    #region Singleton
    [SerializeField]
    public static ThirdPersonGameManager instance;
    void Awake() {
        if (instance != null) {
            Debug.LogWarning("More than one instance of ThirdPersonGameManager found!");
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }
    #endregion

    [Header("References")]
    [SerializeField] ThirdPersonLevelManager levelManager;

    void Start() {

        UpdateInstances(false);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        UpdateInstances(false);
    }

    public void LevelCompleted() {
        UpdateInstances(true);
        SceneManager.LoadScene("TriggerZones");
    }

    public void LevelReset() {
        SceneManager.LoadScene("DebugGrounds");
    }

    public void UpdateInstances(bool remove) { 
        if (remove) {
            levelManager = null;
        }
        else if (!remove) {
            if (ThirdPersonLevelManager.instance) {
                levelManager = ThirdPersonLevelManager.instance;                
            }
        }
    }
}
