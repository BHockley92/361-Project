using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public abstract class AbstractUnit 
{
	public UnitType myType { get; private set; }
	public ActionType currentAction { get; private set; }
	
	public AbstractVillage myVillage { get; private set; }
	public AbstractTile myLocation { get; private set; }

	public PlayerPrefs getPlayer()
	{
		return myVillage.myPlayer;
	}
}
