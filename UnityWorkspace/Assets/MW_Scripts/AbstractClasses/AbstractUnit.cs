using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public abstract class AbstractUnit 
{
	private UnitType myType { get; set; }
	private ActionType currentAction { get; set; }
	
	private IList<AbstractVillage> myVillage { get; set; }
	private AbstractTile myLocation { get; set; }
}
