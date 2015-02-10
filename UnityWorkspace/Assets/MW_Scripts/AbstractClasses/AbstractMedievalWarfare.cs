using UnityEngine;
using System.Collections.Generic;

public class MedievalWarfare
{
	public AbstractGame currentGame { get; }
	public void newGame( IList<Player> participants);
}
