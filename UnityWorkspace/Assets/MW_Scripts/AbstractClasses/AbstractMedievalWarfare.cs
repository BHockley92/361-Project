using UnityEngine;
using System.Collections.Generic;

public abstract class AbstractMedievalWarfare
{
	public AbstractGame currentGame { get; set; }
	public abstract void newGame( List<AbstractPlayer> participants);
	public abstract List<Tile> loadMaps(string myMapFile);
	public abstract void assignVillages(List<Tile> myGameTiles);
	public abstract void makeVillage(List<Tile> visitedTiles, Tile myTile, List<Player> participants);
}
