abstract class AbstractMedievalWarfare
{
	/*
		The newGame operation creates/initializes all data structures needed to 
		play a new game of Medieval Warfare between (at least) two players (identiﬁed 
		by the participants conceptual parameter). This includes creating a new game, 
		creating the tiles, and initializing the structure and topology of the battleﬁeld 
		(grass, meadow, sea, trees) either by generating the map or by loading a 
		preexisting one (identiﬁed by a parameter) and assigning initial regions to players. 
		It also positions the starting villages at some random positions within each region.
	*/
	abstract public void newGame(Dictionary<AbstractPlayer> participants);

	/*
		The beginTurn operation performs all game state updates that need to be done at the 
		start of player p’s turn. This includes replacing tombstones on tiles owned by p with 
		trees, producing meadows and roads, updating the gold stock of each village with the 
		income and paying the wages of the villagers, potentially replacing villagers by 
		tombstones if their wages cannot be paid.
	*/
	abstract public void beginTurn(AbstractGame g, AbstractPlayer p);

	abstract public void buildRoad(AbstractUnit u);

	abstract public void upgradeVillage(AbstractVillage v, VillageType newLevel);

	abstract public void upgradeUnit(AbstractUnit u, UnitType newLevel);

	/*
		The behaviour of the takeoverTile operation is to remove the dest tile from the region 
		of the enemy player, and add the dest tile to the region controlled by the village 
		that the capturing unit belongs to. It is assumed that the unit that is capturing the 
		tile is already on the dest tile (i.e., another operation has already checked that this 
		tile can be captured by this unit, and moved the unit to the dest tile). Depending on 
		the situation, one or several of the following game state updates are additionally performed:

		- if the dest tile has an enemy village on it, then it is invaded (i.e. the gold and wood 
		of the enemy village are added to the village of the capturing unit) and then destroyed. 
		If the enemy region is still big enough to support a village, a new hovel is placed at a 
		random place on the enemy region.

		- if capturing the dest tile split the enemy region into two or three unconnected regions, 
		then the region that has the enemy village on it loses all units that are not connected to 
		the village anymore. Furthermore, if any of the new regions is big enough to support a village, 
		a new enemy hovel is created on a random tile in that region, and the units that are located in 
		that region are assigned to the new region. Any units located on regions that are too small to 
		support a village are destroyed, and all tiles belonging to the region are converted to neutral 
		tiles.

		- if the enemy player has no regions left under his control, he is eliminated from the game 
		and his loss statistics are updated.

		- if the last enemy player is eliminated, the win statistics for the current player are updated. 
	*/
	abstract public void takeOverTile(AbstractTile destination);

	/*
		The behaviour of the moveUnit operation is to move a unit (identiﬁed by parameter u) to some 
		new position on the battleﬁeld (identiﬁed by the dest parameter). If the unit can not be moved 
		or if it is impossible for the unit to reach the destination tile, the game state is left 
		unchanged. If it is possible for u to reach dest (recall that units can only pass through tiles 
		that belong to the village they are supported by, and can not traverse sea or tree tiles, and 
		cannot invade enemy territory that is protected by a unit of equal or greater level), the current 
		location of the unit is set to dest. If a knight is moved, any meadow tiles without roads that 
		are used to get from the old to the new location are reverted to grass. Depending on the 
		destination tile, one or several of the following game state updates are performed:

	- if the dest tile has a tombstone, the tombstone is removed.

	- if the dest tile has a tree, the tree is removed and the wood reserve of the village is updated.
	
	- if the dest tile is neutral territory, it is added to the region controlled by the village. In 
	the case where the acquisition of this tile results in two regions of the same player touching each 
	other, then the regions are combined into one, and so are the two villages (the village of lower 
	level is removed, and its gold and wood reserve are added to the reserve of the other village, and 
	so is the control of its units).
	
	- if the dest tile is enemy territory, then the invadeTile operation is invoked.
	*/
	abstract public void moveUnit(AbstractUnit u, AbstractTile Destination);
}