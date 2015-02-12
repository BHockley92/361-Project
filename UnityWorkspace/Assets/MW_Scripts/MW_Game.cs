using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public class MW_Game : AbstractGame
{
	private IList<AbstractPlayer> participants;
	public AbstractPlayer turnOf { get; private set; }
	
	public AbstractGameLogic myGameLogic { get; private set; }

	// TODO: board representation -- use this: http://www.redblobgames.com/grids/hexagons/#map-storage
	// Axial coordinate system will be used
	public AbstractTile[,] board { get; private set; }
	
	// Global stats tracked, not for each player
	private int roundsPlayed = 0;
	private int unitsDiedFromCombat = 0;
	private int unitsDiedFromPoverty = 0;

	// for a randomly generated board, assumes length and width are > 0
	public MW_Game(IList<AbstractPlayer> players, int boardLength, int boardWidth)
	{
		participants = players;
		board = generateRandomBoard (boardLength, boardWidth);
	}

	// Constructor for when a board is loaded
	public MW_Game(IList<AbstractPlayer> players, AbstractTile[,] b)
	{
		participants = players;
		turnOf = players [0];
		board = b;
	}

	// Generates a random island that conforms to specs (300 land tiles that aren't water, one big land mass)
	private AbstractTile[,] generateRandomBoard(int boardLength, int boardWidth)
	{
		// TODO
		return null;
	}

	// get the neighbours of a hex tile
	// list is ordered from the upper left neighbor and moving clockwise
	public List<Tile> getNeighbors(Coordinate c)
	{
		// TODO: check that indices are valid -- don't forget that not all indices in the array are used!
		List<Tile> ret = new List<Tile> ();
		ret.Add (board [c.q, c.r - 1]);
		ret.Add (board [c.q + 1, c.r - 1 ]);
		ret.Add (board [c.q + 1, c.r ]);
		ret.Add (board [c.q, c.r + 1]);
		ret.Add (board [c.q - 1, c.r + 1 ]);
		ret.Add (board [c.q - 1, c.r]);
	}
}
