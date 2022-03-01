using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interactable class specific to flipping a tile for the purpose of these placeholder mechanics
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HexSpace))]
public class TileFlip : Interactable {
    //Required components for class to execute
    //Transform transform;
	Animator anim;

	public AudioSource audioSource;

	TileGrid grid;
	HexSpace hexSpace;

	public delegate void OnTileFlip(HexSpace space);
	public event OnTileFlip FlipCallback;
	public event OnTileFlip OriginCallback;

	public override void Start() {
		base.Start();

		grid = TileGrid.instance;
		hexSpace = GetComponent<HexSpace>();

		anim = GetComponent<Animator>();
		anim.SetBool("Destroy", false);
	}
//

	public override void Interact() {
		base.Interact();

		StartCoroutine(FlipTile(true));
	}

//Play tile flip animation, trigger adjacent tiles if origin, send events to TileGenerator
	public IEnumerator FlipTile(bool origin) {
		StartCoroutine(hover.Deactivate());
		anim.SetBool("Flip", true);
		//yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length);
		//Hardcoded delay bc I couldn't figure out delaying by current animation clip ^^^
		yield return new WaitForSeconds(.25f);
		if (!origin) audioSource.volume = audioSource.volume / 4;
		audioSource.Play();
		if (!origin) audioSource.volume = audioSource.volume * 2;
		anim.SetBool("Flip", false);

		//hexSpace.FlipHexSpace(grid.gridDef.colors); these too lines are redundant, but I think I want this one
		FlipCallback?.Invoke(hexSpace);
		yield return new WaitForSeconds(.125f);
		if (origin) { 
			OriginCallback?.Invoke(hexSpace);
			active = true;
		}
	}
//Plays one-way flip animation and destroys gameObject
	public IEnumerator DestroyTile() {
		anim.SetBool("Destroy", true);
		anim.SetBool("Flip", true);
		yield return new WaitForSeconds(.125f);
		audioSource.volume = audioSource.volume / 8;
		audioSource.Play();
		yield return new WaitForSeconds(1);
		Destroy(gameObject);
	}
}
