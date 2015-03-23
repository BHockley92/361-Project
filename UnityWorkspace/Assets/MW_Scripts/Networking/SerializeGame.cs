using UnityEngine;
using System.Xml;
using System.Collections.Generic;
using GameEnums;
using System;
/************ XML TEMPLATE ********
<map>
    <tile landType = "" boardPosition = "" gamePosition = "">

		<village playerName= "" gold= "" wood="" villageType="" locationOfTile=""/>
       
        <structure structureType = ""/>
    
        <unit unitType="" unitAction=""/>  
        
    </tile>
</map>

 */


public class SerializeGame 
{
	public void saveGameState(MW_Game myGame) {
				//XmlDocument doc = new XmlDocument ();

				Board myBoard = myGame.gameBoard;
		
				foreach (Tile t in myBoard.board) {
						string landType = t.myType.ToString ();
						string boardPosition = t.boardPosition.ToString ();
						string gamePosition = t.gamePosition.ToString ();

						string playerName = t.myVillage.myPlayer.username;
						string gold = t.myVillage.gold.ToString ();
						string wood = t.myVillage.wood.ToString ();
						string villageType = t.myVillage.myType.ToString ();
						string locationOfTile = t.myVillage.location.boardPosition.ToString (); //not sure if that is the location Ben wants

						string unitType = t.occupyingUnit.myType.ToString ();
						string unitAction = t.occupyingUnit.currentAction.ToString ();

						string structureType = t.occupyingStructure.myType.ToString ();



				}
		}

	static void Main( string[ ] args ) {
		XmlDocument doc = new XmlDocument ();
		XmlNode rootNode = doc.CreateElement ("map");
		doc.AppendChild (rootNode);
			
		XmlNode tileNode = doc.CreateElement ("tile");
		XmlAttribute landTypeAtt = doc.CreateAttribute ("landType");
		landTypeAtt.Value = "Sea";
		XmlAttribute boardPositionAtt = doc.CreateAttribute ("boardPosition");
		boardPositionAtt.Value = "Vector1";
		XmlAttribute gamePositionAtt = doc.CreateAttribute ("gamePosition");
		gamePositionAtt.Value = "Vector1";

		tileNode.Attributes.Append (landTypeAtt);
		tileNode.Attributes.Append (boardPositionAtt);
		tileNode.Attributes.Append (gamePositionAtt);
		rootNode.AppendChild (tileNode);

		XmlNode villageNode = doc.CreateElement ("village");
		XmlAttribute pName = doc.CreateAttribute ("playerName");
		pName.Value = "Ben";
		XmlAttribute goldAtt = doc.CreateAttribute ("gold");
		goldAtt.Value = "78";
		XmlAttribute woodAtt = doc.CreateAttribute ("wood");
		woodAtt.Value = "56";
		XmlAttribute vAtt = doc.CreateAttribute ("villageType");
		vAtt.Value = "Hovel";
		XmlAttribute locAtt = doc.CreateAttribute ("locationOfTiles");
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

		doc.Save ("test.xml");
		}



} //end class



