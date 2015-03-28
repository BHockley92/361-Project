using UnityEngine;
using System.Xml;
using System.Collections.Generic;
using GameEnums;
using System;
/************ XML TEMPLATE ********
<map length = "" width = "", waterBorder ="">
    <tile landType = "" boardPositionX = "" boardPositionY = "" gamePositionX = "" gamePositionY="">

		<village playerName= "" gold= "" wood="" villageType="" locationOfTileX="" locationOfTileY=""/>
       
        <structure structureType = ""/>
    
        <unit unitType="" unitAction=""/>  
        
    </tile>
</map>

 */


public class SerializeGame  //: MonoBehaviour //uncomment when testing
{

	//advised not to do an XML diff, so for now we recreate the gameState from scratch by parsing xml file
	public Board loadGameState(string path){
		XmlDocument doc = new XmlDocument ();
		doc.Load (path); //ex: doc.Load("c:\\test.xml");

		//<map> attributes
		string length = doc.DocumentElement.Attributes["length"].InnerText;
		string width = doc.DocumentElement.Attributes["width"].InnerText;
		string waterBorder = doc.DocumentElement.Attributes["waterBorder"].InnerText;

		Tile[,] myTiles = new Tile[ Convert.ToInt32(length), Convert.ToInt32(width) ];
		Board myGameBoard = new Board(myTiles, Convert.ToInt32(waterBorder)); //will this cause an error because myTiles is null right now?

		//we will build myTiles from the XML file
		foreach (XmlNode node in doc.DocumentElement.ChildNodes) {

			if (node.Name == "tile"){
				string landType = node.Attributes["landType"].InnerText;
				string boardPositionX = node.Attributes["boardPositionX"].InnerText;
				string boardPositionY = node.Attributes["boardPositionY"].InnerText;
				string gamePositionX = node.Attributes["gamePositionX"].InnerText;
				string gamePositionY = node.Attributes["gamePositionY"].InnerText;

				//make new tile
				LandType lType = (LandType)Enum.Parse(typeof(LandType), landType, true );
				Tile myTile = new Tile(lType,myGameBoard,Convert.ToInt32(boardPositionX),Convert.ToInt32(boardPositionY));
				Vector2 gamePos = new Vector2(Convert.ToInt32(gamePositionX), Convert.ToInt32(gamePositionY));
				myTile.gamePosition = gamePos;


				//skip this if tile is a sea tile, else iterate over village, structure and unit nodes
				foreach (XmlNode tileChild in node.ChildNodes) {
					if(tileChild.Name == "village"){
						string playerName = tileChild.Attributes["playerName"].InnerText;
						string gold = tileChild.Attributes["gold"].InnerText;
						string wood = tileChild.Attributes["wood"].InnerText;
						string villageType = tileChild.Attributes["villageType"].InnerText;
						string locationOfTileX = tileChild.Attributes["locationOfTileX"].InnerText;
						string locationOfTileY = tileChild.Attributes["locationOfTileY"].InnerText;
						if(locationOfTileX == boardPositionX && locationOfTileY==boardPositionY){
							//village is on this tile


						}

					
					}

					if(tileChild.Name == "structure"){
						string structureType = tileChild.Attributes["structureType"].InnerText;
						//convert string to enum
						StructureType strucType = (StructureType)Enum.Parse(typeof(StructureType), structureType, true );
						Structure myStruc = new Structure(myTile,strucType);
					}

					if(tileChild.Name == "unit"){
						string unitType = tileChild.Attributes["unitType"].InnerText;
						//Unit myUnit = new 
						//myTile.occupyingUnit = myUnit;
					}





			}//end iterating over each tiles children: village, structure, unit


			} //end iterating over each tile

		}

		return myGameBoard;
	} //end loadGameState





	public XmlDocument saveGameState(MW_Game myGame, MW_Player myPlayer = null, string path = null) { //game state of myPlayer who ended turn will be saved

		XmlDocument doc = new XmlDocument ();
		XmlNode rootNode = doc.CreateElement ("map");
		doc.AppendChild (rootNode);

		Board myBoard = myGame.gameBoard;

		XmlAttribute length = doc.CreateAttribute ("length");
		length.Value =(myBoard.board.GetLength (0) - 1).ToString ();

		XmlAttribute width = doc.CreateAttribute ("width");
		width.Value = (myBoard.board.GetLength(1) - 1).ToString (); 

		XmlAttribute waterBorder = doc.CreateAttribute ("waterBorder");
		waterBorder.Value = myBoard.border.ToString ();

		rootNode.Attributes.Append (length);
		rootNode.Attributes.Append (width);
		rootNode.Attributes.Append (waterBorder);
				
		foreach (Tile t in myBoard.board) {

			if(t.myType == LandType.Sea){  //  when type is sea, we only serialize tile info
					
				XmlNode tileNode = doc.CreateElement ("tile");
				XmlAttribute landTypeAtt = doc.CreateAttribute ("landType");
				landTypeAtt.Value = "Sea";
				XmlAttribute x = doc.CreateAttribute ("boardPositionX");
				x.Value = t.boardPosition.x.ToString(); //x value
				XmlAttribute y = doc.CreateAttribute ("boardPositionY");
				x.Value = t.boardPosition.y.ToString(); //y value
				XmlAttribute gamePositionX = doc.CreateAttribute ("gamePositionX"); //x value
				gamePositionX.Value = t.gamePosition.x.ToString ();
				XmlAttribute gamePositionY = doc.CreateAttribute ("gamePositionY"); //y value
				gamePositionY.Value = t.gamePosition.x.ToString ();


				tileNode.Attributes.Append (landTypeAtt);
				tileNode.Attributes.Append (x);
				tileNode.Attributes.Append (y);
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
				x.Value = t.boardPosition.y.ToString(); //y value

				XmlAttribute gamePositionX = doc.CreateAttribute ("gamePositionX"); //x value
				gamePositionX.Value = t.gamePosition.x.ToString ();
				XmlAttribute gamePositionY = doc.CreateAttribute ("gamePositionY"); //y value
				gamePositionY.Value = t.gamePosition.x.ToString ();

				tileNode.Attributes.Append (landTypeAtt);
				tileNode.Attributes.Append(x);
				tileNode.Attributes.Append(y);
				tileNode.Attributes.Append (gamePositionX);
				tileNode.Attributes.Append (gamePositionY);
				rootNode.AppendChild (tileNode);
			
				XmlNode villageNode = doc.CreateElement ("village");
				XmlAttribute pName = doc.CreateAttribute ("playerName");
				pName.Value = t.myVillage.myPlayer.username;
				XmlAttribute goldAtt = doc.CreateAttribute ("gold");
				goldAtt.Value = t.myVillage.gold.ToString ();
				XmlAttribute woodAtt = doc.CreateAttribute ("wood");
				woodAtt.Value = t.myVillage.wood.ToString ();
				XmlAttribute vAtt = doc.CreateAttribute ("villageType");
				vAtt.Value = t.myVillage.myType.ToString ();
				XmlAttribute locX = doc.CreateAttribute ("locationOfTileX");//removed plural why was it there 
				locX.Value =t.myVillage.location.boardPosition.x.ToString ();
				XmlAttribute locY = doc.CreateAttribute ("locationOfTileY");//removed plural why was it there 
				locY.Value =t.myVillage.location.boardPosition.y.ToString ();

				villageNode.Attributes.Append (pName);
				villageNode.Attributes.Append (goldAtt);
				villageNode.Attributes.Append (woodAtt);
				villageNode.Attributes.Append (vAtt);
				villageNode.Attributes.Append (locX);
				villageNode.Attributes.Append (locY);
								tileNode.AppendChild (villageNode);
			
				XmlNode structureNode = doc.CreateElement ("structure");
				XmlAttribute sTypeAtt = doc.CreateAttribute ("structureType");
				sTypeAtt.Value = t.occupyingStructure.myType.ToString ();
				structureNode.Attributes.Append (sTypeAtt);
				tileNode.AppendChild (structureNode);
			
				XmlNode unitNode = doc.CreateElement ("unit");
				XmlAttribute uTypeAtt = doc.CreateAttribute ("unitType");
				uTypeAtt.Value = t.occupyingUnit.myType.ToString ();
				XmlAttribute uAction = doc.CreateAttribute ("unitAction");
				uAction.Value = t.occupyingUnit.currentAction.ToString ();
			
				unitNode.Attributes.Append (uTypeAtt);
				unitNode.Attributes.Append (uAction);
			
				tileNode.AppendChild (unitNode);
			}
		} //end iterating over each tile

		//save xml file state_username.xml
		if(path != null) {
			string pname = myPlayer.username.ToString ();
			doc.Save ("state_"+pname+".xml");
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
		doc.Save ("emily_"+test+".xml");
		}
*/
} //end class



