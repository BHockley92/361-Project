using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbstractGame
{
	private List<Player> participants;
	private AbstractPlayer turnOf { get; }

	private AbstractGameLogic myGameLogic { get; }
	private Tile[,] board { get; }
	// TODO: board representation -- use this: http://www.redblobgames.com/grids/hexagons/#map-storage

	// Global stats tracked, not for each player
	private int roundsPlayed = 0;
	private int unitsDiedFromCombat = 0;
	private int unitsDiedFromPoverty = 0;

}
