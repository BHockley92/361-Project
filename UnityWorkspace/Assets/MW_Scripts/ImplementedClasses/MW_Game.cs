using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public class MW_Game : AbstractGame
{
	// Global stats tracked, not for each player
	private int roundsPlayed { get; set; }

	// for a randomly generated board, assumes length and width are > 0
	public MW_Game(List<AbstractPlayer> players, int boardLength, int boardWidth, int waterborder, AbstractGameLogic gl)
	{
		initialize (players, new Board (boardLength, boardWidth, waterborder), gl);
		roundsPlayed = 0;
	}
}
