using UnityEngine;
using System.Collections;
using GameEnums;

public class ValueManager
{
	private string filePath { get; set; }
	private int value;

	public ValueManager();

	public int getUnitValue(UnitType ut);
	public int getLandValue(LandType lt);

}
