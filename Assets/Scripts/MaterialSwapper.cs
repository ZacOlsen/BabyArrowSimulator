﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwapper : MonoBehaviour {

	[SerializeField] private Material opaque = null;
	[SerializeField] private Material transparent = null;

	private MeshRenderer mesh;

	void Start () {
		mesh = GetComponent<MeshRenderer> ();
	}

	public void SwapToOpaque () {
		mesh.material = opaque;
	}

	public void SwapToTransparent () {
		mesh.material = transparent;
	}
}
