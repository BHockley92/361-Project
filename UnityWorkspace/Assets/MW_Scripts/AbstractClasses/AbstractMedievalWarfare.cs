using UnityEngine;
using System.Collections.Generic;

public abstract class AbstractMedievalWarfare
{
	public AbstractGame currentGame { get; set; }
	public abstract MW_Game newGame( List<AbstractPlayer> participants);
	//public abstract List<Tile> loadMaps(string myMapFile);
	public abstract void assignRegions(Board gameBoard);
}
