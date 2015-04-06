using UnityEngine;
using System.Collections.Generic;
using GameEnums;



public class MedievalWarfare : AbstractMedievalWarfare
{
	public override MW_Game newGame(List<AbstractPlayer> participants)
	{
		// initalize game for randomly generated board
		GameLogic gl = new GameLogic (); 
		ValueManager vm = new ValueManager (); //must initalize value manager for game
		gl.myValueManager = vm;
		MW_Game myGame = new MW_Game (participants, gl);
		// instantiate players list of villages to 0;
		foreach (MW_Player p in participants) {
			p.myVillages = new List<AbstractVillage>();
		}
		return myGame;
	}//end newGame

	public override void assignRegions(Board gameBoard)
	{
		//BFS algorithm
		foreach (Tile t in gameBoard.board) {
			if (t.myType != LandType.Sea && t.myVillage !=null) { //tile village = null if neutral 
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

						//for all neighbours that havent been visited and have same owner
						foreach (Tile neighb in neighbours) {
							if(neighb.myVillage != null){ //skip neutral land
								if (neighb.myVillage.myPlayer == belongsTo && neighb.isVisited == false) {
									myStack.Push (neighb);
								}// end for each neighbour tile
							}
						}
					}// end if tile is not visited
						

				} //end when stack is empty

					//if region is greater than 3 && tile doesn't belong to official village, remove temp villages and pick random one to have village
					if (visitedTiles.Count >= 3 && t.hasVillage == false) {
						int randInt = Random.Range (0, visitedTiles.Count);
						Village myNewVillage = new Village (visitedTiles, belongsTo);
						myNewVillage.location = visitedTiles [randInt];
						 
						foreach (Tile w in visitedTiles) {
							//all these tiles are controlled by myNewVillage
							w.hasVillage = true;
							w.myVillage = myNewVillage;
						}
					t.myVillage = myNewVillage;
					belongsTo.myVillages.Add(myNewVillage);
	
					} 
					else if (visitedTiles.Count < 3 && t.hasVillage == false) {
						//set land to neutral
						foreach (Tile n in visitedTiles) {
							n.myVillage = null;
						}
					}


				} //skip if see or already visited

			}//end tile iteration

}//end assignRegions

}