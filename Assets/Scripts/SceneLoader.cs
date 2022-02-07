
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
//Variables for screen fading, only implemented when the game starts rn
    public GameObject blackImage;
    public float fadeDuration;
//
//Event for signaling when a scene is loaded
    public delegate void OnSceneChange();
    public event OnSceneChange SceneLoadedCallback;


//Week 2 Singleton Pattern
    private static SceneLoader instance;
    public static SceneLoader GetInstance() {
        return instance;
    }
    void Awake() { 
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded; //Subscribe to SceneLoaded delegate
    }

    void Start() {
        StartCoroutine(FadeInCam());
        
    }
//Subscribed function to SceneLoaded callback, invokes its own callback for other scripts to reference -- mostly GameManager rn
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        SceneLoadedCallback?.Invoke();

    }
//Don't really need this rn but we'll see if it'll be useful later
    public void ChangeScene(string name) {

        SceneManager.LoadScene(name);
    }
//Not using this how my scenes transition currently, but functionality is there if I want it
    public IEnumerator FadeOutCam() {
        Color alpha = new Color(0, 0, 0, 0);
        blackImage.SetActive(true);
        blackImage.GetComponent<Image>().color = alpha;
        while (alpha.a < 1) {
            alpha = new Color(alpha.r, alpha.g, alpha.b, alpha.a += 0.1f);
            blackImage.GetComponent<Image>().color = alpha;
            yield return new WaitForSeconds(fadeDuration/60);
        }
        yield return new WaitForSeconds(1);
    }
//Fades in the cam by fading out black image... could be clearer but only used once so
    public IEnumerator FadeInCam() {
        Color alpha = new Color(0, 0, 0, 1);
        blackImage.SetActive(true);
        blackImage.GetComponent<Image>().color = alpha;
        while (alpha.a > 0) {
            alpha = new Color(alpha.r, alpha.g, alpha.b, alpha.a -= 0.1f);
            blackImage.GetComponent<Image>().color = alpha;
            yield return new WaitForSeconds(fadeDuration/60);
        }
        blackImage.SetActive(false);
    }

    
}
