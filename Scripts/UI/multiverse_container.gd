extends Control

signal square_clicked(square : Vector2, temporal_position : Vector2, color : bool)
signal square_right_clicked( square : Coord5,pressed : bool)
signal square_hovered( c : Coord5 )


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


func add_board(board_array : Array, multiverse_position : Vector2, new_board_color):
	var tl = get_timeline_layer(multiverse_position.x)
	tl.add_board(board_array, multiverse_position,new_board_color)


func add_timeline(new_timeline : Node2D):
	var centering_offset = (VisualSettings.multiverse_tile_height / 2.0) - ((chessboard_dimensions.y * 128) / 2.0)
	new_timeline.square_clicked.connect(timeline_square_clicked)
	new_timeline.square_right_clicked.connect(timeline_square_right_clicked)
	new_timeline.square_hovered.connect(timeline_square_hovered)
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


func annotate_highlight_square( square : Coord5, annotation_color : Color):
	get_timeline_layer(square.v.z).annotate_highlight_square(square,annotation_color)


func annotate_unhighlight_square(square : Coord5):
	get_timeline_layer(square.v.z).annotate_unhighlight_square(square)


func clear_highlights():
	for child in get_children():
		child.clear_highlights()


func connect_signals():
	for child in get_children():
		child.square_clicked.connect(timeline_square_clicked)
		child.square_right_clicked.connect(timeline_square_right_clicked)
		child.square_hovered.connect(timeline_square_hovered)


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


func timeline_square_right_clicked(c : Coord5, pressed : bool):
	square_right_clicked.emit(c,pressed)


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


func timeline_square_hovered( c : Coord5):
	square_hovered.emit(c)


func undo_moves( turntimelines : Array ):
	for layer in turntimelines: #this is so dumb, idk if there is a better way. Maybe if we know the array is sorted.
		var timeline = get_timeline_layer(layer)
		if timeline == null:
			print_debug(str(turntimelines) + "TL nill exists")
			return
		timeline.pop_board_off()
