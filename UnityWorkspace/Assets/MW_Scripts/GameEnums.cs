using UnityEngine;
using System.Collections;

namespace GameEnums
{
	enum PlayerStatus { Offline, Online };
	enum LandType { Sea, Grass, Tree, Meadow };
	enum UnitType { Peasant, Infantry, Soldier, Knight };
	enum ActionType { ReadyForOrders, Moved, BuildingRoad, 
		ChoppingTree, ClearingTombStone, UpgradingCombining, 
		StartCultivating, FinishCultivating };

	enum StructureType { NONE, Tombstone, Road, Tower };
}
