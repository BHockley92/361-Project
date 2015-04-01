using UnityEngine;
using System.Collections.Generic;
using GameEnums;
using System.IO;

public class GameLogic : AbstractGameLogic
{
	public override void destroyVillage(AbstractVillage v, AbstractUnit invader){
		int myGold = v.gold;
		int myWood = v.wood;
		int invaderGold = invader.myVillage.gold;
		int invaderWood = invader.myVillage.wood;
		//invader takes villages gold
		invader.myVillage.gold = invaderGold + myGold;
		invader.myVillage.wood = invaderWood + myWood;
		//invader takes control of tile, no longer part of v's controlledRegion
		v.swapControl (v.location, invader.myVillage);

		//move village to new tile if controlled region is greater than 3
		if (v.controlledRegion.Count >= 3) {
			int randomTile = Random.Range (0, v.controlledRegion.Count); 
			//change location of village
			v.location = v.controlledRegion [randomTile];
			//empty villages resources, empty hovel recreated
			v.wood = 0;
			v.myType = VillageType.Hovel;
			v.gold = 0;
			
		}
		else{
			//controlled region isnt big enough, destroy village and units, make rest of tiles neutral land
			AbstractTile myTile = v.location;
			foreach(Unit u in v.supportedUnits){
				u.myLocation.occupyingUnit = null;
			}
			foreach(Tile t in v.controlledRegion){
				t.myVillage = null; //neutral land
			}
			v = null; //destroy village
			//village tile turns to a tree
			myTile.myType = LandType.Tree;

		}
		
		
	
	}
//TODO: Emily
	public override void divideRegion(List<AbstractTile> region) {



		//if tile split region into unconnected regions
		//region that has village loses all units NOT connected anymore (ie: units in other subregions)
		// if subregion >=3, new hovel on random tile and occupyingunits on those tiles part of supportedUnits of new village
		//subregions <3, all occupying units destroyed and subregion tiles become neutral



	}






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

	// returns true upon successful build
	public override bool buildTower(AbstractTile t)
	{
		if( t.myVillage != null)
		{
			if( t.myVillage.wood >= 5 && (int) t.myVillage.myType >= (int) VillageType.Town)
			{
				t.myVillage.wood -= 5;

				if( t.occupyingStructure == null )
					t.occupyingStructure = new Structure( t, StructureType.Tower );
				else
					t.occupyingStructure.myType = StructureType.Tower;

				return true;
			}
		}
		return false;
	}

	public override bool buildCannon(AbstractTile t, AbstractVillage owner)
	{
		if( t.myVillage == owner )
		{
			if( (int) owner.myType >= (int) VillageType.Fort && t.getNeighbours().Contains(owner.location))
			{
				if(t.occupyingUnit == null && owner.gold >= 35 && owner.wood >= 12)
				{
					owner.gold -= 35;
					owner.wood -= 12;

					Unit cannon = new Unit( owner, t );
					cannon.isCannon = true;
					cannon.myType = UnitType.Soldier;
					t.occupyingUnit = cannon;

					owner.supportedUnits.Add(cannon);
					return true;
				}
			}
		}
		return false;
	}
	
	public override bool isVillageStillAlive(AbstractVillage v)
	{
		if(v.myType == VillageType.Hovel && v.damageTaken >= 1)
			return false;
		if(v.myType == VillageType.Town && v.damageTaken >= 2)
			return false;
		if(v.myType == VillageType.Fort && v.damageTaken >= 5)
			return false;
		if(v.myType == VillageType.Castle && v.damageTaken >= 10)
			return false;

		return true;
	}

	// returns true upon successful upgrade
	public override bool upgradeVillage(AbstractVillage v, VillageType newType)
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
			//Update the GUI
			foreach(GUIText current in GameObject.FindObjectsOfType<GUIText>()) {
				if(current.name == "Gold") {
					current.text = "Gold: " + v.gold;
				}
			}
			v.myType = newType;
			return true;
		}
		return false;
	}

	// returns true upon successful upgrade
	public override bool upgradeUnit(AbstractUnit u, UnitType newType)
	{
		AbstractVillage unitVillage = u.myVillage;
		VillageType UVType = unitVillage.myType;

		// make sure the village can support the upgrade
		if( (int) newType < (int) UnitType.Soldier
		   || (int) UVType >= (int) VillageType.Fort
		   || (UVType == VillageType.Town && newType == UnitType.Soldier))
		{
			int vGold = unitVillage.gold;
			int upgradeValue = myValueManager.getUnitValue (newType);
			upgradeValue -= myValueManager.getUnitValue (u.myType);
			
			// >= 0 because presumably higher levels cost more, so you don't want
			// to downgrade a unit
			if(upgradeValue <= vGold && upgradeValue >= 0)
			{
				unitVillage.gold = vGold - upgradeValue;
				u.myType = newType;
				return true;
			}
		}
		return false;
	}

	// returns true upon success
	public override bool combineUnits (AbstractUnit upgrader, AbstractUnit sacrificed)
	{
		if( upgrader.myType != UnitType.Knight && upgrader.myType == sacrificed.myType)
		{
			VillageType vType = upgrader.myVillage.myType;
			int nextRank = (int)upgrader.myType + 1;

			// make sure the village can support the upgrade
			if( nextRank < (int) UnitType.Soldier
			   || (int) vType >= (int) VillageType.Fort
			   || (vType == VillageType.Town && (UnitType) nextRank == UnitType.Soldier))
			{
				// check that the two units are adjacent
				if( upgrader.myLocation.getNeighbours().Contains( sacrificed.myLocation ))
				{
					sacrificed.myVillage.supportedUnits.Remove( sacrificed );
					sacrificed.myLocation.occupyingUnit = null;
					sacrificed.myLocation = null;

					upgrader.currentAction = ActionType.UpgradingCombining;
				}
			}
		}
		return false;
	}

	// returns true upon successful movement of unit
	public override bool moveUnit(AbstractUnit u, AbstractTile dest) 
	{
		AbstractTile unitLocation = u.myLocation;

		UnitType unitType = u.myType;
		AbstractPlayer player = u.getPlayer ();
		List<AbstractTile> unitTileNeighbours = unitLocation.getNeighbours ();
		LandType landtype = dest.myType;

		if( unitTileNeighbours.Contains(dest) && u.currentAction == ActionType.ReadyForOrders)
		{
			if( 
			   (unitType == UnitType.Knight && landtype != LandType.Tree)
				 || unitType != UnitType.Knight
			   )
			{
				List<AbstractTile> destNeighbours = dest.getNeighbours();

				foreach( AbstractTile destNeighbour in destNeighbours )
				{
					AbstractUnit neighbourUnit = destNeighbour.occupyingUnit;

					AbstractVillage neighbourVillage = destNeighbour.myVillage;
					VillageType neighbourVillageType = neighbourVillage.myType;
					AbstractPlayer neighbourPlayer = neighbourVillage.myPlayer;

					AbstractStructure neighbourStructure = destNeighbour.occupyingStructure;
					UnitType neighbourUnitType = neighbourUnit.myType;

					if( neighbourPlayer != player && 
					   	( (int) neighbourUnitType > (int) unitType ||
					 		( (int) unitType < (int) UnitType.Soldier && 
					 			( neighbourVillage != null || neighbourStructure.myType == StructureType.Tower)
					 		) ||
					 		( (int) unitType < (int) UnitType.Knight && neighbourVillageType == VillageType.Fort )
					 	)
					  )
					{
						return false;
					}
				}

				// move unit part 2
				// All systems go, move is allowed

				// kill enemy units/structures
				foreach( AbstractTile t in dest.getNeighbours())
				{
					if( t.myVillage != null)
					{
						AbstractPlayer p = t.myVillage.myPlayer;
						if( p != player )
						{
							AbstractStructure s = t.occupyingStructure;
							AbstractUnit unit = t.occupyingUnit;

							if(s != null)
							{
								if( s.myType == StructureType.Tower )
									s.myType = StructureType.NONE;
							}

							if( unit != null )
							{
								unit.myLocation.occupyingUnit = null;
								unit.myVillage.supportedUnits.Remove(unit);
								unit.myLocation = null;

								t.occupyingStructure = new Structure( t, StructureType.Tombstone);
							}
						}
					}
				}

				StructureType destStructureType = StructureType.NONE;
				if( dest.occupyingStructure != null )
					destStructureType = dest.occupyingStructure.myType;

				AbstractPlayer destPlayer = null;
				if( dest.myVillage == null )
				{
					connectRegions( player.myVillages );
					u.currentAction = ActionType.Moved;
				}
				else destPlayer = dest.myVillage.myPlayer;

				u.myLocation.occupyingUnit = null;
				u.myLocation = dest;

				if( destPlayer != player && destPlayer != null)
				{
					takeoverTile( dest );
					connectRegions( player.myVillages );
					u.currentAction = ActionType.Moved;
				}

				if( destPlayer == player )
				{
					u.currentAction = ActionType.ReadyForOrders;
				}

				if( (int) u.myType < (int) UnitType.Knight && dest.myType == LandType.Tree )
					u.currentAction = ActionType.ChoppingTree;

				if( (int) u.myType < (int) UnitType.Knight && destStructureType == StructureType.Tombstone)
					u.currentAction = ActionType.ClearingTombStone;

				if( dest.myType == LandType.Meadow && (int) u.myType >= (int) UnitType.Soldier )
					dest.myType = LandType.Grass;

				return true;
			}
		}
		return false;
	}



	public override void takeoverTile(AbstractTile dest) 
	{
		AbstractUnit attackingUnit = dest.occupyingUnit;
		AbstractVillage attackingVillage = attackingUnit.myVillage;
		AbstractVillage occupyingVillage = dest.myVillage;
		//vilage resides on this tile "dest"
		if (occupyingVillage != null) {
				destroyVillage (occupyingVillage, attackingUnit);
		}
		//village is not on this tile, swap control, see if region is split
		else{
			//swap control of tile, now belongs to attacking unit's village
			AbstractVillage myVillage = dest.myVillage;
			myVillage.swapControl (dest, attackingVillage);

			List<AbstractTile> controlledRegion = myVillage.controlledRegion;
			divideRegion (controlledRegion);
		}
	}

	protected override void connectRegions(List<AbstractVillage> villages) 
	{
		// randomly choose a village to keep
		AbstractVillage chosenVillage = villages [Random.Range (0, villages.Count + 1)];

		foreach( AbstractVillage v in villages )
		{
			if( v == chosenVillage ) continue;

			// Transfer gold, wood, units, tiles to remaining village
			chosenVillage.gold += v.gold;
			v.gold = 0;

			chosenVillage.wood += v.wood;
			v.wood = 0;

			foreach( AbstractUnit u in v.supportedUnits)
			{
				chosenVillage.supportedUnits.Add(u);
			}
			v.supportedUnits.Clear();

			v.location = null;

			foreach( AbstractTile t in v.controlledRegion)
			{
				chosenVillage.controlledRegion.Add(t);
				t.myVillage = chosenVillage;
			}
			v.controlledRegion.Clear();
		}
	}
	
	public override void beginTurn( AbstractPlayer p, AbstractGame g)
	{
		// If it's a new round, tree growth
		if( g.turnIndex == 0 ) treeGrowthPhase( g.gameBoard );

		List<AbstractVillage> myVillages = p.myVillages;
		
		foreach(AbstractVillage v in myVillages)
		{
			tombStonePhase(v);
			buildPhase(v);
			incomePhase(v);
			paymentPhase(v);
			readyForOrders(v);
		}
	}

	// Finishes combining units
	private void readyForOrders(AbstractVillage v)
	{
		foreach(AbstractUnit u in v.supportedUnits)
		{
			if( u.currentAction == ActionType.Moved)
			{
				u.currentAction = ActionType.ReadyForOrders;
			}
			else if( u.currentAction == ActionType.UpgradingCombining )
			{
				u.currentAction = ActionType.ReadyForOrders;
				if( u.myType != UnitType.Knight )
					u.myType = (UnitType)( (int) u.myType + 1);
			}
		}
	}

	protected override void treeGrowthPhase( Board b )
	{
		// iterate over each tile in the board
		for(int i = 0; i < b.board.GetLength(0); i++)
		{
			for(int j = 0; j < b.board.GetLength(1); j++)
			{
				// check if the tile is a tree
				AbstractTile t = b.board[i, j];
				if( t.myType == LandType.Tree )
				{
					spawnTreesInNeighbours( t );
				}
			}
		}
	}

	// Goes through each neighbour of the tile and if they are eligible 
	// for tree growth, a dice is rolled to determine if it actually happens
	private void spawnTreesInNeighbours( AbstractTile t)
	{
		List<AbstractTile> neighbours = t.getNeighbours();
		
		// If the tile neighbour is elegible for tree growth, roll
		foreach( AbstractTile neighbour in neighbours )
		{
			if( neighbour.occupyingUnit == null 
			   && neighbour.occupyingStructure.myType == StructureType.NONE
			   && neighbour.myVillage.location != neighbour 
			   && neighbour.myType != LandType.Sea )
			{
				int diceRoll = Random.Range(0, 2);
				
				if( diceRoll == 1 )
				{
					neighbour.myType = LandType.Tree;
				}
			}
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
		
		int totalGoldCost = 0;

		foreach(AbstractUnit u in supportedUnits)
		{
			if( !u.isCannon)
				totalGoldCost += myValueManager.getMaintenanceCost(u.myType);
			else
				totalGoldCost += 5;

		}

		if( myVillage.myType == VillageType.Castle )
			totalGoldCost += 80;
		
		if( myVillage.gold >= totalGoldCost )
		{
			myVillage.gold -= totalGoldCost;
		}
		
		// not enough money, EVERYONE DIES (that's associated to the village)
		else
		{
			myVillage.gold = 0;
			perishVillagers(myVillage);

			if( myVillage.myType == VillageType.Castle )
				myVillage.myType = VillageType.Fort;
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
