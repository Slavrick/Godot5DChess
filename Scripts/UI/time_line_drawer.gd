extends Control

signal square_clicked(square : Vector2, temporal_position : Vector2, color : bool)

enum DRAWMODE {
	FULL,
	WHITE,
	BLACK
}

@export var layer := 0
@export var board_margin := 20
@export var draw_mode := DRAWMODE.FULL
var chessboard_dimensions : Vector2

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	place_children()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func place_children():
	match draw_mode:
		DRAWMODE.WHITE:
			return
		DRAWMODE.BLACK:
			return
		_:
			pass
	for child in get_children():
		var ply = child.multiverse_position.y - 1
		if child.color:
			ply *= 2
		else:
			ply *= 2
			ply += 1
		child.square_clicked.connect(board_square_clicked)
		child.position = Vector2(ply,0) * Vector2(child.functional_width() + board_margin,child.functional_height() + board_margin)


func _draw():
	var width = 0
	for child in get_children():
		width += child.functional_width()
	
	draw_rect(Rect2(0,0,width,20),Color.PURPLE,true)



func board_square_clicked(square ,time, color):
	square_clicked.emit(square,Vector2(layer,time),color)


func highlight_square(square : Vector4, color : bool ):
	get_board(square.w, color).highlight_square(Vector2(square.x,square.y),Color.MEDIUM_SLATE_BLUE)


func clear_highlights():
	for child in get_children():
		child.clear_highlights()


func get_board( time : int , color : bool ) -> Node:
	for child in get_children():
		if child.multiverse_position.y == time and child.color == color:
			return child
	return null
