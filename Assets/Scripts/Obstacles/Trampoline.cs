using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour {

	[SerializeField][Range(0, 1)] private float energyConserved = .66f;

	public float GetEnergyConserved () {
		return energyConserved;
	}
}
