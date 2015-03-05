using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public abstract class AbstractVillage 
{
	public VillageType myType { get; set; }
	public int gold { get; set; }
	public int wood { get; set; }
	public List<AbstractTile> controlledRegion { get; protected set; }
	public List<AbstractUnit> supportedUnits { get; protected set; }
	public AbstractTile location { get; set; } //changed from protected set because I need to set location of village in newGame
	public AbstractPlayer myPlayer { get; protected set; }

	public abstract void swapControl(AbstractTile myTile, AbstractVillage other);
}
