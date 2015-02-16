using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public abstract class AbstractGameLogic 
{
	public ValueManager myValueManager { get; protected set; }

	public void buildRoad(AbstractUnit u)
	{
		UnitType mytype = u.myType;
		if(mytype == UnitType.Peasant)
		{
			u.currentAction = ActionType.BuildingRoad;
		}
	}

	public void upgradeVillage(AbstractVillage v, VillageType newType)
	{
		int gold = v.gold;
		int newValue = myValueManager.getVillageValue (newType);
		int oldValue = myValueManager.getVillageValue (v.myType);
		int upgradeValue = newValue - oldValue;

		// >= 0 because presumably higher levels cost more, so you don't want
		// to downgrade a unit
		if (upgradeValue <= gold && upgradeValue >= 0)
		{
			v.gold = gold - upgradeValue;
			v.myType = newType;
		}
	}

	public void upgradeUnit(AbstractUnit u, UnitType newType)
	{
		AbstractVillage unitVillage = u.myVillage;
		UnitType uType = u.myType;

		int vGold = unitVillage.gold;
		int upgradeValue = myValueManager.getUnitValue (newType);
		upgradeValue -= myValueManager.getUnitValue (u.myType);

		// >= 0 because presumably higher levels cost more, so you don't want
		// to downgrade a unit
		if(upgradeValue <= vGold && upgradeValue >= 0)
		{
			unitVillage.gold = vGold - upgradeValue;
			u.myType = newType;
		}
	}

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
