using UnityEngine;
using System.Collections;
using GameEnums;

public abstract class AbstractTile 
{
	public LandType myType { get; set; }
	public AbstractStructure occupyingStructure { get; set; }
	public AbstractVillage myVillage{ get; set; }
	public AbstractUnit occupyingUnit { get; set; }
}
