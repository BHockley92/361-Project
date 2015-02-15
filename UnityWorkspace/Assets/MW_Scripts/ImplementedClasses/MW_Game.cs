using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public class MW_Game : AbstractGame
{
	// Global stats tracked, not for each player
	private int roundsPlayed = 0;
	private int unitsDiedFromCombat = 0;
	private int unitsDiedFromPoverty = 0;

	// for a randomly generated board, assumes length and width are > 0
	public MW_Game(List<AbstractPlayer> players, int boardLength, int boardWidth, AbstractGameLogic gl)
	{
		initialize (players, new Board (boardLength, boardWidth), gl);
	}

	// Constructor for when a board is loaded
	public MW_Game(List<AbstractPlayer> players,  Tile[,] b, AbstractGameLogic gl)
	{
		initialize (players, new Board (b), gl);
	}
}
