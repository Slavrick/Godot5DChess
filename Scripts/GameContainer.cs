using Godot;
using System;
using Engine;
using FileIO5D;
using System.Collections.Generic;

public partial class GameContainer : Control
{
	GameStateManager gsm;
	List<CoordFour> destinations;
	CoordFive SelectedSquare;
	Node mvcontainer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode("SubViewport/Menus").Connect("submit_turn", new Callable(this,nameof(SubmitTurn)));
		GetNode("SubViewport/Menus").Connect("undo_turn", new Callable(this,nameof(UndoTurn)));
		GetNode("SubViewport/Menus").Connect("load_game", new Callable(this,nameof(OpenFileDialog)));
		GetNode("FileDialog").Connect("file_selected", new Callable(this, nameof(LoadGame)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	
	public void GameStateToGodotNodes()
	{
		var multiverse = ResourceLoader.Load<PackedScene>("res://Scenes/UI/multiverse_Container.tscn").Instantiate();
		multiverse.Set("chessboard_dimensions",new Vector2(gsm.Width,gsm.Height));
		for(int i = 0; i < gsm.Multiverse.Count; i++){
			multiverse.AddChild(TimeLineToGodotNodes(gsm.Multiverse[i],gsm.MinTL+i));
		}
		multiverse.Connect("square_clicked", new Callable(this,nameof(HandleClick)));
		mvcontainer = multiverse;
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
		scene.Set("board_width",gsm.Width);
		scene.Set("board_height",gsm.Height);
		scene.Set("board", arr);
		scene.Set("multiverse_position", new Vector2(L,T));
		scene.Set("color", color);
		scene.Call("logicalBoardToUIBoard");
		return scene;
	}
	
	public void HandleClick(Vector2 square, Vector2 Temporalposition, bool color){
		CoordFour clicked = new CoordFour((int)square.X,(int)square.Y,(int)Temporalposition.Y,(int)Temporalposition.X);
		if( destinations == null ){
			GetDestinationsFromClick(square,Temporalposition,color);
		}
		else if(destinations.Contains(clicked)){
			if(gsm.CoordIsPlayable(SelectedSquare)){
				Move SelectedMove = new Move(SelectedSquare,clicked);
				gsm.MakeMove(SelectedMove);
				//need to add cleanup
				UpdateRender();
			}
			
		}else{
			GetDestinationsFromClick(square,Temporalposition,color);
		}
		
	}
	
	public void GetDestinationsFromClick(Vector2 square, Vector2 Temporalposition, bool color){
		//Need to pass the square. From the Square get the piece and coord.
		CoordFive coord = new CoordFive((int)square.X,(int)square.Y,(int)Temporalposition.Y,(int)Temporalposition.X,color);
		SelectedSquare = coord;
		Console.WriteLine(coord);
		int piece = gsm.GetSquare(coord);
		if(piece == 0){
			return;
		}
		destinations = MoveGenerator.GetMoves(piece,gsm,coord);
		Godot.Collections.Array DestinationsGodot = new Godot.Collections.Array();
		foreach( CoordFour cd in destinations ){
			DestinationsGodot.Add(new Vector4(cd.X,cd.Y,cd.L,cd.T));
		}
		if( mvcontainer != null){
			mvcontainer.Call("clear_highlights");
			mvcontainer.Call("highlight_squares",DestinationsGodot,color);
		}
		//CoordFive coord = new CoordFive(); based on what was passed
		//MoveGenerator.getMoves(piece,gsm,coord);
	}
	
	public void SubmitTurn(){
		gsm.SubmitMoves();
		GetNode("SubViewport/Menus").Call("set_turn_label",gsm.Color,gsm.Present);
	}
	
	public void UndoTurn(){
		gsm.undoTempMoves();
		UpdateRender();
	}
	
	public void UpdateRender(){
		if(mvcontainer != null){
			mvcontainer.Call("queue_free");
		}if(gsm != null){
			GameStateToGodotNodes();
			GetNode("SubViewport/Menus").Call("set_turn_label",gsm.Color,gsm.Present);
		}
	}
	
	
	public void LoadGame(String filepath){
		gsm = FENParser.ShadSTDGSM(filepath);
		UpdateRender();
	}
	
	
	public void OpenFileDialog(){
		GetNode("FileDialog").Call("show");
	}
	
	
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("SubmitTurn"))
		{
			SubmitTurn();
		}
		if (@event.IsActionPressed("UndoTurn"))
		{
			UndoTurn();
		}
	}
}
