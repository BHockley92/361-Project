using UnityEngine;
using System.Collections;

public class HexTile : MonoBehaviour {

	public Vector3 [] VERTICES;
	public Vector2 [] UV;
	public int[] 	  TRIANGLES;
	public Texture    TEXTURE;
	
	public void MeshSetup(Color playerColour)
	{
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
		Mesh  mesh = new Mesh ();

		mesh.vertices  = VERTICES ;

		mesh.triangles  = TRIANGLES ;

		mesh.uv = UV;
		mesh.RecalculateNormals();

		meshFilter.mesh = mesh;

		meshRenderer.material.mainTexture = TEXTURE;
		
		// make border tile
		GameObject tile = (GameObject)Resources.Load("Tile");
		GameObject border = Instantiate(tile, transform.position, Quaternion.identity) as GameObject;
		HexTile ht = border.GetComponent<HexTile>();
		
		
		MeshFilter  meshFilter2 = border.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer2 = border.AddComponent<MeshRenderer>();
		
		meshFilter2.mesh = mesh;
		meshRenderer2.material.mainTexture = (Texture)Resources.Load ("border-white");
		meshRenderer2.material.shader = Shader.Find ("Transparent/Diffuse");
		meshRenderer2.material.color = playerColour;
	}
}
