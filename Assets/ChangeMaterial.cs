using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    [SerializeField] Material[] materials;
    int currentMatIndex;
    
    public void CycleToNextMat() {
        currentMatIndex++;
        if (currentMatIndex > materials.Length) currentMatIndex = 0;
        gameObject.GetComponent<Renderer>().material = materials[currentMatIndex];
    }

    public void DisableGameObject() {
        this.gameObject.SetActive(false);
    }
}
