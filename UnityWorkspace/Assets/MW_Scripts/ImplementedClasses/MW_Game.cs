using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public class MW_Game : AbstractGame
{
	// for a randomly generated board, assumes length and width are > 0
	public MW_Game(List<AbstractPlayer> players, AbstractGameLogic gl)
	{
		initialize (players, gl);
	}

	public void EndTurn() {
		nextTurn ();
			
	}
}
