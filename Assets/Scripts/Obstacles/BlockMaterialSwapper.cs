using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMaterialSwapper : MaterialSwapper {

	new void Start () {

		base.Start ();
		opaque = new Material [1];
		opaque [0] = mesh.material;
	}
}
