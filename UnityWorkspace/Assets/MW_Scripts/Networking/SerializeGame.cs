using UnityEngine;
using System.Xml;
using System.Collections.Generic;
using GameEnums;
using System;
using System.IO;
/************ XML TEMPLATE (EDITED)********
<map length = "" width = "", waterBorder ="">

	<tile landType = "" boardPositionX = "" boardPositionY = "" gamePositionX = "" gamePositionY="">
    		<structure structureType = ""/> //structure only if not sea
    </tile>

	<village playerName= "" gold= "" wood="" villageType="" locationOfTileX="" locationOfTileY="" damageTaken="">
			<controlledTile boardX="" boardY=""/>
			<controlledTile boardX="" boardY=""/>
			<controlledTile boardX="" boardY=""/>
	</village>

	<unit unitType="" unitAction="" unitX = "" unitY="" villageOwnerX="" villageOwnerY="" isCannon=""/>

	<game turnIndex="" turnOf="">
		<participant username=""/>
		<participant username=""/>
	</game>


</map>

 */


public class SerializeGame 
{
	string fromWhere = "";
	public MW_Game loadGame(XmlDocument doc){
		
		//<map> attributes
		string length = doc.DocumentElement.Attributes["length"].InnerText;
		string width = doc.DocumentElement.Attributes["width"].InnerText;
		string waterBorder = doc.DocumentElement.Attributes["waterBorder"].InnerText;
		
		Tile[,] myTiles = new Tile[Convert.ToInt32(length)+1, Convert.ToInt32(width)+1];
		Board myGameBoard = new Board(myTiles, Convert.ToInt32(waterBorder)); 
		GameLogic gl = new GameLogic ();
		List<AbstractPlayer> gamePlayers = new List<AbstractPlayer> ();
		string turnIndex = "";
		string turnOf = "";

		foreach (XmlNode node in doc.DocumentElement.ChildNodes) {
			if (node.Name == "game") {
				//create participant list
				foreach(XmlNode gameChild in node.ChildNodes){
					string username = gameChild.Attributes["username"].InnerText;
					MW_Player myPlayer = new MW_Player();
					myPlayer.username = username;
					myPlayer.myVillages = new List<AbstractVillage>();
					gamePlayers.Add(myPlayer);
				}

				turnIndex = node.Attributes ["turnIndex"].InnerText;
				turnOf = node.Attributes ["turnOf"].InnerText;

			} 
		}
		//make new instance of MW_Game from XML specs
		MW_Game myGame = new MW_Game(gamePlayers,gl);
		ValueManager vm = new ValueManager (); //must initalize value manager for game
		gl.myValueManager = vm;
		foreach (AbstractPlayer p in myGame.participants) {
			if (turnOf == p.username) {
				myGame.turnOf = p;
				break;
			}
		}
		myGame.gameBoard = myGameBoard;
		myGame.turnIndex = Convert.ToInt32 (turnIndex);

		// update gameBoard
		foreach (XmlNode node in doc.DocumentElement.ChildNodes) {
							
			if (node.Name == "tile") {
				string landType = node.Attributes ["landType"].InnerText;
				string boardPositionX = node.Attributes ["boardPositionX"].InnerText;
				string boardPositionY = node.Attributes ["boardPositionY"].InnerText;
				string gamePositionX = node.Attributes ["gamePositionX"].InnerText;
				string gamePositionY = node.Attributes ["gamePositionY"].InnerText;
				
				//make new tile
				LandType lType = (LandType)Enum.Parse (typeof(LandType), landType, true);
				Tile myTile = new Tile (lType, myGameBoard, Convert.ToInt32 (boardPositionX), Convert.ToInt32 (boardPositionY));
				Vector2 gamePos = new Vector2 (Convert.ToSingle (gamePositionX), Convert.ToSingle (gamePositionY)); //needs to be a float aka "single"
				myTile.gamePosition = gamePos;
				
				if (landType != "Sea") {
					
					
					//skip this if tile is a sea tile, else iterate over structure
					foreach (XmlNode tileChild in node.ChildNodes) { //should only be one tileChild - structure
						
						if (tileChild.Name == "structure") {
							string structureType = tileChild.Attributes ["structureType"].InnerText;
							//convert string to enum
							StructureType strucType = (StructureType)Enum.Parse (typeof(StructureType), structureType, true);
							Structure myStruc = new Structure (myTile, strucType);
							myTile.occupyingStructure = myStruc;
						}
						
					}
				}
				//add this tile to myTiles array at boardPosition (x,y)
				myTiles [Convert.ToInt32 (boardPositionX), Convert.ToInt32 (boardPositionY)] = myTile;
				
			} // END IF TILE NODES
		} //end iteration1
		
		//Now all tiles are added to board, iterate to add villages and then occupying units
		foreach (XmlNode node in doc.DocumentElement.ChildNodes) {
			
			if (node.Name == "village") {
				string playerName = node.Attributes ["playerName"].InnerText;
				string gold = node.Attributes ["gold"].InnerText;
				string wood = node.Attributes ["wood"].InnerText;
				string villageType = node.Attributes ["villageType"].InnerText;
				string locationOfTileX = node.Attributes ["locationOfTileX"].InnerText;
				string locationOfTileY = node.Attributes ["locationOfTileY"].InnerText;
				string damageTaken = node.Attributes["damageTaken"].InnerText;
				
				MW_Player villageOwner = new MW_Player();
				foreach(MW_Player myplayer in myGame.participants){
					if(playerName == myplayer.username){
						villageOwner = myplayer;
						break;
						
					}
				}
				List <AbstractTile> region = new List<AbstractTile> (); 
				Village myVillage = new Village (region, villageOwner);
				myVillage.gold = Convert.ToInt32 (gold);
				myVillage.wood = Convert.ToInt32 (wood);
				myVillage.damageTaken = Convert.ToInt32(damageTaken); //added damage taken 
				//convert villageType to enum
				VillageType vType = (VillageType)Enum.Parse (typeof(VillageType), villageType, true);
				myVillage.myType = vType;
				myVillage.location = myTiles [Convert.ToInt32 (locationOfTileX), Convert.ToInt32 (locationOfTileY)];
				//iterate over all controlledTiles of the village
				foreach (XmlNode villageChild in node.ChildNodes) { 
					string boardX = villageChild.Attributes ["boardX"].InnerText;
					string boardY = villageChild.Attributes ["boardY"].InnerText;
					if (villageChild.Name == "controlledTile") {
						region.Add (myTiles [Convert.ToInt32 (boardX), Convert.ToInt32 (boardY)]);
						myTiles [Convert.ToInt32 (boardX), Convert.ToInt32 (boardY)].myVillage = myVillage;
						
					}
				}
				//controlled region for village made
				myVillage.controlledRegion = region;
				villageOwner.myVillages.Add(myVillage);
			}
		}			
		
		foreach (XmlNode node in doc.DocumentElement.ChildNodes) {
			
			if (node.Name == "unit") {
				string unitType = node.Attributes ["unitType"].InnerText;
				string villageOwnerX = node.Attributes ["villageOwnerX"].InnerText;
				string villageOwnerY = node.Attributes ["villageOwnerY"].InnerText;
				string unitX = node.Attributes ["unitX"].InnerText;
				string unitY = node.Attributes ["unitY"].InnerText;
				string unitAction = node.Attributes ["unitAction"].InnerText;
				string isCannon = node.Attributes["isCannon"].InnerText;
				
				AbstractVillage unitVillage = myTiles [Convert.ToInt32 (villageOwnerX), Convert.ToInt32 (villageOwnerY)].myVillage;
				Tile unitTile = myTiles [Convert.ToInt32 (unitX), Convert.ToInt32 (unitY)];
				Unit myUnit = new Unit (unitVillage, unitTile);
				
				//convert string to enum
				ActionType aType = (ActionType)Enum.Parse (typeof(ActionType), unitAction, true);
				myUnit.currentAction = aType;
				
				UnitType uType = (UnitType)Enum.Parse(typeof(UnitType), unitType, true);
				myUnit.myType = uType;
				myUnit.isCannon = Convert.ToBoolean(isCannon);
				unitTile.occupyingUnit = myUnit;
				
			}
			
		}//end iterating over each node

		//testing, see what file is produced
		//XmlDocument mydoc = new XmlDocument ();
		//mydoc = saveGameState (myGame); 
		return myGame;

	} //end loadGameState







	public Board loadGameState(XmlDocument doc, MW_Game myGame){

		//<map> attributes
		string length = doc.DocumentElement.Attributes["length"].InnerText;
		string width = doc.DocumentElement.Attributes["width"].InnerText;
		string waterBorder = doc.DocumentElement.Attributes["waterBorder"].InnerText;

		Tile[,] myTiles = new Tile[Convert.ToInt32(length)+1, Convert.ToInt32(width)+1];
		Board myGameBoard = new Board(myTiles, Convert.ToInt32(waterBorder)); 

		//we will build myTiles from the XML file
		foreach (XmlNode node in doc.DocumentElement.ChildNodes) {
			if (node.Name == "tile") {
					string landType = node.Attributes ["landType"].InnerText;
					string boardPositionX = node.Attributes ["boardPositionX"].InnerText;
					string boardPositionY = node.Attributes ["boardPositionY"].InnerText;
					string gamePositionX = node.Attributes ["gamePositionX"].InnerText;
					string gamePositionY = node.Attributes ["gamePositionY"].InnerText;

					//make new tile
					LandType lType = (LandType)Enum.Parse (typeof(LandType), landType, true);
					Tile myTile = new Tile (lType, myGameBoard, Convert.ToInt32 (boardPositionX), Convert.ToInt32 (boardPositionY));
					Vector2 gamePos = new Vector2 (Convert.ToSingle (gamePositionX), Convert.ToSingle (gamePositionY)); //needs to be a float aka "single"
					myTile.gamePosition = gamePos;

				if (landType != "Sea") {
				
				
				//skip this if tile is a sea tile, else iterate over structure
					foreach (XmlNode tileChild in node.ChildNodes) { //should only be one tileChild - structure
							
							if (tileChild.Name == "structure") {
								string structureType = tileChild.Attributes ["structureType"].InnerText;
								//convert string to enum
								StructureType strucType = (StructureType)Enum.Parse (typeof(StructureType), structureType, true);
								Structure myStruc = new Structure (myTile, strucType);
								myTile.occupyingStructure = myStruc;
							}

					}
				}
				//add this tile to myTiles array at boardPosition (x,y)
				myTiles [Convert.ToInt32 (boardPositionX), Convert.ToInt32 (boardPositionY)] = myTile;

			} // END IF TILE NODES
		} //end iteration1

		//Now all tiles are added to board, iterate to add villages and then occupying units
		foreach (XmlNode node in doc.DocumentElement.ChildNodes) {

			if (node.Name == "village") {
				string playerName = node.Attributes ["playerName"].InnerText;
				string locationOfTileX = node.Attributes ["locationOfTileX"].InnerText;
				string locationOfTileY = node.Attributes ["locationOfTileY"].InnerText;
				/*
				string gold = node.Attributes ["gold"].InnerText;
				string wood = node.Attributes ["wood"].InnerText;
				string villageType = node.Attributes ["villageType"].InnerText;

				string damageTaken = node.Attributes["damageTaken"].InnerText;
				*/
				MW_Player villageOwner = new MW_Player();
				foreach(MW_Player myplayer in myGame.participants){
					if(playerName == myplayer.username){
						villageOwner = myplayer;
						break;

					}
				}
				Village myVillage = null;
				foreach(Village v in villageOwner.myVillages){
					if(v.location.boardPosition.x.ToString() == locationOfTileX && v.location.boardPosition.y.ToString() == locationOfTileY){
						myVillage = v;
						myVillage.location = myTiles [Convert.ToInt32 (locationOfTileX), Convert.ToInt32 (locationOfTileY)];
						myTiles [Convert.ToInt32 (locationOfTileX), Convert.ToInt32 (locationOfTileY)].myVillage = myVillage;
						break;
					}
				}


				List <AbstractTile> region = new List<AbstractTile> (); 
				/*
				Village myVillage = new Village (region, villageOwner);
				myVillage.gold = Convert.ToInt32 (gold);
				myVillage.wood = Convert.ToInt32 (wood);
				myVillage.damageTaken = Convert.ToInt32(damageTaken); //added damage taken 
				//convert villageType to enum
				VillageType vType = (VillageType)Enum.Parse (typeof(VillageType), villageType, true);
				myVillage.myType = vType;
				myVillage.location = myTiles [Convert.ToInt32 (locationOfTileX), Convert.ToInt32 (locationOfTileY)];
				*/
				//iterate over all controlledTiles of the village
				foreach (XmlNode villageChild in node.ChildNodes) { 
					string boardX = villageChild.Attributes ["boardX"].InnerText;
					string boardY = villageChild.Attributes ["boardY"].InnerText;
					if (villageChild.Name == "controlledTile") {
							region.Add (myTiles [Convert.ToInt32 (boardX), Convert.ToInt32 (boardY)]);
							myTiles [Convert.ToInt32 (boardX), Convert.ToInt32 (boardY)].myVillage = myVillage;

					}
				}
			//controlled region for village made
			myVillage.controlledRegion = region;
			
			}
		}			
			
		foreach (XmlNode node in doc.DocumentElement.ChildNodes) {

			if (node.Name == "unit") {
				string unitType = node.Attributes ["unitType"].InnerText;
				string villageOwnerX = node.Attributes ["villageOwnerX"].InnerText;
				string villageOwnerY = node.Attributes ["villageOwnerY"].InnerText;
				string unitX = node.Attributes ["unitX"].InnerText;
				string unitY = node.Attributes ["unitY"].InnerText;
				string unitAction = node.Attributes ["unitAction"].InnerText;
				string isCannon = node.Attributes["isCannon"].InnerText;

				//AbstractVillage unitVillage = myTiles [Convert.ToInt32 (villageOwnerX), Convert.ToInt32 (villageOwnerY)].myVillage;
				//Tile unitTile = myTiles [Convert.ToInt32 (unitX), Convert.ToInt32 (unitY)];
				Unit myUnit = new Unit (myTiles [Convert.ToInt32 (villageOwnerX), Convert.ToInt32 (villageOwnerY)].myVillage, myTiles [Convert.ToInt32 (unitX), Convert.ToInt32 (unitY)]);

				//convert string to enum
				ActionType aType = (ActionType)Enum.Parse (typeof(ActionType), unitAction, true);
				myUnit.currentAction = aType;

				UnitType uType = (UnitType)Enum.Parse(typeof(UnitType), unitType, true);
				myUnit.myType = uType;
				myUnit.isCannon = Convert.ToBoolean(isCannon);
				myTiles [Convert.ToInt32 (unitX), Convert.ToInt32 (unitY)].occupyingUnit = myUnit;
				myTiles [Convert.ToInt32 (villageOwnerX), Convert.ToInt32 (villageOwnerY)].myVillage.supportedUnits.Add(myUnit);


			}
					
		}//end iterating over each node
		//testing, see what file is produced
		//XmlDocument mydoc = new XmlDocument ();
		//fromWhere = "fromLoad";
		//mydoc = saveGameState (myGame); 
		//fromWhere = "";
	return myGameBoard;
	} //end loadGameState





	public XmlDocument saveGameState(MW_Game myGame) { //game state of myPlayer who ended turn will be saved
		//count ++;
		XmlDocument doc = new XmlDocument ();
		XmlNode rootNode = doc.CreateElement ("map");
		Board myBoard = myGame.gameBoard;

		//<map> attributes: length, width, height
		XmlAttribute length = doc.CreateAttribute ("length");
		length.Value =(myBoard.board.GetLength (0) - 1).ToString (); //length is -1 (?)
		XmlAttribute width = doc.CreateAttribute ("width");
		width.Value = (myBoard.board.GetLength(1) - 1).ToString (); 
		XmlAttribute waterBorder = doc.CreateAttribute ("waterBorder");
		waterBorder.Value = myBoard.border.ToString ();

		rootNode.Attributes.Append (length);
		rootNode.Attributes.Append (width);
		rootNode.Attributes.Append (waterBorder);
		doc.AppendChild (rootNode); 

		foreach (Tile t in myBoard.board) {

			if(t.myType == LandType.Sea){  //  when type is sea, we only serialize tile info
					
				XmlNode tileNode = doc.CreateElement ("tile");
				XmlAttribute landTypeAtt = doc.CreateAttribute ("landType");
				landTypeAtt.Value = "Sea";
				XmlAttribute boardPX = doc.CreateAttribute ("boardPositionX");
				boardPX.Value = t.boardPosition.x.ToString(); //x value
				XmlAttribute boardPY = doc.CreateAttribute ("boardPositionY");
				boardPY.Value = t.boardPosition.y.ToString(); //y value
				XmlAttribute gamePositionX = doc.CreateAttribute ("gamePositionX"); //x value
				gamePositionX.Value = t.gamePosition.x.ToString ();
				XmlAttribute gamePositionY = doc.CreateAttribute ("gamePositionY"); //y value
				gamePositionY.Value = t.gamePosition.y.ToString ();


				tileNode.Attributes.Append (landTypeAtt);
				tileNode.Attributes.Append (boardPX);
				tileNode.Attributes.Append (boardPY);
				tileNode.Attributes.Append (gamePositionX);
				tileNode.Attributes.Append (gamePositionY);
				rootNode.AppendChild (tileNode);
					
			}

			else{
				XmlNode tileNode = doc.CreateElement ("tile");
				XmlAttribute landTypeAtt = doc.CreateAttribute ("landType");
				landTypeAtt.Value = t.myType.ToString ();

				XmlAttribute x = doc.CreateAttribute ("boardPositionX");
				x.Value = t.boardPosition.x.ToString(); //x value
				XmlAttribute y = doc.CreateAttribute ("boardPositionY");
				y.Value = t.boardPosition.y.ToString(); //y value

				XmlAttribute gamePositionX = doc.CreateAttribute ("gamePositionX"); //x value
				gamePositionX.Value = t.gamePosition.x.ToString ();
				XmlAttribute gamePositionY = doc.CreateAttribute ("gamePositionY"); //y value
				gamePositionY.Value = t.gamePosition.y.ToString ();

				tileNode.Attributes.Append (landTypeAtt);
				tileNode.Attributes.Append(x);
				tileNode.Attributes.Append(y);
				tileNode.Attributes.Append (gamePositionX);
				tileNode.Attributes.Append (gamePositionY);
				rootNode.AppendChild (tileNode);

				//if there's a structure make a child node for tile called <structure>
				if(t.occupyingStructure != null){
					XmlNode structureNode = doc.CreateElement ("structure");
					XmlAttribute sTypeAtt = doc.CreateAttribute ("structureType");
					sTypeAtt.Value = t.occupyingStructure.myType.ToString ();
					structureNode.Attributes.Append (sTypeAtt);
					tileNode.AppendChild (structureNode);
				}
				//if there's a village and it's on the tile, make a village node
				if(t.myVillage != null && t.myVillage.location == t){
					XmlNode villageNode = doc.CreateElement ("village");
					XmlAttribute pName = doc.CreateAttribute ("playerName");
					pName.Value = t.myVillage.myPlayer.username;
					XmlAttribute goldAtt = doc.CreateAttribute ("gold");
					goldAtt.Value = t.myVillage.gold.ToString ();
					XmlAttribute woodAtt = doc.CreateAttribute ("wood");
					woodAtt.Value = t.myVillage.wood.ToString ();
					XmlAttribute vAtt = doc.CreateAttribute ("villageType");
					vAtt.Value = t.myVillage.myType.ToString ();
					XmlAttribute locX = doc.CreateAttribute ("locationOfTileX"); 
					locX.Value =t.myVillage.location.boardPosition.x.ToString ();
					XmlAttribute locY = doc.CreateAttribute ("locationOfTileY");
					locY.Value =t.myVillage.location.boardPosition.y.ToString ();
					XmlAttribute damageTaken = doc.CreateAttribute("damageTaken"); //added damageTaken to village
					damageTaken.Value = t.myVillage.damageTaken.ToString();


					villageNode.Attributes.Append (pName);
					villageNode.Attributes.Append (goldAtt);
					villageNode.Attributes.Append (woodAtt);
					villageNode.Attributes.Append (vAtt);
					villageNode.Attributes.Append (locX);
					villageNode.Attributes.Append (locY);
					villageNode.Attributes.Append(damageTaken);
					rootNode.AppendChild (villageNode); 

					//iterate over each tile in controlledRegion and add as subchildren to village node
					List<AbstractTile> region = t.myVillage.controlledRegion;
					foreach(Tile regionTile in region){
						XmlNode controlledTile = doc.CreateElement("controlledTile");
						XmlAttribute boardX = doc.CreateAttribute("boardX");
						boardX.Value = regionTile.boardPosition.x.ToString();
						XmlAttribute boardY = doc.CreateAttribute("boardY");
						boardY.Value = regionTile.boardPosition.y.ToString();

						controlledTile.Attributes.Append(boardX);
						controlledTile.Attributes.Append(boardY);
						villageNode.AppendChild(controlledTile); //<controlledTile> is a subchild
					}


				}
				//if there's a unit, make a node <unit>
				if(t.occupyingUnit != null){
					XmlNode unitNode = doc.CreateElement ("unit");
					XmlAttribute uTypeAtt = doc.CreateAttribute ("unitType");
					uTypeAtt.Value = t.occupyingUnit.myType.ToString ();
					XmlAttribute uAction = doc.CreateAttribute ("unitAction");
					uAction.Value = t.occupyingUnit.currentAction.ToString ();
					XmlAttribute xpos = doc.CreateAttribute("villageOwnerX");
					xpos.Value = t.occupyingUnit.myVillage.location.boardPosition.x.ToString();
					XmlAttribute ypos = doc.CreateAttribute("villageOwnerY");
					ypos.Value = t.occupyingUnit.myVillage.location.boardPosition.y.ToString();
					XmlAttribute unitPosX = doc.CreateAttribute("unitX");
					unitPosX.Value=t.occupyingUnit.myLocation.boardPosition.x.ToString();
					XmlAttribute unitPosY = doc.CreateAttribute("unitY");
					unitPosY.Value=t.occupyingUnit.myLocation.boardPosition.y.ToString();
					XmlAttribute isCannon = doc.CreateAttribute("isCannon"); //added isCannon boolean 
					isCannon.Value = t.occupyingUnit.isCannon.ToString();

					unitNode.Attributes.Append (uTypeAtt);
					unitNode.Attributes.Append (uAction);
					unitNode.Attributes.Append(xpos);
					unitNode.Attributes.Append(ypos);
					unitNode.Attributes.Append(unitPosX);
					unitNode.Attributes.Append(unitPosY);
					unitNode.Attributes.Append(isCannon);
					rootNode.AppendChild (unitNode); 
				}

			} //end all tiles but sea
		} //end iterating over each tile
		XmlNode gameNode = doc.CreateElement ("game");
		XmlAttribute turnIndex = doc.CreateAttribute ("turnIndex");
		turnIndex.Value = myGame.turnIndex.ToString ();
		XmlAttribute turnOf = doc.CreateAttribute ("turnOf");
		turnOf.Value = myGame.turnOf.username;

		gameNode.Attributes.Append (turnIndex);
		gameNode.Attributes.Append (turnOf);
		rootNode.AppendChild (gameNode);

		foreach(AbstractPlayer myPlayer in myGame.participants){
			XmlNode participantNode = doc.CreateElement("participant");
			XmlAttribute username = doc.CreateAttribute("username");
			username.Value = myPlayer.username;
			participantNode.Attributes.Append(username);
			gameNode.AppendChild(participantNode);
		}


		if (fromWhere== "fromLoad")
			doc.Save ("afterLoad.xml");
		else {
			doc.Save("beforeLoad.xml");
		}
		return doc;
	}


//FOR TESTING PURPOSES
	/*void Start(){
		XmlDocument doc = new XmlDocument ();
		XmlNode rootNode = doc.CreateElement ("map");
		doc.AppendChild (rootNode);
			
		for (int i = 1; i <= 10; i++) {

						if (i == 5){ // test for when type is sea, so we only serialize tile info
					
				XmlNode tileNode = doc.CreateElement ("tile");
				XmlAttribute landTypeAtt = doc.CreateAttribute ("landType");
				landTypeAtt.Value = "Sea";
				XmlAttribute boardPositionAtt = doc.CreateAttribute ("boardPosition");
				boardPositionAtt.Value = "Vector";
				XmlAttribute gamePositionAtt = doc.CreateAttribute ("gamePosition");
				gamePositionAtt.Value = "Vector";
				
				tileNode.Attributes.Append (landTypeAtt);
				tileNode.Attributes.Append (boardPositionAtt);
				tileNode.Attributes.Append (gamePositionAtt);
				rootNode.AppendChild (tileNode);


						}
			else{
						XmlNode tileNode = doc.CreateElement ("tile");
						XmlAttribute landTypeAtt = doc.CreateAttribute ("landType");
						landTypeAtt.Value = "Sea";
						XmlAttribute boardPositionAtt = doc.CreateAttribute ("boardPosition");
						boardPositionAtt.Value = "Vector";
						XmlAttribute gamePositionAtt = doc.CreateAttribute ("gamePosition");
						gamePositionAtt.Value = "Vector";

						tileNode.Attributes.Append (landTypeAtt);
						tileNode.Attributes.Append (boardPositionAtt);
						tileNode.Attributes.Append (gamePositionAtt);
						rootNode.AppendChild (tileNode);

						XmlNode villageNode = doc.CreateElement ("village");
						XmlAttribute pName = doc.CreateAttribute ("playerName");
						pName.Value = "Ben";
						XmlAttribute goldAtt = doc.CreateAttribute ("gold");
						goldAtt.Value = i.ToString ();
						XmlAttribute woodAtt = doc.CreateAttribute ("wood");
						woodAtt.Value = (i + 6).ToString ();
						XmlAttribute vAtt = doc.CreateAttribute ("villageType");
						vAtt.Value = "Hovel";
						XmlAttribute locAtt = doc.CreateAttribute ("locationOfTile");
						locAtt.Value = "Vector";

						villageNode.Attributes.Append (pName);
						villageNode.Attributes.Append (goldAtt);
						villageNode.Attributes.Append (woodAtt);
						villageNode.Attributes.Append (vAtt);
						villageNode.Attributes.Append (locAtt);
						tileNode.AppendChild (villageNode);

						XmlNode structureNode = doc.CreateElement ("structure");
						XmlAttribute sTypeAtt = doc.CreateAttribute ("structureType");
						sTypeAtt.Value = "Tower";
						structureNode.Attributes.Append (sTypeAtt);
						tileNode.AppendChild (structureNode);

						XmlNode unitNode = doc.CreateElement ("unit");
						XmlAttribute uTypeAtt = doc.CreateAttribute ("unitType");
						uTypeAtt.Value = "Knight";
						XmlAttribute uAction = doc.CreateAttribute ("unitAction");
						uAction.Value = "Fighting";

						unitNode.Attributes.Append (uTypeAtt);
						unitNode.Attributes.Append (uAction);

						tileNode.AppendChild (unitNode);
			}
			Debug.Log (i.ToString());
			
				}
		string test = "hello";
		string mytest = doc.OuterXml;
		doc.Save ("emily_"+test+".xml");
		Debug.Log (mytest);
		XmlDocument mydoc = new XmlDocument ();
		mydoc.LoadXml (mytest);
		Debug.Log ("loaded");
		}
	*/

} //end class



