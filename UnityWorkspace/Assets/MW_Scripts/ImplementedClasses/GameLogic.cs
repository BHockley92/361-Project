using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public class GameLogic : AbstractGameLogic
{
	public override void buildRoad(AbstractUnit u)
	{
		UnitType mytype = u.myType;
		if(mytype == UnitType.Peasant)
		{
			u.currentAction = ActionType.BuildingRoad;
		}
	}
	
	public override void upgradeVillage(AbstractVillage v, VillageType newType)
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
	
	public override void upgradeUnit(AbstractUnit u, UnitType newType)
	{
		AbstractVillage unitVillage = u.myVillage;
		
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

	// TODO: this may be wrong. I followed the diagram as best as I could
	public override void moveUnit(AbstractUnit u, AbstractTile dest) 
	{
		AbstractTile unitLocation = u.myLocation;
		UnitType unitType = u.myType;
		AbstractPlayer player = u.getPlayer ();

		List<AbstractTile> unitTileNeighbours = unitLocation.getNeighbours ();
		List<AbstractTile> destTileNeighbours;

		LandType landtype = dest.myType;


		if(unitTileNeighbours.Contains(dest))
		{
			if( unitType != UnitType.Knight && landtype != LandType.Tree )
			{
				destTileNeighbours = dest.getNeighbours();
			}
			
			foreach( AbstractTile t in destTileNeighbours)
			{
				// TODO: things can throw null pointer exceptions here -- fix
				AbstractUnit neighbourUnit = t.occupyingUnit;
				AbstractVillage neighbourVillage = t.myVillage;
				VillageType neighbourVillageType = neighbourVillage.myType;
				AbstractPlayer neighbourPlayer = neighbourVillage.myPlayer;

				Structure neighbourStructure = t.occupyingStructure;
				UnitType neighbourUnitType = neighbourUnit.myType;

				int neighbourUnitValue = myValueManager.getUnitValue(neighbourUnitType);
				int unitValue = myValueManager.getUnitValue(unitType);

				// Fuck this check and everything about it
				if(neighbourPlayer != player &&
				   ( neighbourUnitValue > unitValue || unitValue < myValueManager.getUnitValue(UnitType.Soldier)
				 		&& (neighbourVillage != null || neighbourStructure == StructureType.Tower)
				   ) 
				   || 
				   (
						unitValue < myValueManager.getUnitValue(UnitType.Knight) 
						&& neighbourVillageType == VillageType.Fort
					)
				   ){ return; }
				// end crazy check
			}
			// TODO: i have no idea what's nested and what isn't. everything is interpreted
			// into code up until the last opt inclusively
			// gg.
		}
	}
	
	// TODO: unimplemented methods below, but not for the demo
	public override void destroyVillage(AbstractVillage v, AbstractUnit invader) {}
	public override void divideRegion(IList<AbstractTile> region) {}
	public override void takeoverTile(AbstractTile dest) {}
	protected override void connectRegions(List<AbstractVillage> v) {}
	
	public override void beginTurn( AbstractPlayer p, AbstractGame g)
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
	
	protected override void tombStonePhase( AbstractVillage myVillage )
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
	
	protected override void peasantBuild( AbstractTile myTile )
	{
		AbstractUnit occupyingUnit = myTile.occupyingUnit;
		UnitType myType = occupyingUnit.myType;
		ActionType currentAction = occupyingUnit.currentAction;
		
		if(myType == UnitType.Peasant)
		{
			if(currentAction == ActionType.BuildingRoad)
			{
				myTile.occupyingStructure.myType = StructureType.Road;
				occupyingUnit.currentAction = ActionType.ReadyForOrders;
			}
			
			else if(currentAction == ActionType.FinishCultivating)
			{
				myTile.myType = LandType.Meadow;
				occupyingUnit.currentAction = ActionType.ReadyForOrders;
			}
			
			else if(currentAction == ActionType.StartCultivating)
			{
				occupyingUnit.currentAction = ActionType.FinishCultivating;
			}
		}
	}
	
	protected override void buildPhase( AbstractVillage myVillage)
	{
		List<AbstractTile> controlledRegion = myVillage.controlledRegion;
		
		foreach(AbstractTile t in controlledRegion)
		{
			peasantBuild(t);
		}
	}
	
	protected override void incomePhase( AbstractVillage myVillage )
	{
		List<AbstractTile> controlledRegion = myVillage.controlledRegion;
		
		foreach(AbstractTile t in controlledRegion)
		{
			myVillage.gold += myValueManager.getLandValue(t.myType);
		}
	}
	
	protected override void paymentPhase( AbstractVillage myVillage )
	{
		List<AbstractUnit> supportedUnits = myVillage.supportedUnits;
		
		int totalCost = 0;
		
		foreach(AbstractUnit u in supportedUnits)
		{
			totalCost += myValueManager.getMaintenanceCost(u.myType);
		}
		
		if( myVillage.gold >= totalCost )
		{
			myVillage.gold -= totalCost;
		}
		
		// not enough money, EVERYONE DIES (that's associated to the village)
		else
		{
			myVillage.gold = 0;
			perishVillagers(myVillage);
		}
	}
	
	protected override void perishVillagers (AbstractVillage myVillage )
	{
		List<AbstractUnit> supportedUnits = myVillage.supportedUnits;
		
		foreach(AbstractUnit u in supportedUnits)
		{
			AbstractTile myLocation = u.myLocation;
			myLocation.occupyingStructure.myType = StructureType.Tombstone;
			
			// Remove the unit
			myLocation.occupyingUnit = null;
			myLocation = null;
			
		}
	}	
}
