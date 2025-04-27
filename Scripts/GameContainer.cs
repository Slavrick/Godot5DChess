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
	
	public GameStateManager gsm;
	
	public List<CoordFive> Destinations;
	public List<CoordFive> HoveredDestinations;
	public CoordFive HoveredSquare;
	public CoordFive SelectedSquare;
	public Coord5 RightClickStart;
	public Move PromotionMove;

	public List<Node> Arrows;
	public List<Node> TempArrows;
	public List<Node> CheckArrows;
	public List<Node> AnnotationArrows;
	public List<int> AnnotationArrowColors;
	public List<Coord5> AnnotationSquares;
	public List<int> AnnotationSquareColors;

	public int VisualPresent = 1;
	public int VisualMinL = 0;
	public int VisualMaxL = 0;
	
	public bool PromotionPanelShowing = false;
	public bool AnalysisMode = false;
	
	Node mvcontainer;
	
	public override void _Ready()
	{
		Arrows = new List<Node>();
		TempArrows = new List<Node>();
		CheckArrows = new List<Node>();
		AnnotationArrows = new List<Node>();
		AnnotationArrowColors = new List<int>();
		AnnotationSquares = new List<Coord5>();
		AnnotationSquareColors = new List<int>();
		GetNode("/root/VisualSettings").Connect("view_changed", new Callable(this, nameof(OnViewChanged)));
		GetNode("SubViewport/GameEscapeMenu/Button").Connect("pressed", new Callable(this, nameof(ExitGamePressed)));
		GetNode("SubViewport/Menus").Connect("submit_turn", new Callable(this,nameof(SubmitTurn)));
		GetNode("SubViewport/Menus").Connect("undo_turn", new Callable(this,nameof(UndoTurn)));
		GetNode("SubViewport/Menus").Connect("promotion_chosen", new Callable(this,nameof(FinishPromotionMove)));
		if(AnalysisMode){
			GetNode("SubViewport/Menus").Call("set_analysis_mode");
			GetNode("SubViewport/Menus").Connect("load_game", new Callable(this,nameof(OpenFileDialog)));
			GetNode("SubViewport/Menus").Connect("save_game", new Callable(this,nameof(OpenSaveDialog)));
			GetNode("FileDialog").Connect("file_selected", new Callable(this, nameof(LoadGame)));
			GetNode("SaveDialog").Connect("file_selected", new Callable(this, nameof(SaveGame)));
			GetNode("SubViewport/Menus").Connect("turntree_item_selected", new Callable(this,nameof(OnTurnTreeSelected)));
			GetNode("SubViewport/Menus").Connect("annotation_changed", new Callable(this,nameof(SaveAnnotation)));
		}
	}
	
	public void GameStateToGodotNodes()
	{
		var multiverse = ResourceLoader.Load<PackedScene>("res://Scenes/UI/multiverse_container.tscn").Instantiate();
		multiverse.Set("chessboard_dimensions",new Vector2(gsm.Width,gsm.Height));
		for(int i = 0; i < gsm.Multiverse.Count; i++){
			multiverse.AddChild(TimeLineToGodotNodes(gsm.Multiverse[i],gsm.MinTL+i));
		}
		multiverse.Connect("square_clicked", new Callable(this,nameof(HandleClick)));
		multiverse.Connect("square_hovered", new Callable(this,nameof(OnSquareHovered)));
		//multiverse.Connect("square_right_clicked", new Callable(this,nameof(HandleRightClick)));
		multiverse.Connect("show_timeline_check", new Callable(this,nameof(ShowCheck)));
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
		if( Destinations == null )
		{
			if(ValidateClickSquare(new CoordFive(clicked,color)))
			{
				GetDestinationsFromClick(square,Temporalposition,color);
			}
		}
		else if(Destinations.Contains(clicked))
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
		int piece = gsm.GetSquare(coord);
		if(piece == 0){
			return;
		}
		Destinations = gsm.GetPossibleDestinations(coord);
		Godot.Collections.Array DestinationsGodot = new Godot.Collections.Array();
		foreach( CoordFive cd in Destinations ){
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
				Node a = CreateArrow(m);
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
				Node a = CreateArrow(m);
				TempArrows.Add(a);
				AddChild(a);
				EmitSignal(SignalName.MoveMade, tile, !color);
				AddBoardToRender(m,color);
			}
			//TODO this function can maybe be shorter
		}
		CheckForChecks();
		SelectedSquare = null;
		Destinations = null;
		return moveStatus;
	}

	public void HandleRightClick(Coord5 square, bool pressed)
	{
		if(pressed)
		{
			RightClickStart = square;
			return;
		}
		else
		{
			if(RightClickStart.Equals(square))
			{
				for(int i = 0; i < AnnotationSquares.Count ; i++)
				{
					if(square.Equals(AnnotationSquares[i]))
					{
						AnnotationSquares.RemoveAt(i);
						AnnotationSquareColors.RemoveAt(i);
						mvcontainer.Call("annotate_unhighlight_square",square);
						return;
					}
				}
				int color = 1;
				Color highlight_color = new Color(0,0,1,(float).5);
				if(Input.IsActionPressed("ThreatAnnotation"))
				{
					color = 0;
					highlight_color = new Color(1,0,0,(float).5);
				}
				else if(Input.IsActionPressed("CautionAnnotation"))
				{
					color = 4;
					highlight_color = new Color(1,(float).5,0,(float).5);
				}
				else if(Input.IsActionPressed("BrilliantAnnotation"))
				{
					color = 2;
					highlight_color = new Color(0,(float).4,(float).4,(float).5);
				}
				AnnotationSquares.Add(square);
				AnnotationSquareColors.Add(color);
				mvcontainer.Call("annotate_highlight_square",square,highlight_color);
			}
			else
			{
				int color = 1;
				Node a = CreateArrowCoord5(RightClickStart, square, new Color(0,0,1,(float).5));
				for(int i = 0 ; i < AnnotationArrows.Count; i++)
				{
					if((bool)a.Call("equals",AnnotationArrows[i]))
					{
						AnnotationArrows[i].Call("queue_free");
						AnnotationArrows.RemoveAt(i);
						AnnotationArrowColors.RemoveAt(i);
						return;
					}
				}
				if(Input.IsActionPressed("ThreatAnnotation"))
				{
					color = 0;
					a.Set("arrow_color",new Color(1,0,0,(float).5));
				}
				if(Input.IsActionPressed("CautionAnnotation"))
				{
					color = 4;
					a.Set("arrow_color",new Color(1,(float).5,0,(float).5));
				}
				if(Input.IsActionPressed("BrilliantAnnotation"))
				{
					color = 2;
					a.Set("arrow_color",new Color(0,(float).4,(float).4,(float).5));
				}
				AnnotationArrows.Add(a);
				AnnotationArrowColors.Add(color);
				AddChild(a);
			}
		}
	}
	
	//this adds a board instead of nuking everything.
	public void AddBoardToRender(Move m, bool color)//TODO need to make this so it doesn't need parameter color.
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
	
	public void AddTimelineToRender(Move m, bool color)//TODO need to make this so it doesn't need parameter color.
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
		if(mvcontainer != null){
			mvcontainer.Call("queue_free");
		}if(gsm != null){
			GameStateToGodotNodes();
			GetNode("SubViewport/Menus").Call("set_turn_label",gsm.Color,gsm.Present);
		}
	}
	
	public void CheckForChecks()
	{
		foreach(Node child in CheckArrows)
		{
			child.Call("queue_free");
		}
		CheckArrows.Clear();
		List<Move> moves = gsm.GetCurrentThreats();
		Godot.Collections.Array<int> indicators = new Godot.Collections.Array<int>();
		foreach(Move m in moves)
		{
			if(gsm.IsInBounds(m.Origin) && gsm.IsInBounds(m.Dest))
			{
				Node a = CreateArrow(m, new Color(1,0,0,(float)0.5));
				AddChild(a);
				CheckArrows.Add(a);
				continue;
			}
			if(!gsm.IsInBounds(m.Origin))
			{
				indicators.Add(m.Origin.L);
			}
			if(!gsm.IsInBounds(m.Dest))
			{
				indicators.Add(m.Dest.L);
			}
		}
		mvcontainer.Call("set_check_indicators",indicators);
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
	
	public Godot.Collections.Array<String> GetLinearLabels()
	{
		List<String> labels = AnnotationTree.GetLabels(gsm.ATR.Root);
		return new Godot.Collections.Array<String>(labels);
	}

	public Node CreateArrow(Move m)
	{
		var arrow = ResourceLoader.Load<PackedScene>("res://Scenes/UI/arrow_draw.tscn").Instantiate();
		arrow.Set("origin",GameInterface.CoordFivetoGD(m.Origin));
		arrow.Set("dest",GameInterface.CoordFivetoGD(m.Dest));
		return arrow;
	}

	public Node CreateArrowCoord5(Coord5 origin, Coord5 dest, Color c)
	{
		var arrow = ResourceLoader.Load<PackedScene>("res://Scenes/UI/arrow_draw.tscn").Instantiate();
		arrow.Set("origin",origin);
		arrow.Set("dest",dest);
		arrow.Set("arrow_color",c);
		return arrow;
	}
	
	public Node CreateArrow(Move m, Color c)
	{
		var arrow = ResourceLoader.Load<PackedScene>("res://Scenes/UI/arrow_draw.tscn").Instantiate();
		arrow.Set("origin",GameInterface.CoordFivetoGD(m.Origin));
		arrow.Set("dest",GameInterface.CoordFivetoGD(m.Dest));
		arrow.Set("arrow_color",c);
		return arrow;
	}
	
	public void RefreshMoveArrows()
	{
		ClearTurnArrows();
		List<AnnotatedTurn> turnList = AnnotationTree.GetPastTurns(gsm.Index);
		for(int i = 0 ; i < turnList.Count; i++)
		{
			foreach(Move m in turnList[i].T.Moves)
			{
				Node arrow = CreateArrow(m);
				Arrows.Add(arrow);
				AddChild(arrow);
			}
		}
	}

	public void ClearTurnArrows()
	{
		foreach(Node child in Arrows){
			child.Call("queue_free");
		}
		Arrows.Clear();
	}
	
	public void LoadTurnAnalysis()
	{
		Console.WriteLine(string.Join(", ", gsm.Index.AT.Arrows));
		Console.WriteLine(string.Join(", ", gsm.Index.AT.ArrowColors));
		for(int i = 0; i < gsm.Index.AT.Highlights.Count; i++)
		{
			CoordFive cf =  gsm.Index.AT.Highlights[i];
			Coord5 annotation = GameInterface.CoordFivetoGD(cf);
			AnnotationSquares.Add(annotation);
			Color c = new Color(1,0,0,(float).5);
			switch(gsm.Index.AT.HighlightColors[i])
			{
				case 1:
					c = new Color(0,1,0,(float).5);
					break;
				case 2:
					c = new Color(0,(float).4,(float).4,(float).5);
					break;
				case 3:
					c = new Color(1,1,0,(float).5);
					break;
				case 4:
					c = new Color(1,(float).5,0,(float).5);
					break;
			}
			mvcontainer.Call("annotate_highlight_square",annotation,c);
		}
		for(int i = 0; i < gsm.Index.AT.Arrows.Count; i++)
		{
			Move m = gsm.Index.AT.Arrows[i];
			Color c = new Color(1,0,0,(float).5);
			switch(gsm.Index.AT.ArrowColors[i])
			{
				case 1:
					c = new Color(0,1,0,(float).5);
					break;
				case 2:
					c = new Color(0,(float).4,(float).4,(float).5);
					break;
				case 3:
					c = new Color(1,1,0,(float).5);
					break;
				case 4:
					c = new Color(1,(float).5,0,(float).5);
					break;
			}
			Node n = CreateArrow(m,c);
			AddChild(n);
			AnnotationArrows.Add(n);
		}
		Console.WriteLine(string.Join(", ", gsm.Index.AT.Highlights));
		Console.WriteLine(string.Join(", ", gsm.Index.AT.HighlightColors));
		AnnotationSquareColors = new List<int>(gsm.Index.AT.HighlightColors);
		AnnotationArrowColors = new List<int>(gsm.Index.AT.ArrowColors);
		GetNode("SubViewport/Menus").Call("set_annotation_text", gsm.Index.AT.Annotation);
	}

	public void SaveTurnAnalysis()
	{
		Console.WriteLine(string.Join(", ", AnnotationSquares));
		Console.WriteLine(string.Join(", ", AnnotationSquareColors));
		gsm.Index.AT.HighlightColors = new List<int>(AnnotationSquareColors);
		gsm.Index.AT.Highlights.Clear();
		foreach(Coord5 c in AnnotationSquares)
		{
			gsm.Index.AT.Highlights.Add(GameInterface.C5toCoordFive(c)); 
		}
		gsm.Index.AT.ArrowColors = new List<int>(AnnotationArrowColors);
		gsm.Index.AT.Arrows.Clear();
		foreach(Node n in AnnotationArrows)
		{
			gsm.Index.AT.Arrows.Add(GameInterface.ArrowToMove(n));
		}
	}

	public void SaveAnnotation(string annotation)
	{
		gsm.Index.AT.Annotation = annotation;
	}


	public void SubmitTurn(){
		if(AnalysisMode)
		{
			SaveTurnAnalysis();
		}
		bool SubmitSuccessful = gsm.SubmitMoves();
		if( SubmitSuccessful ){
			EmitSignal(SignalName.TurnChanged, gsm.Color, gsm.Present);
			Arrows.AddRange(TempArrows);
			TempArrows.Clear();
			//Handle Annotation TODO
			foreach(Node n in AnnotationArrows)
			{
				n.Call("queue_free");
			}
			AnnotationArrows.Clear();
			AnnotationArrowColors.Clear();
			foreach(Coord5 c in AnnotationSquares)
			{
				mvcontainer.Call("annotate_unhighlight_square",c);
			}
			AnnotationSquares = new List<Coord5>();
			AnnotationSquareColors = new List<int>();
		}
		if( SubmitSuccessful && gsm.IsMated()) {;
			EmitSignal(SignalName.IsMated, gsm.Color);
		}
		Destinations = null;
		CheckForChecks();
		GetNode("SubViewport/Menus").Call("set_turn_label",gsm.Color,gsm.Present);//This is awful
		if(AnalysisMode)
		{
			LoadTurnAnalysis();
		}
	}
	
	public void UndoTurn(){
		foreach(Node n in TempArrows){
			n.Call("queue_free");
		}
		TempArrows.Clear();
		Godot.Collections.Array<int> arr = new Godot.Collections.Array<int>();
		int[] turnTLS = gsm.TurnTLS.ToArray();
		foreach( int i in turnTLS )
		{
			arr.Add(i);
		}
		mvcontainer.Call("undo_moves",arr);
		gsm.UndoTempMoves();
		CheckActiveArea();
		CheckForChecks();
	}
	
	//TODO clear check arrows.
	public void LoadGame(String filepath)
	{
		if(filepath.Contains("PGN5S"))
		{
						Console.WriteLine("Got HERE");
			gsm = FENParser.Parse5dStudy(filepath);
		}else
		{
			gsm = FENParser.ShadSTDGSM(filepath);
		}
		GetNode("/root/VisualSettings").Set("game_board_dimensions", new Vector2(gsm.Width,gsm.Height));
		GetNode("/root/VisualSettings").Call("change_game");
		UpdateRender();
		RefreshMoveArrows();
		CheckActiveArea();
		CheckForChecks();
		EmitSignal(SignalName.GameLoaded);
		EmitSignal(SignalName.ActiveAreaChanged, VisualPresent,VisualMinL,VisualMaxL);
	}

	public void SaveGame(String filepath)
	{
		if(filepath.Contains(".PGN5S"))
		{
			FENExporter.ExportString(filepath, FENExporter.ExportAnalysisGame(gsm));
		}
		else
		{
			FENExporter.ExportGameState(gsm, filepath);
		}
	}
	
	public void OpenFileDialog(){
		GetNode("FileDialog").Call("show");
	}
	
	public void OpenSaveDialog(){
		GetNode("SaveDialog").Call("show");
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

	public void ShowCheck(int layer)
	{
		Move nullMove = gsm.MakePassingMove(layer);
		if(nullMove == null) return;
		AddBoardToRender(nullMove, nullMove.Origin.Color);
		CheckForChecks();
		//TODO make a variable that marks if the layer is made a passing move.
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
	
	public void OnSquareHovered(Coord5 c)
	{
		if(SelectedSquare != null){
			return;
		}
		CoordFive hover = GameInterface.C5toCoordFive(c);
		if( HoveredSquare == null || !hover.Equals(HoveredSquare)){
			HoveredSquare = hover;
			//TODO possibly make it so that you can only hover your own pieces.
			int piece = gsm.GetSquare(HoveredSquare);
			if(piece == Board.ERRORSQUARE || piece == Board.EMPTYSQUARE){
				mvcontainer.Call("clear_highlights");
				return;
			}
			HoveredDestinations = MoveGenerator.GetMoves(piece,gsm,HoveredSquare);
			Godot.Collections.Array DestinationsGodot = new Godot.Collections.Array();
			foreach( CoordFive cd in HoveredDestinations ){
				DestinationsGodot.Add(new Vector4(cd.X,cd.Y,cd.L,cd.T));
			}
			if( mvcontainer != null){
				mvcontainer.Call("clear_highlights");
				mvcontainer.Call("highlight_squares",DestinationsGodot,HoveredSquare.Color);
			}
		}
	}

	public void OnTurnTreeSelected(int index)
	{
		SaveTurnAnalysis();
		foreach(Node n in AnnotationArrows)
		{
			n.Call("queue_free");
		}
		AnnotationArrows.Clear();
		AnnotationArrowColors.Clear();
		foreach(Coord5 c in AnnotationSquares)
		{
			mvcontainer.Call("annotate_unhighlight_square",c);
		}
		AnnotationSquares = new List<Coord5>();
		AnnotationSquareColors = new List<int>();
		gsm.NavigateToTurn(index);
		UpdateRender();
		LoadTurnAnalysis();
		RefreshMoveArrows();
		CheckActiveArea();
		CheckForChecks();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if(mouseEvent.ButtonIndex == MouseButton.Right)
			{
				Coord5 c = (Coord5)GetNode("/root/VisualSettings").Call("position_to_coordinate", GetGlobalMousePosition());
				bool pressed = mouseEvent.Pressed;
				HandleRightClick(c,pressed);
			}
			//Coord5 coord = 
		}
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
