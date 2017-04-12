using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwapper : MonoBehaviour {

	[SerializeField] private Material opaque = null;
	[SerializeField] private Material transparent = null;

	private MeshRenderer mesh;

	void Start () {
		mesh = GetComponent<MeshRenderer> ();
		mesh.material.color = new Color (1, 1, 1, 1);
	}

	public void SwapToOpaque () {
		mesh.material = opaque;
	}

	public void SwapToTransparent () {
		mesh.material = transparent;
		Debug.Log ("hi");
	}
}
