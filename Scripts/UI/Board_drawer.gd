extends Panel

signal square_clicked( square : Vector2, time : int, color : bool)

enum PERSPECTIVE {
	BLACK,
	WHITE
}

enum TYPE {
	PRESENT,
	HISTORY,
	GHOST,
	INACTIVE,
}

@export_group("dimensions")
@export var SQUARE_WIDTH := 128
@export var board_height := 8
@export var board_width := 8
@export var margin := 20
@export_group("colors")
@export var light_color := Color.BEIGE
@export var dark_color := Color.BLACK
@export var black_style_box : StyleBoxFlat
@export var white_style_box : StyleBoxFlat
@export_group("visual_settings")
@export var board_perspective := PERSPECTIVE.WHITE
@export var board_type := TYPE.PRESENT

var board : Array
@export var multiverse_position = Vector2.ZERO
@export var color := false
var highlighted_squares : Array
var packed_piece = load("res://Scenes/UI/piece.tscn")

#Translates from what the array uses in my game manager to the positions of this.
const translation_dict = {
	0:0,
	1:1,
	2:2,
	3:3,
	4:4,
	5:5,
	6:6,
	7:7,
	8:8,
	9:9,
	10:10,
	11:11,
	12:12,
	13:14,
	14:15,
	15:16,
	16:17,
	17:18,
	18:19,
	19:20,
	20:21,
	21:21,
	22:22,
	23:23,
	24:24,
	25:25,
	26:26,
	27:27
}

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	size = Vector2(SQUARE_WIDTH * board_width,SQUARE_WIDTH * board_height)
	size += Vector2(margin,margin) * 2
	load_board_array()
	if color:
		add_theme_stylebox_override("panel",white_style_box)
	else:
		add_theme_stylebox_override("panel",black_style_box)

func place_children():
	for child in get_children():
		if(board_perspective == PERSPECTIVE.WHITE):
			child.position = piece_local_position_white(child.rank,child.file)
		else:
			child.position = piece_local_position_black(child.rank,child.file)

func piece_local_position(rank, file):
	return Vector2(margin + SQUARE_WIDTH * file, margin + SQUARE_WIDTH * rank)


func piece_local_position_white(rank, file):
	return Vector2(margin + SQUARE_WIDTH * file, margin + SQUARE_WIDTH * (board_height - rank - 1))

func piece_local_position_black(rank, file):
	return Vector2(margin + SQUARE_WIDTH * (board_width - file - 1), margin + SQUARE_WIDTH * rank)

func load_board_array():
	for i in range(board.size()):
		if board[i] == 0:
			continue
		var file = i % board_width
		var rank = floor(i / board_width)
		var piece = packed_piece.instantiate()
		piece.piece_type = board[i]
		piece.rank = rank
		piece.file = file
		piece.pressed.connect(button_clicked.bind(Vector2(file,rank)))
		piece.piece_right_clicked.connect(highlight_square.bind(Vector2(file,rank),Color.INDIAN_RED))
		add_child(piece)
	place_children()


func _draw() -> void:
	for y in range(board_height):
		for x in range(board_width):
			var rect = Rect2(margin + x * SQUARE_WIDTH, margin + y * SQUARE_WIDTH,SQUARE_WIDTH,SQUARE_WIDTH)
			if (x + y) % 2 == 0:
				draw_rect(rect,light_color,true)
			else:
				draw_rect(rect,dark_color,true)
	for highlight in highlighted_squares:
		if(board_perspective == PERSPECTIVE.WHITE):
			var rect = Rect2(margin + highlight.square.x * SQUARE_WIDTH, margin + (board_height - highlight.square.y - 1) * SQUARE_WIDTH,SQUARE_WIDTH,SQUARE_WIDTH)
			draw_rect(rect,highlight.highlight_color,true)
		else:
			var rect = Rect2(margin + (board_width - highlight.square.x - 1) * SQUARE_WIDTH, margin + highlight.square.y * SQUARE_WIDTH,SQUARE_WIDTH,SQUARE_WIDTH)
			draw_rect(rect,highlight.highlight_color,true)


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


func local_position_to_square( local_pos ) -> Vector2:
	var file
	var rank
	if board_perspective == PERSPECTIVE.WHITE:
		file = floor((local_pos.x - margin) / SQUARE_WIDTH)
		rank = board_height - floor((local_pos.y - margin) / SQUARE_WIDTH) - 1
	else:
		file = board_width - floor((local_pos.x - margin) / SQUARE_WIDTH)  - 1
		rank = floor((local_pos.y - margin) / SQUARE_WIDTH)
	return Vector2(file,rank)
