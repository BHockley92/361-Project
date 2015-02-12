using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public class MW_Game : AbstractGame
{
	private IList<AbstractPlayer> participants;
	public AbstractPlayer turnOf { get; private set; }
	
	public AbstractGameLogic myGameLogic { get; private set; }

	public Board gameBoard { get; private set; }
	
	// Global stats tracked, not for each player
	private int roundsPlayed = 0;
	private int unitsDiedFromCombat = 0;
	private int unitsDiedFromPoverty = 0;

	// for a randomly generated board, assumes length and width are > 0
	public MW_Game(IList<AbstractPlayer> players, int qboardLength, int rboardWidth, AbstractGameLogic gl)
	{
		participants = players;
		turnOf = participants [0];

		gameBoard = new Board (qboardLength, rboardWidth);
		myGameLogic = gl;
	}

	// Constructor for when a board is loaded
	public MW_Game(IList<AbstractPlayer> players,  Tile[,] b, AbstractGameLogic gl)
	{
		participants = players;
		turnOf = players [0];

		gameBoard = new Board(b);
		myGameLogic = gl;
	}
}
