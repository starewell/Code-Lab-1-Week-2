using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Scraps from the TileFlip class to utilize for button mechanics... Just stole them from the UI.Button class
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Animator))]
public class TileButton : Interactable {

	public AudioSource audioSource;
	Animator anim;
	Button button; //...Passing executable functions through unity button rather than hardcoded reference... why is this bad

	public override void Start() {
        base.Start();

        active = true;
		anim = GetComponent<Animator>();
		anim.SetBool("Destroy", false);

		button = GetComponent<Button>(); 
	}

    public override void Interact() {
        base.Interact();

		StartCoroutine(FlipTile());
    }

	public IEnumerator FlipTile() {
		active = true;
		StartCoroutine(hover.Deactivate());
		anim.SetBool("Flip", true);
		//yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length);
		//Hardcoded delay bc I couldn't figure out delaying by current animation clip ^^^
		yield return new WaitForSeconds(.25f);
		audioSource.Play();
		anim.SetBool("Flip", false);
		button.onClick.Invoke();
	}
}
