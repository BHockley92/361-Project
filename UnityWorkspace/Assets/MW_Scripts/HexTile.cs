using UnityEngine;
using System.Collections;

public class HexTile : MonoBehaviour {

	public Vector3 [] VERTICES;
	public Vector2 [] UV;
	public int[] 	  TRIANGLES;
	public Texture    TEXTURE;
	
	public void InstantiateTile() {
		float floorLevel = 0;
		VERTICES  = new Vector3 []
		{
			new Vector3 (-1f , floorLevel, -.5f),
			new Vector3 (-1f, floorLevel, .5f),
			new Vector3 (0f, floorLevel, 1f),
			new Vector3 (1f, floorLevel, .5f),
			new Vector3 (1f, floorLevel, -.5f),
			new Vector3 (0f, floorLevel, -1f)
		};
		
		TRIANGLES  = new int[]
		{
			1,5,0,
			1,4,5,
			1,2,4,
			2,3,4
		};
		
		UV = new Vector2 []
		{
			new Vector2 (0,0.25f),
			new Vector2 (0,0.75f),
			new Vector2 (0.5f,1),
			new Vector2 (1,0.75f),
			new Vector2 (1,0.25f),
			new Vector2 (0.5f,0),
		};

		MeshFilter  meshFilter = gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		Mesh mesh = new Mesh ();

		mesh.vertices  = VERTICES ;

		mesh.triangles  = TRIANGLES ;

		mesh.uv = UV;
		mesh.RecalculateNormals();

		meshFilter.mesh = mesh;

		meshRenderer.material.mainTexture = TEXTURE;
	}
}
