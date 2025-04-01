using Godot;
using System;
using FiveDChess;
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
	
	[Signal]
	public delegate void ActiveAreaChangedEventHandler(int present, int minActiveTimeline, int maxActiveTimeline);
	
	
	public GameState gsm;
	public List<CoordFive> destinations;
	public CoordFive SelectedSquare;
	public List<Node> Arrows;
	public List<Node> TempArrows;
	public List<Node> CheckArrows;
	public Move PromotionMove;
	public int VisualPresent = 1;
	public int VisualMinL = 0;
	public int VisualMaxL = 0;
	
	public bool PromotionPanelShowing = false;
	public bool AnalysisMode = false;
	
	Node mvcontainer;
	
	public override void _Ready()
	{
		CheckArrows = new List<Node>();
		Arrows = new List<Node>();
		TempArrows = new List<Node>();
		GetNode("/root/VisualSettings").Connect("view_changed", new Callable(this, nameof(OnViewChanged)));
		GetNode("SubViewport/GameEscapeMenu/Button").Connect("pressed", new Callable(this, nameof(ExitGamePressed)));
		GetNode("SubViewport/Menus").Connect("submit_turn", new Callable(this,nameof(SubmitTurn)));
		GetNode("SubViewport/Menus").Connect("undo_turn", new Callable(this,nameof(UndoTurn)));
		GetNode("SubViewport/Menus").Connect("load_game", new Callable(this,nameof(OpenFileDialog)));
		GetNode("SubViewport/Menus").Connect("promotion_chosen", new Callable(this,nameof(FinishPromotionMove)));
		if(AnalysisMode){
			GetNode("SubViewport/Menus").Call("set_analysis_mode");
		}
		GetNode("FileDialog").Connect("file_selected", new Callable(this, nameof(LoadGame)));
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
		multiverse.Set("game_container",this);
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
				int piece = b.GetSquare(y,x); // no idea why this is inverted, but o well.
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
		CoordFive clicked = new CoordFive((int)square.X,(int)square.Y,(int)Temporalposition.Y,(int)Temporalposition.X,color);
		int piece = gsm.GetSquare(clicked);
		Console.WriteLine(clicked.ToString());
		if( destinations == null )
		{
			Console.WriteLine("Dest Null");
			if(ValidateClickSquare(new CoordFive(clicked,color)))
			{
				Console.WriteLine("Valid Click");
				GetDestinationsFromClick(square,Temporalposition,color);
			}
		}
		else if(destinations.Contains(clicked))
		{
			if(gsm.CoordIsPlayable(SelectedSquare))
			{
				piece = gsm.GetSquare(SelectedSquare);
				if(piece < 0){
					piece = piece * -1;
				}
				Move SelectedMove = new Move(SelectedSquare,clicked);
				if(piece == (int)Board.Piece.WPAWN || piece == (int)Board.Piece.BPAWN 
					|| piece == (int)Board.Piece.WBRAWN || piece == (int)Board.Piece.BBRAWN)
				{
					if(gsm.Color && clicked.Y == gsm.Height-1)
					{
						PromotionMove = SelectedMove;
						GetNode("SubViewport/Menus").Call("show_promotion");
						return;
					}
					else if(!gsm.Color && clicked.Y == 0)
					{
						PromotionMove = SelectedMove;
						GetNode("SubViewport/Menus").Call("show_promotion");
						return;
					} 
				}
				bool moveStatus = MakeMove(SelectedMove,color);
				if(moveStatus){
					CheckActiveArea();
					mvcontainer.Call("clear_highlights");
				}
			}
		}
		else{
			if(ValidateClickSquare(new CoordFive(clicked,color))){//TODO Check This
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
		destinations = gsm.GetPossibleDestinations(coord);
		Godot.Collections.Array DestinationsGodot = new Godot.Collections.Array();
		foreach( CoordFive cd in destinations ){
			DestinationsGodot.Add(new Vector4(cd.X,cd.Y,cd.L,cd.T));
		}
		if( mvcontainer != null){
			mvcontainer.Call("clear_highlights");
			mvcontainer.Call("highlight_squares",DestinationsGodot,color);
		}
	}
	
	public bool ValidateClickSquare(CoordFive cf){
		if(cf.Color != gsm.Color){
			return false;
		}
		if(!gsm.CoordIsPlayable(cf)){
			return false;
		}
		int piece = gsm.GetSquare(cf);
		Console.WriteLine(cf.ToString() + "Gets piece: " + piece.ToString());
		if(piece == 0){
			return false;
		}
		if(Board.GetColorBool(piece) != gsm.Color){
			return false;
		}
		return true;
	}
	
	public bool MakeMove(Move m, bool color)
	{
		bool moveStatus = gsm.MakeMove(m);
		if(moveStatus)
		{
			if(m.Type == 3 )
			{
				//Branching Move
				int Layer;
				if(color)
				{
					Layer = gsm.MaxTL;
				}
				else
				{
					Layer = gsm.MinTL;
				}
				Vector2 tile = new Vector2(Layer,m.Dest.T);
				if(!color)
				{
					tile.Y += 1;
				}
				Node a = CreateArrow(m,color);
				TempArrows.Add(a);
				AddChild(a);
				EmitSignal(SignalName.MoveMade, tile,!color);
				AddTimelineToRender(m,color);
			}
			else
			{
				Vector2 tile = new Vector2(m.Dest.L,m.Dest.T);
				if(!color)
				{
					tile.Y += 1;
				}
				Node a = CreateArrow(m,color);
				TempArrows.Add(a);
				AddChild(a);
				EmitSignal(SignalName.MoveMade, tile, !color);
				AddBoardToRender(m,color);
				CheckForChecks();
				return moveStatus;
			}
			CheckForChecks();
			//UpdateRender();//TODO change this from being here, might need to put elsewhere
			//need to add cleanup
		}
		return moveStatus;
	}
	
	//this adds a board instead of nuking everything.
	public void AddBoardToRender(Move m, bool color)
	{
		Vector2 newBoardPosition = new Vector2(m.Dest.L,m.Dest.T);
		CoordFive cfBoardPosition = new CoordFive(m.Dest,!color);
		if(!color){
			newBoardPosition.Y += 1;
			cfBoardPosition.T += 1;
		}
		Board b = gsm.GetBoard(cfBoardPosition);
		var arr = new Godot.Collections.Array();
		for(int x = 0; x < b.Width; x++){
			for(int y = 0; y < b.Height; y++){
				int piece = b.GetSquare(y,x); // no idea why this is inverted, but o well.
				if( piece < 0 ){
					piece *= -1;
				}
				arr.Add(piece);
			}	
		}
		mvcontainer.Call("add_board",arr,newBoardPosition,cfBoardPosition.Color);
		if(m.Type == 1){
			return;
		}
		newBoardPosition = new Vector2(m.Origin.L,m.Origin.T);
		cfBoardPosition = new CoordFive(m.Origin,!color);
		if(!color){
			newBoardPosition.Y += 1;
			cfBoardPosition.T += 1;
		}
		b = gsm.GetBoard(cfBoardPosition);
		arr = new Godot.Collections.Array();
		for(int x = 0; x < b.Width; x++){
			for(int y = 0; y < b.Height; y++){
				int piece = b.GetSquare(y,x); // no idea why this is inverted, but o well.
				if( piece < 0 ){
					piece *= -1;
				}
				arr.Add(piece);
			}	
		}
		mvcontainer.Call("add_board",arr,newBoardPosition,cfBoardPosition.Color);
	}
	
	public void AddTimelineToRender(Move m, bool color)
	{
		//Origin Board.
		Vector2 newBoardPosition = new Vector2(m.Origin.L,m.Origin.T);
		CoordFive cfBoardPosition = new CoordFive(m.Origin,!color);
		if(!color){
			newBoardPosition.Y += 1;
			cfBoardPosition.T += 1;
		}
		Board b = gsm.GetBoard(cfBoardPosition);
		var arr = new Godot.Collections.Array();
		for(int x = 0; x < b.Width; x++){
			for(int y = 0; y < b.Height; y++){
				int piece = b.GetSquare(y,x); // no idea why this is inverted, but o well.
				if( piece < 0 ){
					piece *= -1;
				}
				arr.Add(piece);
			}	
		}
		mvcontainer.Call("add_board",arr,newBoardPosition,cfBoardPosition.Color);
		int new_layer;
		if(color)
		{
			new_layer = gsm.MaxTL;
		}
		else
		{
			new_layer = gsm.MinTL;
		}
		Timeline new_timeline = gsm.GetTimeline(new_layer);
		Node timeline_node = TimeLineToGodotNodes(new_timeline,new_layer);
		mvcontainer.Call("add_timeline",timeline_node);
	}
	
	public void UpdateRender(){
		Console.WriteLine("nuking nodes");
		if(mvcontainer != null){
			mvcontainer.Call("queue_free");
		}if(gsm != null){
			GameStateToGodotNodes();
			GetNode("SubViewport/Menus").Call("set_turn_label",gsm.Color,gsm.Present);
		}
	}
	
	public void CheckForChecks()
	{
		foreach(Node child in CheckArrows){
			child.Call("queue_free");
		}
		CheckArrows.Clear();
		List<Move> moves = gsm.GetCurrentThreats();
		foreach(Move m in moves){
			Node a = CreateArrow(m,!gsm.Color, new Color(1,0,0,(float)0.5));
			AddChild(a);
			CheckArrows.Add(a);
		}
	}
	
	public void CheckActiveArea()
	{
		gsm.CalcPresent();
		var changed = false;
		if(gsm.Present != VisualPresent){
			changed = true;
			VisualPresent = gsm.Present;
		}
		if(gsm.MaxActiveTL != VisualMaxL){
			changed = true;
			VisualMaxL = gsm.MaxActiveTL;
		}
		if(gsm.MinActiveTL != VisualMinL){
			changed = true;
			VisualMinL = gsm.MinActiveTL;
		}
		if(changed){
			EmitSignal(SignalName.ActiveAreaChanged, VisualPresent,VisualMinL,VisualMaxL);
		}
	}
	
	public Vector2 GetPresentTile(){
		CoordFive cf = gsm.GetPresentCoordinate(0);
		return new Vector2(cf.L,cf.T);
	}
	
	public Node CreateArrow(Move m, bool color)
	{
		var arrow = ResourceLoader.Load<PackedScene>("res://Scenes/UI/arrow_draw.tscn").Instantiate();
		arrow.Set("origin",GameInterface.CoordFivetoCoord5(m.Origin,color));
		arrow.Set("dest",GameInterface.CoordFivetoCoord5(m.Dest,color));
		return arrow;
	}
	
	public Node CreateArrow(Move m, bool color, Color c)
	{
		var arrow = ResourceLoader.Load<PackedScene>("res://Scenes/UI/arrow_draw.tscn").Instantiate();
		arrow.Set("origin",GameInterface.CoordFivetoCoord5(m.Origin,color));
		arrow.Set("dest",GameInterface.CoordFivetoCoord5(m.Dest,color));
		arrow.Set("arrow_color",c);
		return arrow;
	}
	
	public void ClearTurnArrows()
	{
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
		if( SubmitSuccessful && gsm.IsMated()) {;
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
		CheckActiveArea();
		CheckForChecks();
		gsm.UndoTempMoves();
		UpdateRender();
	}
	
	//TODO clear check arrows.
	public void LoadGame(String filepath){
		Console.WriteLine("chose Path: " + filepath);
		gsm = FENParser.ShadSTDGSM(filepath);
		GetNode("/root/VisualSettings").Set("game_board_dimensions", new Vector2(gsm.Width,gsm.Height));
		GetNode("/root/VisualSettings").Call("change_game");
		UpdateRender();
		ClearTurnArrows();
		CheckActiveArea();
		EmitSignal(SignalName.GameLoaded);
		EmitSignal(SignalName.ActiveAreaChanged, VisualPresent,VisualMinL,VisualMaxL);
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
		//{ XXX not needed since drawn nodes pay attention to this.
			//arrow.Call("get_coordinate");
		//}
	}
	
	public void FinishPromotionMove( int piece )
	{
		if(PromotionMove == null)
		{
			return;
		}
		if(piece <= 1)
		{
			piece = 7;//troll the player if they choose pawn. (Doesn't work for white :( )
		}
		if(!gsm.Color)
		{
			piece += Board.NUMTYPES;
		}
		PromotionMove.SpecialType = piece;
		if(MakeMove(PromotionMove,gsm.Color))
		{
			CheckActiveArea();
			mvcontainer.Call("clear_highlights");
		}
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
