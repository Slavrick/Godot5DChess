extends Node2D
#TODO This originally was coded wierdly, need to unsphagettify.
#TODO Control draw calls only show in its 'box' so to say. the timeline was pushed back and then
#TODO the draw calls are pushed forward so you could have the tail of the timeline.
#TODO change this so that not nessesary to push it back, simplifying the draw calls.
#TODO Start with the multiverse container place timelines function.
signal square_clicked(square : Vector2, temporal_position : Vector2, color : bool)

enum DRAWMODE {
	FULL,
	WHITE,
	BLACK
}

@export var draw_mode := DRAWMODE.FULL

var TStart := 0
var color_start := true
var layer := 0
var active := true
var timeline_arrow_thickness := 160
var chessboard_dimensions : Vector2
var inactive_color := Color.WEB_GRAY


func _ready() -> void:
	place_children()
	connect_children()
	VisualSettings.view_changed.connect(on_view_changed)


func connect_children():
	for child in get_children():
		child.square_clicked.connect(board_square_clicked)


func place_children():
	match VisualSettings.multiverse_view:
		VisualSettings.WHITE_VIEW:
			for child in get_children():
				if !child.color:
					child.hide()
					continue
				else:
					child.show()
				var ply = child.multiverse_position.y - TStart + 1
				child.position.x = ply * (child.functional_width() + VisualSettings.board_horizontal_margin) + VisualSettings.board_horizontal_margin/2
		VisualSettings.BLACK_VIEW:
			for child in get_children():
				if child.color == true:
					child.hide()
					continue
				else:
					child.show()
				var ply = child.multiverse_position.y - TStart + 1
				child.position.x = ply * (child.functional_width() + VisualSettings.board_horizontal_margin) + VisualSettings.board_horizontal_margin/2
		_:
			for child in get_children():
				child.show()
				var ply = child.multiverse_position.y - TStart + 1
				if child.color:
					ply *= 2
				else:
					ply *= 2
					ply += 1
				child.position.x = ply * (VisualSettings.multiverse_tile_width / 2)
				child.position.x += VisualSettings.board_horizontal_margin
#TODO change all child.functional_width and height


func _draw():
	var horizontal_stickout = 100 #TODO define this in visualSettings.
	var draw_color = VisualSettings.timeline_color
	if !active:
		draw_color = inactive_color
	#draw_rect(Rect2(-30,-30,60,60),Color.MAGENTA) For testing purposes.
	match VisualSettings.multiverse_view:
		VisualSettings.FULL_VIEW:
			var width = horizontal_stickout * 2
			for child in get_children():
						width += VisualSettings.multiverse_tile_width / 2
			var tl_draw_position
			if color_start:
				tl_draw_position = Vector2(VisualSettings.multiverse_tile_width - horizontal_stickout,VisualSettings.game_board_dimensions.y * 128 / 2)
			else:
				tl_draw_position = Vector2(VisualSettings.multiverse_tile_width * 1.5 - horizontal_stickout,VisualSettings.game_board_dimensions.y * 128 / 2)
			var tl_arrow_dimension = Vector2(width,timeline_arrow_thickness)
			draw_rect(Rect2(tl_draw_position,tl_arrow_dimension),draw_color,true)
			draw_circle(tl_draw_position + Vector2(0,timeline_arrow_thickness/2),timeline_arrow_thickness/2,draw_color)
			var points = PackedVector2Array()
			points.append(tl_draw_position + Vector2(tl_arrow_dimension.x, -timeline_arrow_thickness))
			points.append(tl_draw_position + Vector2(tl_arrow_dimension.x, 2 * timeline_arrow_thickness))
			points.append(tl_draw_position + Vector2(tl_arrow_dimension.x + horizontal_stickout * 1.5, timeline_arrow_thickness/2))
			draw_colored_polygon(points,draw_color)
		VisualSettings.BLACK_VIEW:
			var width = horizontal_stickout * 2
			for child in get_children():
				if !child.color:
					width += VisualSettings.multiverse_tile_width 
			var tl_draw_position
			tl_draw_position = Vector2(VisualSettings.multiverse_tile_width - horizontal_stickout,VisualSettings.game_board_dimensions.y * 128 / 2)
			var tl_arrow_dimension = Vector2(width,timeline_arrow_thickness)
			draw_rect(Rect2(tl_draw_position,tl_arrow_dimension),draw_color,true)
			
			draw_circle(tl_draw_position + Vector2(0,timeline_arrow_thickness/2),timeline_arrow_thickness/2,draw_color)
			
			var points = PackedVector2Array()
			points.append(tl_draw_position + Vector2(tl_arrow_dimension.x, -horizontal_stickout))
			points.append(tl_draw_position + Vector2(tl_arrow_dimension.x, 2 * horizontal_stickout))
			points.append(tl_draw_position + Vector2(tl_arrow_dimension.x + horizontal_stickout * 1.5, timeline_arrow_thickness/2))
			draw_colored_polygon(points,draw_color)
		VisualSettings.WHITE_VIEW:
			var width = horizontal_stickout * 2
			for child in get_children():
				if child.color:
					width += VisualSettings.multiverse_tile_width 
			var tl_draw_position
			tl_draw_position = Vector2(VisualSettings.multiverse_tile_width - horizontal_stickout,VisualSettings.game_board_dimensions.y * 128 / 2)
			if !color_start:
				tl_draw_position.x += VisualSettings.multiverse_tile_width
			var tl_arrow_dimension = Vector2(width,timeline_arrow_thickness)
			draw_rect(Rect2(tl_draw_position,tl_arrow_dimension),draw_color,true)
			
			draw_circle(tl_draw_position + Vector2(0,timeline_arrow_thickness/2),timeline_arrow_thickness/2,draw_color)
			
			var points = PackedVector2Array()
			points.append(tl_draw_position + Vector2(tl_arrow_dimension.x, -horizontal_stickout))
			points.append(tl_draw_position + Vector2(tl_arrow_dimension.x, 2 * horizontal_stickout))
			points.append(tl_draw_position + Vector2(tl_arrow_dimension.x + horizontal_stickout * 1.5, timeline_arrow_thickness/2))
			draw_colored_polygon(points,draw_color)
#TODO need to recalc the center y of tile.


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


func set_perspective( perspective ):
	for child in get_children():
		child.board_perspective = perspective
		child.queue_redraw()


func on_view_changed( perspective : bool , view ):
	place_children()
	queue_redraw()
