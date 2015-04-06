using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public class MW_Game : AbstractGame
{
	// Global stats tracked, not for each player
	public int roundsPlayed { get; set; }

	// for a randomly generated board, assumes length and width are > 0
	public MW_Game(List<AbstractPlayer> players, AbstractGameLogic gl)
	{
		initialize (players, gl);
		roundsPlayed = 0;
	}

	public void EndTurn() {
		nextTurn ();
		if(turnIndex == 0)
			roundsPlayed++;
	}
}
