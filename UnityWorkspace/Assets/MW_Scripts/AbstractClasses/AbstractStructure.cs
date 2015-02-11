using UnityEngine;
using System.Collections;
using GameEnums;

public class AbstractStructure 
{
	public StructureType myType { get; private set; }
	public AbstractTile myLocation { get; set; }
}
