using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple GUI class, with two button, might change or even be deleted
/// It is juste for test purpose for now.
/// </summary>
public class GUI : MonoBehaviour {
    private Dungeon _dungeon;
    private AABB _bounds;
	// Use this for initialization
	void Start () {
		_dungeon = Dungeon.GetInstance();
        _bounds = new AABB(new XY(30, 30), new XY(20, 20));
        
    }
	
    void OnGUI()
    {
        if (GUILayout.Button("Add Room"))
        {
            Room.CreateRandomRoom(_bounds).Draw(_dungeon.Texture);
        }
        if (GUILayout.Button("Show Boundaries"))
        {
            _bounds.Draw(_dungeon.Texture);
        }
        if (GUILayout.Button("Draw tree limits"))
        {
            gameObject.GetComponent<Generator>().DrawTreeLimits();
        }
        if (GUILayout.Button("Draw tree rooms"))
        {
            gameObject.GetComponent<Generator>().DrawTreeRooms();
        }
        if (GUILayout.Button("Draw Tiles"))
        {
            gameObject.GetComponent<Generator>().DrawTiles();
        }
        if (GUILayout.Button("Link Rooms"))
        {
            gameObject.GetComponent<Generator>().LinkRooms();
        }
    }
}
