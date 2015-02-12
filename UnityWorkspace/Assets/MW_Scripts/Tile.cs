using UnityEngine;
using System.Collections;
using GameEnums;

public class Tile : AbstractTile 
{
	public LandType myType { get; private set; }
	public AbstractStructure occupyingStructure { get; private set; }
	public AbstractVillage myVillage{ get; private set; }
	public AbstractUnit occupyingUnit { get; private set; }

	public Tile( LandType type, AbstractVillage village)
	{
		myType = type;
		myVillage = village;
		occupyingStructure = null;
		occupyingUnit = null;
	}
}
