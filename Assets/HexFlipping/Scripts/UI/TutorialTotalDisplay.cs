using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Scrap from real TotalsDisplay class, but for singular use case and not hooked up to other classes
public class TutorialTotalDisplay : MonoBehaviour
{
    public Slider slider;
    public Text[] texts;
    public FlipGrid grid;

    void Start() {
        grid.TotalsChangeCallback += UpdateTotal;
        UpdateGoal();
    }

    void UpdateGoal() {
        texts[1].text = "/ 5";
        slider.maxValue = 5;
    }

    void UpdateTotal(float r, float g, float b) {
        slider.value = g;
        texts[0].text = g.ToString();
    }

}
