extends Panel


@export var SQUARE_WIDTH := 10
@export var board_height := 8
@export var board_width := 8
@export var light_color := Color.BEIGE
@export var dark_color := Color.BLACK
@export var margin := 20
@export var multiverse_position = Vector2.ZERO
@export var color := false

var board : Array
var packed_piece = load("res://Scenes/UI/piece.tscn")


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


func logicalBoardToUIBoard():
	for i in range(board.size()):
		board[i] = translation_dict[board[i]]


func functional_width() -> float:
	return SQUARE_WIDTH * board_width + margin * 2


func functional_height() -> float:
	return SQUARE_WIDTH * board_height + margin * 2


func highlight_square():
	pass
