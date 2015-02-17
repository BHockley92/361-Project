﻿using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public class MW_Game : AbstractGame
{
	// Global stats tracked, not for each player
	private int roundsPlayed { get; set; }

	// for a randomly generated board, assumes length and width are > 0
	public MW_Game(List<AbstractPlayer> players, int boardLength, int boardWidth, AbstractGameLogic gl)
	{
		initialize (players, new Board (boardLength, boardWidth), gl);
		roundsPlayed = 0;
	}

	// Constructor for when a board is loaded
	public MW_Game(List<AbstractPlayer> players,  Tile[,] b, AbstractGameLogic gl)
	{
		initialize (players, new Board (b), gl);
		roundsPlayed = 0;
	}
}
