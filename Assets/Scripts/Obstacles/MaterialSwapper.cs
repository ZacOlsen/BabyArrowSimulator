using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwapper : MonoBehaviour {

	[SerializeField] protected Material[] opaque = null;
	[SerializeField] private Material[] transparent = null;

	protected MeshRenderer mesh;

	protected void Start () {
		mesh = GetComponent<MeshRenderer> ();
	}

	public void SwapToOpaque () {
		mesh.materials = opaque;
	}

	public void SwapToTransparent () {
		mesh.materials = transparent;
	}
}
