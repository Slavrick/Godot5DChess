extends Node2D


@export var camera : Node2D
@export var camera_position := Vector2.ZERO
@export var light_color := Color(0.412, 0.573, 0.243)
@export var dark_color := Color(0.306, 0.471, 0.216)
@export var SQUARE_LENGTH := 2304
@export var SQUARE_HEIGHT := 1500
@export var coord_font : Font
@export var coord_font_size := 200
@export var grid_width = 40
@export var grid_height = 40


func _ready():
	VisualSettings.visual_theme_changed.connect(update_theme)
	update_theme("null")
	VisualSettings.game_changed.connect(update_dimensions)

func _process( delta :float ) -> void:
	if camera != null:
		if camera.global_position != camera_position:
			camera_position = camera.global_position
			queue_redraw()


func update_theme(visual_changed : String):
	dark_color = VisualSettings.black_multiverse_color
	light_color = VisualSettings.white_multiverse_color
	queue_redraw()


func update_dimensions():
	SQUARE_LENGTH = (128 * VisualSettings.game_board_dimensions.x * 2
		+ VisualSettings.board_padding * 4 
		+ VisualSettings.board_horizontal_margin * 2)
	SQUARE_HEIGHT = (128 * VisualSettings.game_board_dimensions.y
		+ VisualSettings.timeline_vertical_margin)


func _draw() -> void:
	
	var x_offset = camera_position.x - (10 * SQUARE_LENGTH) - (int(floor(camera_position.x)) % SQUARE_LENGTH)
	var y_offset = camera_position.y - (10 * SQUARE_HEIGHT) - (int(floor(camera_position.y)) % SQUARE_HEIGHT)
	
	var odd_start =  ( abs(int( floor(x_offset/SQUARE_LENGTH))) + abs(int( floor(y_offset/SQUARE_HEIGHT))) ) % 2 == 1
	
	var starting_layer = (int(y_offset) / SQUARE_HEIGHT) - 2
	var starting_time = int(x_offset) / SQUARE_LENGTH
	for x in range(grid_width):
		for y in range(grid_height):
			var rect = Rect2(x * SQUARE_LENGTH + x_offset, y * SQUARE_HEIGHT + y_offset,SQUARE_LENGTH, SQUARE_HEIGHT)	
			if odd_start:
				if (x + y) % 2 == 0:
					draw_rect(rect,light_color,true)
				else:
					draw_rect(rect,dark_color,true)
			else:
				if (x + y) % 2 == 0:
					draw_rect(rect,dark_color,true)
				else:
					draw_rect(rect,light_color,true)
			if coord_font_size > 0:
				draw_string(coord_font, Vector2(x * SQUARE_LENGTH + x_offset, y * SQUARE_HEIGHT + y_offset), str(starting_layer + y) + "L" + str(starting_time + x) + "T",HORIZONTAL_ALIGNMENT_LEFT,-1,coord_font_size)
