extends Control

signal square_clicked(square : Vector2, temporal_position : Vector2, color : bool)

var highlighted_squares = []
var chessboard_dimensions : Vector2


func _ready() -> void:
	place_timelines()


func _process(delta: float) -> void:
	pass


func highlight_squares(squares : Array, color : bool):
	for square in squares:
		square as Vector4
		get_timeline_layer(square.z).highlight_square(square,color)


func clear_highlights():
	for child in get_children():
		child.clear_highlights()


func place_timelines():
	for child in get_children():
		child.position.y = child.layer * 1500
		child.square_clicked.connect(timeline_square_clicked)


func get_timeline_layer(layer : int) -> Node:
	for child in get_children():
		if child.layer == layer:
			return child
	return null


func timeline_square_clicked(square : Vector2 , temporal_position : Vector2, color : bool ):
	square_clicked.emit(square,temporal_position,color)
	print_debug(square,temporal_position,color)
