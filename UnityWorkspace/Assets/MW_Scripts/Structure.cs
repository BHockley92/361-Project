using UnityEngine;
using System.Collections;
using GameEnums;

public class Structure 
{
	private StructureType myType { get; }

	public Structure (Tile loc);
	public Tile getMyLocation();
}
