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


public class SerializeGame //: MonoBehaviour //uncomment when testing
{
	public void saveGameState(MW_Game myGame, MW_Player myPlayer) { //game state of myPlayer who ended turn will be saved

		XmlDocument doc = new XmlDocument ();
		XmlNode rootNode = doc.CreateElement ("map");
		doc.AppendChild (rootNode);

		Board myBoard = myGame.gameBoard;
		
		foreach (Tile t in myBoard.board) {

			if(t.myType == LandType.Sea){  //  when type is sea, we only serialize tile info
					
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
				landTypeAtt.Value = t.myType.ToString ();
				XmlAttribute boardPositionAtt = doc.CreateAttribute ("boardPosition");
				boardPositionAtt.Value = t.boardPosition.ToString ();
				XmlAttribute gamePositionAtt = doc.CreateAttribute ("gamePosition");
				gamePositionAtt.Value = t.gamePosition.ToString ();
			
				tileNode.Attributes.Append (landTypeAtt);
				tileNode.Attributes.Append (boardPositionAtt);
				tileNode.Attributes.Append (gamePositionAtt);
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
				XmlAttribute locAtt = doc.CreateAttribute ("locationOfTiles");
				locAtt.Value =t.myVillage.location.boardPosition.ToString ();
			
				villageNode.Attributes.Append (pName);
				villageNode.Attributes.Append (goldAtt);
				villageNode.Attributes.Append (woodAtt);
				villageNode.Attributes.Append (vAtt);
				villageNode.Attributes.Append (locAtt);
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
		string pname = myPlayer.username.ToString ();
		doc.Save ("state_"+pname+".xml");
	}


//FOR TESTING PURPOSES
	void Start(){
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
			}
			Debug.Log (i.ToString());
			
				}
		string test = "hello";
		doc.Save ("emily_"+test+".xml");
		}

} //end class



