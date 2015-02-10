using UnityEngine;
using System.Collections;
using GameEnums;

public abstract class AbstractTile 
{
	private LandType myType { get; set; }
	private AbstractStructure occupyingStructure { get; set; }
	private AbstractVillage myVillage{ get; set; }
	private AbstractUnit occupyingUnit { get; set; }

	public AbstractTile( LandType lt);

}
