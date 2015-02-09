using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game
{
	private List<Player> participants;
	private Player turnOf { get; }

	private GameLogic myGameLogic { get; }

	// Global stats tracked, not for each player
	private int roundsPlayed = 0;
	private int unitsDiedFromCombat = 0;
	private int unitsDiedFromPoverty = 0;

	// TODO: board representation -- use this: http://www.redblobgames.com/grids/hexagons/#map-storage
		// PROBLEM: specs don't specify dimensions of the board, it just says the island uses 300 tiles
}
