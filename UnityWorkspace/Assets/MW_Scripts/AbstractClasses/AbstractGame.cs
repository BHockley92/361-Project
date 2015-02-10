using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AbstractGame
{
	List<AbstractPlayer> participants;
	private AbstractPlayer turnOf { get; set; }

	AbstractGameLogic myGameLogic { get; set; }
	AbstractTile[,] board { get; set; }
	// TODO: board representation -- use this: http://www.redblobgames.com/grids/hexagons/#map-storage

	// Global stats tracked, not for each player
	int roundsPlayed = 0;
	int unitsDiedFromCombat = 0;
	int unitsDiedFromPoverty = 0;

}
