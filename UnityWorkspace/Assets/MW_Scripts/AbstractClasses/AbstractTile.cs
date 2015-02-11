using UnityEngine;
using System.Collections;
using GameEnums;

public abstract class AbstractTile 
{
	public LandType myType { get; private set; }
	public AbstractStructure occupyingStructure { get; private set; }
	public AbstractVillage myVillage{ get; private set; }
	public AbstractUnit occupyingUnit { get; private set; }
}
