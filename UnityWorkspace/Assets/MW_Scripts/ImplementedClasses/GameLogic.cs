using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public class GameLogic : AbstractGameLogic
{
	public override void hireVillager(AbstractUnit u, AbstractVillage commandingVillage, AbstractTile spawnedTile) 
	{
		int unitcost = myValueManager.getUnitValue (u.myType);
		// Check to make sure the village can afford it
		if( commandingVillage.gold >= unitcost )
		{
			// Check to make sure the tile the villager will be spawned on is controlled by the
			// commanding village
			if(spawnedTile.myVillage == commandingVillage)
			{
				// Check to make sure there is not unit where the
				// villager is being spawned
				if( spawnedTile.occupyingUnit == null )
				{
					// Now check to make sure there are no enemy units blocking it
					// from being spawned
					List<AbstractTile> neighbourTiles = spawnedTile.getNeighbours();
					foreach(AbstractTile t in neighbourTiles)
					{
						// If the adjacent tiles aren't neutral
						if ( t.myVillage != null)
						{
							// and they belong to another player
							if( t.myVillage.myPlayer != commandingVillage.myPlayer)
							{
								// If there is a unit or watchtower there, you can't
								// spawn the player here
								if(t.occupyingUnit != null || t.occupyingStructure.myType == StructureType.Tower)
									return;
							}
						}
					}

					// If we made it past all of these checks, we can spawn the unit

					// first charge the village for it
					commandingVillage.gold -= unitcost;

					// now add it to supported units and spawn it
					commandingVillage.supportedUnits.Add(u);
					u.myVillage = commandingVillage;
					u.myLocation = spawnedTile;
					spawnedTile.occupyingUnit = u;
				}
			}
		}
	}


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
	// Only the first nested if statement is relevant to the demo ( if destVillage == null )
	public override void moveUnit(AbstractUnit u, AbstractTile dest) 
	{
		// Only look at adjacent tiles. No multitile movement here

		// Moving into a neutral tile ends movement
		// If a villager walks over a tombstone that isn't a knight, it is cleared and the unit can no longer move
		// If a non-knight unit moves on a tree square it is vacated and one wood is added to the stokpile
		// Villagers should be able to cultivate meadows AFTER moving, same goes for building a road
		// Knights cannot move through forests

		// Need to take into account: expanding territory joins villages
		// Only infantry rank and higher can move into enemy territory
		// watchtower == infantry level

		List<AbstractTile> adjacentTiles = u.myLocation.getNeighbours ();
		ActionType unitAction = u.currentAction;

		int uValue = myValueManager.getUnitValue(u.myType);
		int soldierValue = myValueManager.getUnitValue(UnitType.Soldier);
		int knightValue = myValueManager.getUnitValue (UnitType.Knight);
		
		// move can only happen if the destination is in the adjacent squares and isn't sea
		// also: The unit cannot move if they've already moved
		if( adjacentTiles.Contains(dest) && dest.myType != LandType.Sea 
		   && unitAction == ActionType.ReadyForOrders)
		{
			AbstractVillage destVillage = dest.myVillage;

			// the destination neutral territory, therefore there is no unit on it
			// THIS IS THE ONLY DEMO RELEVANT PART
			if(destVillage == null)
			{
				// TODO: refactor this part into its own method

				// knights can't go into forests
				if( dest.myType == LandType.Tree && u.myType == UnitType.Knight ) return;

				// move the unit
				u.myLocation.occupyingUnit = null;
				u.myLocation = dest;

				// Take over the territory
				dest.myVillage = u.myVillage;
				u.myVillage.controlledRegion.Add(dest);

				// Neutral territory entered: the unit can no longer receive orders
				u.currentAction = ActionType.Moved;

				// If infantry or knight, trample the meadow
				if(u.myType == UnitType.Soldier || u.myType == UnitType.Knight && dest.myType == LandType.Meadow)
				{
					dest.myType = LandType.Grass;
				}

				// If there's a tombstone or forest on the tile, clear it
				// < knightvalue because the knights do not care about clearing tombstones
				if(uValue < knightValue && dest.occupyingStructure.myType == StructureType.Tombstone)
				{
					u.currentAction = ActionType.ClearingTombStone;
				}


				else if( dest.myType == LandType.Tree )
				{
					u.currentAction = ActionType.ChoppingTree;
				}
			}

			// The tile belongs to someone and might have a unit on it
			else
			{
				// Get a bunch of values which will be used
				AbstractUnit destUnit = dest.occupyingUnit;
				AbstractPlayer destPlayer = destVillage.myPlayer;
				AbstractPlayer player = u.getPlayer();

				int destUnitValue = -1;

				if(destUnit != null)
				{
					destUnitValue = myValueManager.getUnitValue(destUnit.myType);
				}

				// Now who does the territory belong to?

				// Destination is within your own territory
				if(player == destPlayer)
				{
					// You can't invade your own units, do nothing
					if( destUnitValue >= 0) return;

					// There is no unit in the destination tile
					else
					{
						// TODO check knight v forest, look for tombstone, etc -- should probably be its own method
					}
				}

				// It's enemy territory
				else
				{
					// TODO: not relevant for the demo
				}
			}
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
