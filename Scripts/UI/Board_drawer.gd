extends Panel

signal square_clicked( square : Vector2, time : int, color : bool)
signal square_right_clicked( square : Vector2, time : int, color : bool, pressed : bool)
signal square_hovered(square : Coord5) #TODO Change this to coord class?
signal check_pressed
signal undo_pressed


enum TYPE {
	PRESENT,
	HISTORY,
	GHOST,
	INACTIVE,
}

@export_group("Dimensions")
@export var SQUARE_WIDTH := 128
@export var board_height := 8
@export var board_width := 8
@export var margin := 20
@export_group("Colors")
@export var light_color := Color.BEIGE
@export var dark_color := Color.BLACK
@export var white_style_box : StyleBoxFlat
@export var black_style_box : StyleBoxFlat
@export var white_present_style_box : StyleBoxFlat
@export var black_present_style_box : StyleBoxFlat
@export_group("Visual Settings")
@export var board_perspective := true
@export var board_type := TYPE.HISTORY:
	set(value):
		board_type = value
		if board_type == TYPE.PRESENT:
			if color:
				add_theme_stylebox_override("panel",white_present_style_box)
			else:
				add_theme_stylebox_override("panel",black_present_style_box)
		else:
			if color:
				add_theme_stylebox_override("panel",white_style_box)
			else:
				add_theme_stylebox_override("panel",black_style_box)
@export var in_check := false :
	set(value):
		if value:
			$InCheckButton.show()
		else:
			$InCheckButton.hide()
@export var temp_board := false :
	set(value):
		if value:
			$UndoButton.show()
		else:
			$UndoButton.hide()
var board : Array
@export var multiverse_position = Vector2.ZERO
@export var color := false
var highlighted_squares : Array
var packed_piece = load("res://Scenes/UI/piece.tscn")
var mouse_hovering = false
var mouse_square : HighLightedSquare
#
#@onready var pawntres = load("res://Resources/Pieces/Pawn.tres")
#@onready var piecestext = load("res://Resources/res/Pieces-hirez.png")
@onready var piecestext = load("res://Sprites/CrazyPenguins-5D-Chess-Set-0.1.0/pieces.png")
#Translates from what the array uses in my game manager to the positions of this.
const translation_dict = {
	0:-1,
	1:0,
	2:1,
	3:2,
	4:3,
	5:15,
	6:4,
	7:5,
	8:13,
	9:14,
	10:12,
	11:16,
	12:17,
	13:6,
	14:7,
	15:8,
	16:9,
	17:21,
	18:10,
	19:11,
	20:19,
	21:20,
	22:18,
	23:22,
	24:23,
	25:25,
	26:26,
	27:27
}

const piecedict = {
	0: Vector2(0,0), # Empty
	1: Vector2(512,0), #WPawn
	2: Vector2(1024,0), #WKnight
	3: Vector2(1536,0), 
	4: Vector2(2048,0),
	5: Vector2(2560,0),
	6: Vector2(0,500),
	7: Vector2(512,512),
	8: Vector2(1024,512),
	9: Vector2(1536,512),
	10: Vector2(2048,512),
	11: Vector2(2560,512), #WBrawn
	12: Vector2(0,1024),
	13: Vector2(512,1024),
	14: Vector2(1024,1024),
	15: Vector2(1536,1024),
	16: Vector2(2048,1024),
	17: Vector2(2560,1024),
	18: Vector2(0,1536),
	19: Vector2(512,1536),
	20: Vector2(1024,1536),
	21: Vector2(1536,1536),
	22: Vector2(2048,1536), #BUnicorn
	23: Vector2(2560,1536), #BDragon
}

#const translation_dict = {
	#0:0,
	#1:1,
	#2:2,
	#3:3,
	#4:4,
	#5:5,
	#6:6,
	#7:7,
	#8:8,
	#9:9,
	#10:11,
	#11:12,
	#12:13,
	#13:14,
	#14:15,
	#15:16,
	#16:17,
	#17:18,
	#18:19,
	#19:20,
	#20:22,
	#21:23,
	#22:24,
	#23:25,
	#24:26,
	#25:25,
	#26:26,
	#27:27
#}
#const piecedict = {
	#0: Vector2(0,0), # Empty
	#1: Vector2(128,0), #WPawn
	#2: Vector2(256,0), #WKnight
	#3: Vector2(384,0), 
	#4: Vector2(512,0),
	#5: Vector2(640,0),
	#6: Vector2(768,0),
	#7: Vector2(896,0),
	#8: Vector2(1024,0),
	#9: Vector2(1152,0),
	#10: Vector2(1280,0),
	#11: Vector2(0,128), #WBrawn
	#12: Vector2(128,128),
	#13: Vector2(256,128),
	#14: Vector2(384,128),
	#15: Vector2(512,128),
	#16: Vector2(640,128),
	#17: Vector2(768,128),
	#18: Vector2(896,128),
	#19: Vector2(1024,128),
	#20: Vector2(1152,128),
	#21: Vector2(1280,128),
	#22: Vector2(0,256), #BUnicorn
	#23: Vector2(128,256), #BDragon
	#24: Vector2(256,256),
	#25: Vector2(384,256),
	#26: Vector2(512,256),
#}


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	mouse_square = HighLightedSquare.new()
	mouse_square.highlight_color = Color(0.1,0.9,0.1,0.6)
	board_perspective = VisualSettings.perspective
	light_color = VisualSettings.light_square_color
	dark_color = VisualSettings.dark_square_color
	board_width = VisualSettings.game_board_dimensions.x
	board_height = VisualSettings.game_board_dimensions.y
	size = Vector2(SQUARE_WIDTH * board_width,SQUARE_WIDTH * board_height) + Vector2(margin,margin) * 2
	$UndoButton.pressed.connect(func():
		undo_pressed.emit()
	)
	$InCheckButton.pressed.connect(func():
		check_pressed.emit()
	)
	#load_board_array() Slated for removal need to re_add highlighting when mousing over.
	if board_type == TYPE.PRESENT:
		if color:
			add_theme_stylebox_override("panel",white_present_style_box)
		else:
			add_theme_stylebox_override("panel",black_present_style_box)
	else:
		if color:
			add_theme_stylebox_override("panel",white_style_box)
		else:
			add_theme_stylebox_override("panel",black_style_box)


func _process(delta: float) -> void:
	if mouse_hovering:
		#TODO only emit when it changed. also nullify the square when mouse exits.
		mouse_square.square = local_position_to_square(get_local_mouse_position())
		mouse_square.square.x = clamp(mouse_square.square.x,0,board_width-1)
		mouse_square.square.y = clamp(mouse_square.square.y,0,board_height-1)
		var hover = Coord5.Create(Vector4(mouse_square.square.x,mouse_square.square.y,multiverse_position.x,multiverse_position.y),color)
		square_hovered.emit(hover)
		queue_redraw()


func piece_local_position_white(rank, file):
	return Vector2(margin + SQUARE_WIDTH * file, margin + SQUARE_WIDTH * (board_height - rank - 1))


func piece_local_position_black(rank, file):
	return Vector2(margin + SQUARE_WIDTH * (board_width - file - 1), margin + SQUARE_WIDTH * rank)


func local_position_to_square( local_pos ) -> Vector2:
	var file
	var rank
	if board_perspective:
		file = floor((local_pos.x - margin) / SQUARE_WIDTH)
		rank = board_height - floor((local_pos.y - margin) / SQUARE_WIDTH) - 1
	else:
		file = board_width - floor((local_pos.x - margin) / SQUARE_WIDTH)  - 1
		rank = floor((local_pos.y - margin) / SQUARE_WIDTH)
	return Vector2(file,rank)


#XXX Slated for removal
#func place_children():
	#return
	#for child in get_children():
		#if board_perspective :
			#child.position = piece_local_position_white(child.rank,child.file)
		#else:
			#child.position = piece_local_position_black(child.rank,child.file)


#func load_board_array(): XXX slated for removal, 
	#for i in range(board.size()):
		#if board[i] == 0:
			#continue
		#var file = i % board_width
		#var rank = floor(i / board_width)
		#var piece = packed_piece.instantiate()
		#piece.piece_type = board[i]
		#piece.rank = rank
		#piece.file = file
		#piece.pressed.connect(button_clicked.bind(Vector2(file,rank)))
		#piece.piece_right_clicked.connect(highlight_square.bind(Vector2(file,rank),Color.INDIAN_RED))
		#add_child(piece)
	#place_children()


func _draw() -> void:
	#TODO can potentially just draw a big initial square and fill in the dark spots to optimize.
	for y in range(board_height):
		for x in range(board_width):
			var rect = Rect2(margin + x * SQUARE_WIDTH, margin + y * SQUARE_WIDTH,SQUARE_WIDTH,SQUARE_WIDTH)
			if (x + y) % 2 == 0:
				draw_rect(rect,light_color,true)
			else:
				draw_rect(rect,dark_color,true)
	if board_perspective:
		for i in range(board.size()):
			if(board[i] == -1):
				continue
			var file = i % board_width
			var rank = floor(i / board_width)
			draw_texture_rect_region(piecestext,Rect2(piece_local_position_white(rank,file),Vector2(128,128)),Rect2(piecedict[board[i]],Vector2(500,500)))
	else:
		for i in range(board.size()):
			if(board[i] == -1):
				continue
			var file = i % board_width
			var rank = floor(i / board_width)
			draw_texture_rect_region(piecestext,Rect2(piece_local_position_black(rank,file),Vector2(128,128)),Rect2(piecedict[board[i]],Vector2(500,500)))
	for highlight in highlighted_squares:
		if board_perspective:
			var rect = Rect2(margin + highlight.square.x * SQUARE_WIDTH, margin + (board_height - highlight.square.y - 1) * SQUARE_WIDTH,SQUARE_WIDTH,SQUARE_WIDTH)
			draw_rect(rect,highlight.highlight_color,true)
		else:
			var rect = Rect2(margin + (board_width - highlight.square.x - 1) * SQUARE_WIDTH, margin + highlight.square.y * SQUARE_WIDTH,SQUARE_WIDTH,SQUARE_WIDTH)
			draw_rect(rect,highlight.highlight_color,true)
	if board_type == TYPE.GHOST:
		self_modulate = Color(1,1,1,.75)
	if board_type == TYPE.INACTIVE:
		self_modulate = Color(.67,.67,.67,1)
	if mouse_hovering:
		if board_perspective:
			var rect = Rect2(margin + mouse_square.square.x * SQUARE_WIDTH, margin + (board_height - mouse_square.square.y - 1) * SQUARE_WIDTH,SQUARE_WIDTH,SQUARE_WIDTH)
			draw_rect(rect,mouse_square.highlight_color,true)
		else:
			var rect = Rect2(margin + (board_width - mouse_square.square.x - 1) * SQUARE_WIDTH, margin + mouse_square.square.y * SQUARE_WIDTH,SQUARE_WIDTH,SQUARE_WIDTH)
			draw_rect(rect,mouse_square.highlight_color,true)


func logicalBoardToUIBoard():
	for i in range(board.size()):
		board[i] = translation_dict[board[i]]


func functional_width() -> float:
	return SQUARE_WIDTH * board_width + margin * 2


func functional_height() -> float:
	return SQUARE_WIDTH * board_height + margin * 2


func highlight_square(square : Vector2, color : Color):
	color.a = .5
	var new_highlight = HighLightedSquare.new()
	new_highlight.square = square
	new_highlight.highlight_color = color
	highlighted_squares.append(new_highlight)
	queue_redraw()

func unhighlight_square(square):
	for i in range(highlighted_squares.size()):
		if highlighted_squares[i].square == square:
			highlighted_squares.remove_at(i)
			return
	queue_redraw()


func clear_highlights():
	highlighted_squares.clear()
	queue_redraw()


func button_clicked( pos : Vector2 ):
	square_clicked.emit(pos,multiverse_position.y,color)


func _on_gui_input(event: InputEvent) -> void:
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_RIGHT and event.pressed:
			var mouse_pos = get_local_mouse_position()
			highlight_square(local_position_to_square(mouse_pos),Color.GOLD)
			queue_redraw()
		if event.button_index == MOUSE_BUTTON_LEFT and event.pressed:
			var mouse_pos = get_local_mouse_position()
			button_clicked(local_position_to_square(mouse_pos))


func _on_mouse_entered() -> void:
	mouse_hovering = true


func _on_mouse_exited() -> void:
	mouse_hovering = false
	queue_redraw() #so that the square no longer shows.
