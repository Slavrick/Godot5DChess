extends Panel

signal square_clicked( square : Vector2, time : int, color : bool)
signal square_right_clicked( square : Coord5, pressed : bool)
signal square_hovered(square : Coord5)
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

var multiverse_position = Vector2.ZERO
var color := false
var board : Array

var packed_piece = load("res://Scenes/UI/piece.tscn")
var mouse_hovering = false
var highlighted_squares : Array
var annotation_highlights : Array
var mouse_square : HighLightedSquare

@onready var piecestext = load("res://Sprites/CrazyPenguins-5D-Chess-Set-0.1.0/pieces-Modified.png")
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
		var currHover = local_position_to_square(get_local_mouse_position())
		currHover.x = clamp(currHover.x,0,board_width-1)
		currHover.y = clamp(currHover.y,0,board_height-1)
		#We only care if the hovered square changed.
		if(!currHover.is_equal_approx(mouse_square.square)):
			mouse_square.square = currHover
			var hover = Coord5.Create(Vector4(mouse_square.square.x,mouse_square.square.y,multiverse_position.x,multiverse_position.y),color)
			square_hovered.emit(hover)
			queue_redraw()


func piece_local_position_white(rank : int, file : int) -> Vector2:
	return Vector2(margin + SQUARE_WIDTH * file, margin + SQUARE_WIDTH * (board_height - rank - 1))


func piece_local_position_black(rank : int, file : int) -> Vector2:
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


func annotate_square(square : Vector2, color : Color):
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


func unannotate_square(square):
	for i in range(annotation_highlights.size()):
		if annotation_highlights[i].square == square:
			annotation_highlights.remove_at(i)
			return
	queue_redraw()


func clear_highlights():
	highlighted_squares.clear()
	queue_redraw()


func clear_annotated_highlights():
	annotation_highlights.clear()
	queue_redraw()


func button_clicked( pos : Vector2 ):
	square_clicked.emit(pos,multiverse_position.y,color)


func _on_gui_input(event: InputEvent) -> void:
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_RIGHT:
			var mouse_pos = get_local_mouse_position()
			var right_click_square = local_position_to_square(mouse_pos)
			var c = Coord5.Create(Vector4(right_click_square.x,right_click_square.y,multiverse_position.x,multiverse_position.y),color)
			square_right_clicked.emit(c,event.pressed)
			print_debug(self.name)
			queue_redraw()
		if event.button_index == MOUSE_BUTTON_LEFT and event.pressed:
			var mouse_pos = get_local_mouse_position()
			button_clicked(local_position_to_square(mouse_pos))


func _on_mouse_entered() -> void:
	mouse_hovering = true


func _on_mouse_exited() -> void:
	mouse_hovering = false
	mouse_square.square = Vector2(-2,-2)
	queue_redraw() #so that the square no longer shows.
