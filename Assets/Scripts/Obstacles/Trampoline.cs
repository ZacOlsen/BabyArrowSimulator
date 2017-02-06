using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour {

	[SerializeField] private float energyConserved = .66f;

	public float GetEnergyConserved () {
		return energyConserved;
	}
}
