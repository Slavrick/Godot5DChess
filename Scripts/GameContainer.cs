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
	
	[Signal]
	public delegate void GameLoadedEventHandler();
	
	public GameStateManager gsm;
	public List<CoordFour> destinations;
	public CoordFive SelectedSquare;
	public List<Node> Arrows;
	public List<Node> TempArrows;
	public List<Node> CheckArrows;
	
	public bool AnalysisMode = false;
	
	Node mvcontainer;
	
	public override void _Ready()
	{
		CheckArrows = new List<Node>();
		Arrows = new List<Node>();
		TempArrows = new List<Node>();
		GetNode("SubViewport/Menus").Connect("submit_turn", new Callable(this,nameof(SubmitTurn)));
		GetNode("SubViewport/Menus").Connect("undo_turn", new Callable(this,nameof(UndoTurn)));
		//GetNode("SubViewport/Menus").Connect("checkforchecks", new Callable(this,nameof(CheckForChecks)));
		GetNode("SubViewport/Menus").Connect("load_game", new Callable(this,nameof(OpenFileDialog)));
		GetNode("FileDialog").Connect("file_selected", new Callable(this, nameof(LoadGame)));
		GetNode("SubViewport/GameEscapeMenu/Button").Connect("pressed", new Callable(this, nameof(ExitGamePressed)));
		GetNode("/root/VisualSettings").Connect("view_changed", new Callable(this, nameof(OnViewChanged)));
		if(AnalysisMode){
			GetNode("SubViewport/Menus").Call("set_analysis_mode");
		}
	}

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
		timeline.Set("name",timelineLayer.ToString() + "L");
		return timeline;
	}
	
	public Node BoardToGodotNodes(Board b, int T, int L, bool color) 
	{
		var boardScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/BoardDrawer.tscn").Instantiate();
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
		boardScene.Set("board_width",gsm.Width);
		boardScene.Set("board_height",gsm.Height);
		boardScene.Set("board", arr);
		boardScene.Set("multiverse_position", new Vector2(L,T));
		boardScene.Set("color", color);
		boardScene.Call("logicalBoardToUIBoard");
		String colorchar;
		if(color){
			colorchar = "w";
		}else{
			colorchar = "b";
		}
		boardScene.Set("name",colorchar+L.ToString()+"T"+T.ToString());
		return boardScene;
	}
	
	public void HandleClick(Vector2 square, Vector2 Temporalposition, bool color){
		CoordFour clicked = new CoordFour((int)square.X,(int)square.Y,(int)Temporalposition.Y,(int)Temporalposition.X);
		int piece = gsm.GetSquare(clicked,color);
		if( destinations == null ){
			if(ValidateClickSquare(new CoordFive(clicked,color))){
				GetDestinationsFromClick(square,Temporalposition,color);
			}
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
						Node a = CreateArrow(SelectedMove,color);
						TempArrows.Add(a);
						AddChild(a);
						EmitSignal(SignalName.MoveMade, tile,!color);
					}
					else{
						Vector2 tile = new Vector2(clicked.L,clicked.T);
						if(!color){
							tile.Y += 1;
						}
						Node a = CreateArrow(SelectedMove,color);
						TempArrows.Add(a);
						AddChild(a);
						EmitSignal(SignalName.MoveMade, tile, !color);
					}
					CheckForChecks();
					UpdateRender();
					//need to add cleanup
				}
			}
			
		}else{
			if(ValidateClickSquare(new CoordFive(clicked,color))){
				GetDestinationsFromClick(square,Temporalposition,color);
			}
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
	
	public bool ValidateClickSquare(CoordFive cf){
		if(cf.Color != gsm.Color){
			return false;
		}
		if(!gsm.CoordIsPlayable(cf)){
			return false;
		}
		int piece = gsm.GetSquare(cf);
		if(Board.GetColorBool(piece) != gsm.Color){
			return false;
		}
		return true;
	}
	
	public void UpdateRender(){
		if(mvcontainer != null){
			mvcontainer.Call("queue_free");
		}if(gsm != null){
			GameStateToGodotNodes();
			GetNode("SubViewport/Menus").Call("set_turn_label",gsm.Color,gsm.Present);
		}
	}
	
	public void CheckForChecks(){
		foreach(Node child in CheckArrows){
			child.Call("queue_free");
		}
		CheckArrows.Clear();
		List<Move> moves = MoveGenerator.GetCurrentThreats(gsm,gsm.Color);
		foreach(Move m in moves){
			Node a = CreateArrow(m,!gsm.Color, new Color(1,0,0,(float)0.5));
			AddChild(a);
			CheckArrows.Add(a);
		}
	}
	
	public Vector2 GetPresentTile(){
		CoordFour cf = gsm.GetPresentCoordinate(0);
		return new Vector2(cf.L,cf.T);
	}
	
	public Node CreateArrow(Move m, bool color)
	{
		var arrow = ResourceLoader.Load<PackedScene>("res://Scenes/UI/arrow_draw.tscn").Instantiate();
		arrow.Set("origin",GameInterface.CoordFourtoCoord5(m.Origin,color));
		arrow.Set("dest",GameInterface.CoordFourtoCoord5(m.Dest,color));
		return arrow;
	}
	
	public Node CreateArrow(Move m, bool color, Color c)
	{
		var arrow = ResourceLoader.Load<PackedScene>("res://Scenes/UI/arrow_draw.tscn").Instantiate();
		arrow.Set("origin",GameInterface.CoordFourtoCoord5(m.Origin,color));
		arrow.Set("dest",GameInterface.CoordFourtoCoord5(m.Dest,color));
		arrow.Set("arrow_color",c);
		return arrow;
	}
	
	public void ClearTurnArrows(){
		foreach(Node child in Arrows){
			child.Call("queue_free");
		}
		Arrows.Clear();
	}
	
	public void SubmitTurn(){
		bool SubmitSuccessful = gsm.SubmitMoves();
		if( SubmitSuccessful ){
			EmitSignal(SignalName.TurnChanged, gsm.Color, gsm.Present);
			Arrows.AddRange(TempArrows);
			TempArrows.Clear();
		}
		if( SubmitSuccessful && gsm.isMated()) {;
			EmitSignal(SignalName.IsMated, gsm.Color);
		}
		destinations = null;
		GetNode("SubViewport/Menus").Call("set_turn_label",gsm.Color,gsm.Present);//This is awful
	}
	
	public void UndoTurn(){
		foreach(Node n in TempArrows){
			n.Call("queue_free");
		}
		TempArrows.Clear();
		gsm.undoTempMoves();
		UpdateRender();
	}
	public void LoadGame(String filepath){
		Console.WriteLine("chose Path: " + filepath);
		gsm = FENParser.ShadSTDGSM(filepath);
		GetNode("/root/VisualSettings").Set("game_board_dimensions", new Vector2(gsm.Width,gsm.Height));
		GetNode("/root/VisualSettings").Call("change_game");
		UpdateRender();
		ClearTurnArrows();
		EmitSignal(SignalName.GameLoaded);
	}
	
	public void OpenFileDialog(){
		GetNode("FileDialog").Call("show");
	}
	
	public void ExitGamePressed(){
		EmitSignal(SignalName.ExitGame);
	}
	
	public void flip_pespective(){
		mvcontainer.Call("flip_perspective");
	}
	
	public void OnViewChanged(bool multiverse_perspective, int multiverseview)
	{
		//foreach(Node arrow in Arrows)
		//{
			//arrow.Call("get_coordinate");
		//}
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
	

}
