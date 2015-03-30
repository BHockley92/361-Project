using UnityEngine;
using System.Collections.Generic;
using GameEnums;



public class MedievalWarfare : AbstractMedievalWarfare
{
	public override MW_Game newGame(List<AbstractPlayer> participants)
	{
		// initalize game for randomly generated board
		GameLogic gl = new GameLogic (); 
		MW_Game myGame = new MW_Game (participants, gl);

		return myGame;
	}//end newGame

	public override void assignRegions(Board gameBoard)
	{
		//BFS algorithm
		foreach (Tile t in gameBoard.board) {
			if (t.myType != LandType.Sea && t.iteratedOver == false) { //attempt to fix error
				Stack<Tile> myStack = new Stack<Tile> ();
				List<AbstractTile> visitedTiles = new List<AbstractTile> ();
				AbstractPlayer belongsTo = t.myVillage.myPlayer; //current owner of tile t

				myStack.Push (t);

				//make list of tiles belonging to belongsTo player
				while (myStack.Count != 0) {
					Tile v = myStack.Pop ();
					visitedTiles.Add (v);

					if (v.isVisited == false) {
						v.isVisited = true; //mark tile as visited

						List<AbstractTile> neighbours = v.getNeighbours (); //Nick fixed to not return sea tiles
						//AbstractPlayer test1 = neighbours[0].myVillage.myPlayer;
						//AbstractPlayer test2 = neighbours[1].myVillage.myPlayer;

						//for all neighbours that havent been visited and have same owner
						foreach (Tile neighb in neighbours) {
							if(neighb.iteratedOver==false){
								if (neighb.myVillage.myPlayer == belongsTo && neighb.isVisited == false) {
									myStack.Push (neighb);
								}// end for each neighbour tile
							}
						}// end if tile is not visited
						

					} //end when stack is empty

					//NOTE TILES ARE FLAGGED AS "ITERATED-OVER" ONCE ASSIGNED A REGION OR NEUTRAL LAND
					//if region is greater than 3 && tile doesn't belong to official village, remove temp villages and pick random one to have village
					if (visitedTiles.Count >= 3 && t.hasVillage == false && t.myVillage != null) {
						foreach(Tile iteratedTile in visitedTiles){
							iteratedTile.iteratedOver = true;
						}

						int randInt = Random.Range (0, visitedTiles.Count);
						AbstractTile villageTile = visitedTiles [randInt];

						//Create the village
						GameObject village = (GameObject)Object.Instantiate (Resources.Load ("buildinghovel"));
						village.transform.position = new Vector3 (villageTile.boardPosition.x, 0.7f, villageTile.boardPosition.y - 0.5f);

						Village myNewVillage = new Village (visitedTiles, belongsTo);
						myNewVillage.location = villageTile;
						foreach (Tile w in visitedTiles) {
							w.hasVillage = true;
							w.myVillage = myNewVillage;
						}
					} 
					else if (visitedTiles.Count < 3) {
						//set land to neutral
						foreach (Tile n in visitedTiles) {
							n.myVillage = null;
							n.iteratedOver = true;
						}


					}

			}//end tile iteration
		} //skip if sea tile

	} 


}//end assignRegions

}