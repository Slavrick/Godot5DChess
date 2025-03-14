extends Control

signal square_clicked(square : Vector2, temporal_position : Vector2, color : bool)

var highlighted_squares = []
var chessboard_dimensions : Vector2
var perspective = true

func _ready() -> void:
	perspective = VisualSettings.perspective
	connect_signals()
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


func connect_signals():
	for child in get_children():
		child.square_clicked.connect(timeline_square_clicked)


func place_timelines():
	var centering_offset = (VisualSettings.multiverse_tile_height / 2.0) - ((chessboard_dimensions.y * 128) / 2.0)
	if perspective:
		for child in get_children():
			child.position.y = (child.layer) * VisualSettings.multiverse_tile_height + centering_offset
			child.position.x = (child.TStart - 2) * VisualSettings.multiverse_tile_width
	else:
		for child in get_children():
			child.position.y =  (-child.layer) * VisualSettings.multiverse_tile_height + centering_offset
			child.position.x = (child.TStart - 2) * VisualSettings.multiverse_tile_width


func get_timeline_layer(layer : int) -> Node:
	for child in get_children():
		if child.layer == layer:
			return child
	return null


func timeline_square_clicked(square : Vector2 , temporal_position : Vector2, color : bool ):
	square_clicked.emit(square,temporal_position,color)


func set_perspective( new_perspective ):
	perspective = new_perspective
	place_timelines()
	for child in get_children():
		child.set_perspective(new_perspective)


func flip_perspective():
	VisualSettings.perspective = !VisualSettings.perspective
	set_perspective(VisualSettings.perspective)
