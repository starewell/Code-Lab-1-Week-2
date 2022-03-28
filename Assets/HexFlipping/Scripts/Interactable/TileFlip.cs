using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interactable class specific to flipping a tile for the purpose of these placeholder mechanics
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HexSpace))]
public class TileFlip : Interactable {
/// <summary>
/// Derrived interactable class, this class is responsible for recieving and acting upon the player input onto a HexSpace
/// </summary>

	Animator anim;

	public AudioSource audioSource;

	HexSpace hexSpace;

	public delegate void OnTileFlip(HexSpace space, bool playerInput = true);
	public event OnTileFlip FlipCallback;
	public event OnTileFlip OriginCallback;
	public event OnTileFlip PlayerActionCallback;

	public override void Start() {
		base.Start();

		hexSpace = GetComponent<HexSpace>();

		anim = GetComponent<Animator>();
		anim.SetBool("Destroy", false);
	}
//

	protected override void Interact() {
		base.Interact();

		PlayerActionCallback?.Invoke(GetComponent<HexSpace>()); //Signals when the player has interacted with a Tile versus an Actor. Passing useless parameter... I should go to helpdesk
		StartCoroutine(FlipTile(true));
	}

//Play tile flip animation, trigger adjacent tiles if origin, send events to TileGenerator
	public IEnumerator FlipTile(bool origin, bool playerInput = true) {
		StartCoroutine(hover.Deactivate());
		anim.SetBool("Flip", true);
		//yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length);
		//Hardcoded delay bc I couldn't figure out delaying by current animation clip ^^^
		yield return new WaitForSeconds(.25f);
		if (!origin) audioSource.volume /= 4;
		audioSource.Play();
		if (!origin) audioSource.volume *= 4;
		anim.SetBool("Flip", false);

		//hexSpace.FlipHexSpace(grid.gridDef.colors); these too lines are redundant, but I think I want this one
		FlipCallback?.Invoke(hexSpace, playerInput);
		yield return new WaitForSeconds(.125f);
		if (origin) 
			OriginCallback?.Invoke(hexSpace, playerInput);			
		
	}
//Plays one-way flip animation and destroys gameObject
	public IEnumerator DestroyTile() {
		anim.SetBool("Destroy", true);
		anim.SetBool("Flip", true);
		yield return new WaitForSeconds(.125f);
		audioSource.volume /= 8;
		audioSource.Play();
		yield return new WaitForSeconds(1);
		Destroy(gameObject);
	}
}
