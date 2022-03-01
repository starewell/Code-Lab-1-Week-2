using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Simple animation script for changing renderer material anim variables and active state
public class PadlockAnimation : MonoBehaviour
{
    [SerializeField] Material[] materials;
    [SerializeField] AudioSource audioSource;
    int currentMatIndex;
    
    public void CycleToNextMat() {
        currentMatIndex++;
        if (currentMatIndex > materials.Length) currentMatIndex = 0;
        gameObject.GetComponent<Renderer>().material = materials[currentMatIndex];
    }

    public void DisableGameObject() {
        this.gameObject.SetActive(false);
    }

    public void PlayAudio() {
        audioSource.Play();
    }
}
