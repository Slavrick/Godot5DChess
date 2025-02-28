extends Panel


@export var SQUARE_WIDTH := 10
@export var board_height := 8
@export var board_width := 8
@export var light_color := Color.BEIGE
@export var dark_color := Color.BLACK
@export var margin := 20

var board : Array
var packed_piece = load("res://Scenes/UI/piece.tscn")

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	board = [0,1,2,3,4,5,6,7,0,0,0,0,0,1,1,1,0,0,0,1,1,1]
	size = Vector2(SQUARE_WIDTH * board_width,SQUARE_WIDTH * board_height)
	size += Vector2(margin,margin) * 2
	load_board_array()


func place_children():
	for child in get_children():
		child.position = piece_local_position(child.rank,child.file)


func piece_local_position(rank, file):
	return Vector2(margin + SQUARE_WIDTH * file, margin + SQUARE_WIDTH * rank)


func load_board_array():
	for i in range(board.size()):
		if board[i] == 0:
			continue
		var file = i % board_width
		var rank = floor(i / board_width)
		var piece = packed_piece.instantiate()
		piece.piece_type = board[i]
		print(board[i])
		piece.rank = rank
		piece.file = file
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
