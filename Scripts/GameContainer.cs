using Godot;
using System;
using Engine;
using FileIO5D;

public partial class GameContainer : Control
{
	GameStateManager gsm;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//gsm = FENParser.ShadSTDGSM("res://Resources/res/testPGNs/ExampleGame.PGN5.txt");
		gsm = FENParser.ShadSTDGSM("res://Resources/res/Puzzles/Puzzlenew.txt");
		GameStateToGodotNodes();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void GameStateToGodotNodes()
	{
		var multiverse = ResourceLoader.Load<PackedScene>("res://Scenes/UI/multiverse_Container.tscn").Instantiate();
		for(int i = 0; i < gsm.Multiverse.Count; i++){
			multiverse.AddChild(TimeLineToGodotNodes(gsm.Multiverse[i],gsm.MinTL+i));
		}
		AddChild(multiverse);
	}
	
	public Node TimeLineToGodotNodes(Timeline tl, int timelineLayer )
	{
		var timeline = ResourceLoader.Load<PackedScene>("res://Scenes/UI/time_line_drawer.tscn").Instantiate();
		//Get all the boards and add them to the array
		for( int i = 0; i < tl.WBoards.Count; i++){
			timeline.AddChild(BoardToGodotNodes(tl.WBoards[i],i + tl.WhiteStart, timelineLayer, true));
		}
		
		for( int i = 0; i < tl.BBoards.Count; i++){
			timeline.AddChild(BoardToGodotNodes(tl.BBoards[i],i + tl.BlackStart, timelineLayer, false));
		}
		timeline.Set("layer",timelineLayer);
		return timeline;
	}
	
	public Node BoardToGodotNodes(Board b, int T, int L, bool color) 
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/BoardDrawer.tscn").Instantiate();
		var arr = new Godot.Collections.Array();
		for(int x = 0; x < b.Width; x++){
			for(int y = 0; y < b.Height; y++){
				int piece = b.Brd[x][y];
				if( piece < 0 ){
					piece *= -1;
				}
				arr.Add(piece);
			}	
		}
		Console.WriteLine(arr);
		scene.Set("board", arr);
		scene.Set("multiverse_position", new Vector2(L,T));
		scene.Set("color", color);
		scene.Call("logicalBoardToUIBoard");
		return scene;
	}
}
