﻿using UnityEngine;
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

		// move can only happen if the destination is in the adjacent squares and isn't sea
		// also: The unit cannot move if they've already moved
		if( adjacentTiles.Contains(dest) && dest.myType != LandType.Sea 
		   && unitAction == ActionType.ReadyForOrders)
		{
			// Now check to make sure the tile isn't occupied.
			// Since units block all adjacent squares and die
			// when an enemy unit enters their guarded squares,
			// it should be impossible to move onto a tile with an enemy unit
			if(dest.occupyingUnit == null)
			{
				// Now check to see if adjacent enemy units/structures are blocking it
				List<AbstractTile>  destNeighbours = dest.getNeighbours();

				foreach(AbstractTile t in destNeighbours)
				{
					// an enemy unit on a tile has to belong
					// to an enemy
					if( t.myVillage != null )
					{
						// The tile belongs to an enemy
						if( t.myVillage.myPlayer != u.getPlayer() )
						{
							// Can't move it if there is a unit
							// of equal or better rank blocking us
							if( t.occupyingUnit != null)
							{
								if(uValue <= myValueManager.getUnitValue(t.occupyingUnit.myType))
									return;
							}

							// Can't invade tower controlled area without a better
							// unit than a knight
							if( t.occupyingStructure != null)
							{
								if( uValue <= soldierValue 
								   && t.occupyingStructure.myType == StructureType.Tower)
									return;
							}
						}
					}
				}

				// Ok, so now we know that the destination is not being
				// blocked by enemies and is adjacent to the unit's location

				// knights can't walk through forests
				if( dest.myType == LandType.Tree && u.myType == UnitType.Knight) return;

				// If it's within the moving unit's territory or neutral territory
				// we can move
				if( dest.myVillage == u.myVillage || dest.myVillage == null)
				{
					actuallyMoveTheUnit( u, dest );

					// if it's neutral territory, take it
					if( dest.myVillage == null )
					{
						dest.myVillage = u.myVillage;
						u.myVillage.controlledRegion.Add(dest);

						// assuming the method call didn't make it cultivate a meadow or something,
						// consider the unit moved
						if( u.currentAction == ActionType.ReadyForOrders)
							u.currentAction = ActionType.Moved;
					}

					// TODO: check if there are any adjacent units that die from space invasion
						// DONT CARE FOR DEMO LOLOL
				}

				// Otherwise we're invading enemy territory
				// Only soldiers and better can invade
				else if ( uValue >= soldierValue );
				{
					// TODO: this is not for the demo. So ... LOLOL DONT CARE
					// actuallyMoveTheUnit(u, dest);
					// Do the invasion stuff
					// Check if domination links two villages
					// Check if adjacent units/watchtowers die of space invasion
					// Make sure action type is set correctly ( tombstone + invade? forest + invade with soldier? )
					// Make sure that watchtowers and units cut off from their home villages die or get reassigned
				}
			}
		}
	}

	// Assumes all checks have been done and we can actually move the unit
	// Does not manage any territories
	// Only takes care of movement, trampling fields, orders and tombstones
	// Should be done BEFORE territories are manages or order management WON'T WORK
	private void actuallyMoveTheUnit( AbstractUnit u, AbstractTile dest)
	{
		// move the unit onto the destination
		u.myLocation.occupyingUnit = null;
		u.myLocation = dest;
		dest.occupyingUnit = u;

		// Trample the meadow if soldier or knight
		if( dest.myType == LandType.Meadow &&
		   ( u.myType == UnitType.Soldier || u.myType == UnitType.Knight))
		{
			dest.myType = LandType.Grass;
		}

		// Only knights won't clear tombstones, so if there is one
			// and the unit isn't a knight, clear it.
		if( dest.occupyingStructure.myType == StructureType.Tombstone
		   && u.myType != UnitType.Knight)
		{
			u.currentAction = ActionType.ClearingTombStone;
		}

		// Only knights won't chop wood, so if there's a forest,
			// clear it if the unit isn't a knight
		if( dest.myType == LandType.Tree && u.myType != UnitType.Knight)
		{
			u.currentAction = ActionType.ChoppingTree;
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
