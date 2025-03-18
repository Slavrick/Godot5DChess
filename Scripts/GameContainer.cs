using Godot;
using System;
using Engine;
using FileIO5D;
using System.Collections.Generic;

public partial class GameContainer : Control
{
	[Signal]
	public delegate void ExitGameEventHandler();
	
	[Signal]
	public delegate void IsMatedEventHandler(bool player_mated);
	
	[Signal]
	public delegate void MoveMadeEventHandler(Vector2 tile_destination, bool destination_color);
	
	[Signal]
	public delegate void TurnChangedEventHandler(bool player, int present);
	
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
		GetNode("SubViewport/Menus").Connect("flip_perspective", new Callable(this,nameof(flip_pespective)));
		GetNode("FileDialog").Connect("file_selected", new Callable(this, nameof(LoadGame)));
		GetNode("SubViewport/GameEscapeMenu/Button").Connect("pressed", new Callable(this, nameof(ExitGamePressed)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	
	public void GameStateToGodotNodes()
	{
		var multiverse = ResourceLoader.Load<PackedScene>("res://Scenes/UI/multiverse_container.tscn").Instantiate();
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
		var timeline = ResourceLoader.Load<PackedScene>("res://Scenes/UI/timeline_drawer.tscn").Instantiate();
		//Get all the boards and add them to the array
		for( int i = 0; i < tl.WBoards.Count; i++){
			timeline.AddChild(BoardToGodotNodes(tl.WBoards[i],i + tl.WhiteStart, timelineLayer, true));
		}
		
		for( int i = 0; i < tl.BBoards.Count; i++){
			timeline.AddChild(BoardToGodotNodes(tl.BBoards[i],i + tl.BlackStart, timelineLayer, false));
		}
		timeline.Set("layer",timelineLayer);
		timeline.Set("TStart",tl.TStart);
		timeline.Set("color_start",tl.ColorStart);
		timeline.Set("chessboard_dimensions",new Vector2(gsm.Width,gsm.Height));
		return timeline;
	}
	
	public Node BoardToGodotNodes(Board b, int T, int L, bool color) 
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/BoardDrawer.tscn").Instantiate();
		var arr = new Godot.Collections.Array();
		for(int x = 0; x < b.Width; x++){
			for(int y = 0; y < b.Height; y++){
				int piece = b.getSquare(y,x); // no idea why this is inverted, but o well.
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
				bool MoveStatus = gsm.MakeMove(SelectedMove);
				if(MoveStatus){
					if(SelectedMove.Type == 3 ){
						Console.WriteLine("branching move detected");
						int Layer;
						if(color){
							Layer = gsm.MaxTL;
						}else{
							Layer = gsm.MinTL;
						}
						Vector2 tile = new Vector2(Layer,clicked.T);
						if(!color){
							tile.Y += 1;
						}
						EmitSignal(SignalName.MoveMade, tile,!color);
					}
					else{
						Vector2 tile = new Vector2(clicked.L,clicked.T);
						if(!color){
							tile.Y += 1;
						}
						EmitSignal(SignalName.MoveMade, tile, !color);
					}
					UpdateRender();
					//need to add cleanup
				}
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
		bool SubmitSuccessful = gsm.SubmitMoves();
		if( SubmitSuccessful ){
			EmitSignal(SignalName.TurnChanged, gsm.Color, gsm.Present);
		}
		if( SubmitSuccessful && gsm.isMated()) {;
			EmitSignal(SignalName.IsMated, gsm.Color);
		}
		GetNode("SubViewport/Menus").Call("set_turn_label",gsm.Color,gsm.Present);//This is awful
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
		Console.WriteLine("chose Path: " + filepath);
		gsm = FENParser.ShadSTDGSM(filepath);
		GetNode("/root/VisualSettings").Set("game_board_dimensions", new Vector2(gsm.Width,gsm.Height));
		GetNode("/root/VisualSettings").Call("change_game");
		UpdateRender();
	}
	
	
	public void OpenFileDialog(){
		GetNode("FileDialog").Call("show");
	}
	
	
	public void ExitGamePressed(){
		EmitSignal(SignalName.ExitGame);
	}
	
	
	public Vector2 GetPresentTile(){
		CoordFour cf = gsm.GetPresentCoordinate(0);
		return new Vector2(cf.L,cf.T);
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
		if(@event.IsActionPressed("escMenu"))
		{
			Control escMenu = GetNode("SubViewport/GameEscapeMenu") as Control;
			
			if(escMenu.Visible){
				escMenu.Call("hide");
			}
			else
			{
				escMenu.Call("show");
			}
		}
	}
	
	public void flip_pespective(){
		mvcontainer.Call("flip_perspective");
	}
}
