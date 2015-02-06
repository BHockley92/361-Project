using UnityEngine;
using System.Collections;
using GameEnums;

public class Tile 
{
	private LandType myType { get; }
	private Structure occupyingStructure { get; }
	private Village myVillage{ get; set; }
}
