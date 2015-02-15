using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public abstract class AbstractVillage 
{
	public VillageType myType { get; protected set; }
	public int gold { get; protected set; }
	public int wood { get; protected set; }
	public IList<AbstractTile> controlledRegion { get; protected set; }
	public IList<AbstractUnit> supportedUnits { get; protected set; }
	public AbstractTile location { get; protected set; }
	public AbstractPlayer myPlayer { get; protected set; }
	
	public AbstractVillage(IList<AbstractTile> region, AbstractPlayer player)
	{
		myType = VillageType.Hovel;
		gold = 0;
		wood = 0;
		
		controlledRegion = region;
		supportedUnits = new List<AbstractUnit> ();
		myPlayer = player;
	}
	
	public abstract void swapControl(AbstractTile myTile, AbstractVillage other);
}
