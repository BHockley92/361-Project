using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public abstract class AbstractVillage 
{
	private VillageType myType { get; set; }
	private int gold { get; set; }
	private int wood { get; set; }
	private IList<AbstractTile> controlledRegion { get; set; }
	private IList<AbstractUnit> supportedUnits { get; set; }
	private AbstractTile location { get; set; }
	private AbstractPlayer myPlayer { get; set; }
	
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
