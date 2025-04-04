extends Control

signal square_clicked(square : Vector2, temporal_position : Vector2, color : bool)

var highlighted_squares = []
var chessboard_dimensions : Vector2
var perspective = true
var game_container : Control
var min_active_tl := 0
var max_active_tl := 0

func _ready() -> void:
	VisualSettings.view_changed.connect(on_view_changed)
	perspective = VisualSettings.perspective
	connect_signals()
	place_timelines()
	if game_container != null:
		game_container.ActiveAreaChanged.connect(update_active_area)


func _process(delta: float) -> void:
	pass


func add_board(board_array : Array, multiverse_position : Vector2, new_board_color):
	var tl = get_timeline_layer(multiverse_position.x)
	tl.add_board(board_array, multiverse_position,new_board_color)


func add_timeline(new_timeline : Node2D):
	var centering_offset = (VisualSettings.multiverse_tile_height / 2.0) - ((chessboard_dimensions.y * 128) / 2.0)
	new_timeline.square_clicked.connect(timeline_square_clicked)
	var layer = new_timeline.layer
	if layer > min_active_tl or layer < min_active_tl:
		new_timeline.active = false
	else:
		new_timeline.active = true
	if perspective:
		new_timeline.position.y = (new_timeline.layer) * VisualSettings.multiverse_tile_height + centering_offset
		new_timeline.position.x = (new_timeline.TStart - 2) * VisualSettings.multiverse_tile_width
	else:
		new_timeline.position.y =  (-new_timeline.layer) * VisualSettings.multiverse_tile_height + centering_offset
		new_timeline.position.x = (new_timeline.TStart - 2) * VisualSettings.multiverse_tile_width
	add_child(new_timeline)

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


func update_active_area( new_present :int, minTL : int, maxTL : int):
	min_active_tl = minTL
	max_active_tl = maxTL
	for tl in get_children():
		var layer = tl.layer
		if layer > maxTL or layer < minTL:
			tl.active = false
		else:
			tl.active = true


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


func on_view_changed( perspective : bool , view ):
	place_timelines()
	set_perspective(VisualSettings.perspective)
