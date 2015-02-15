using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public abstract class AbstractGameLogic 
{
	public void buildRoad(AbstractUnit u)
	{
		UnitType mytype = u.myType;
		if(mytype == UnitType.Peasant)
		{
			u.currentAction = ActionType.BuildingRoad;
		}
	}

	public abstract void upgradeVillage(AbstractVillage v, VillageType newType);
	public abstract void upgradeUnit(AbstractUnit u, UnitType newType);
	public abstract void moveUnit(AbstractUnit u, AbstractTile dest);
	public abstract void destroyVillage(AbstractVillage v, AbstractUnit invader);
	public abstract void divideRegion(IList<AbstractTile> region);
	public abstract void takeoverTile(AbstractTile dest);
	public abstract void beginTurn( AbstractPlayer p, AbstractGame g);
	public abstract void tombStonePhase( VillageType myVillage );
	public abstract void peasantBuild( Tile myTile );
	public abstract void incomePhase( VillageType myVillage );
	public abstract void paymentPhase( VillageType myVillage );
	public abstract void payVillagers( VillageType myVillage );
	public abstract void perishVillagers (VillageType myVillage );
	// TODO: the two private methods
}
