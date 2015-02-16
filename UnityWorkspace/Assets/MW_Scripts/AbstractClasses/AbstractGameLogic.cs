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

	public abstract void moveUnit(AbstractUnit u, AbstractTile dest); // TODO
	public abstract void destroyVillage(AbstractVillage v, AbstractUnit invader);
	public abstract void divideRegion(IList<AbstractTile> region);
	public abstract void takeoverTile(AbstractTile dest);

	public void beginTurn( AbstractPlayer p, AbstractGame g)
	{
		List<AbstractVillage> myVillages = p.myVillages;

		foreach(AbstractVillage v in myVillages)
		{
			tombStonePhase(v);
			buildPhase(v);
			incomePhase(v);
			paymentPhase(v);
		}
	}

	public void tombStonePhase( AbstractVillage myVillage )
	{
		List<AbstractTile> controlledRegion = myVillage.controlledRegion;

		foreach(AbstractTile t in controlledRegion)
		{
			StructureType occupyingStructure = t.occupyingStructure.myType;

			if(occupyingStructure == StructureType.Tombstone)
			{
				t.occupyingStructure.myType = StructureType.NONE;
				t.myType = LandType.Tree;
			}
		}
	}

	public abstract void peasantBuild( AbstractTile myTile )
	{
		AbstractUnit occupyingUnit = myTile.occupyingUnit;
		UnitType myType = occupyingUnit.myType;
		ActionType currentAction = occupyingUnit.currentAction;

		if(myType == UnitType.Peasant && currentAction == ActionType.BuildingRoad)
		{
			myTile.occupyingStructure.myType = StructureType.Road;
			occupyingUnit.currentAction = ActionType.ReadyForOrders;
		}

		else if(myType == UnitType.Peasant && currentAction == ActionType.FinishCultivating)
		{
			myTile.myType = LandType.Meadow;
			occupyingUnit.currentAction = ActionType.ReadyForOrders;
		}

		else if(myType == UnitType.Peasant && currentAction == ActionType.StartCultivating)
		{
			occupyingUnit.currentAction = ActionType.FinishCultivating;
		}
	}

	public void buildPhase( AbstractVillage myVillage)
	{
		List<AbstractTile> controlledRegion = myVillage.controlledRegion;

		foreach(AbstractTile t in controlledRegion)
		{
			peasantBuild(t);
		}
	}
	public abstract void incomePhase( AbstractVillage myVillage );
	public abstract void paymentPhase( AbstractVillage myVillage );
	public abstract void payVillagers( AbstractVillage myVillage );
	public abstract void perishVillagers (VillageType myVillage );
	// TODO: the two private methods
}
