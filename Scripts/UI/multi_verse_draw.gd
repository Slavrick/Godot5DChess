extends Node2D


@export var camera : Node2D
@export var camera_position := Vector2.ZERO
@export var light_color := Color.PALE_TURQUOISE
@export var dark_color := Color.NAVY_BLUE
@export var SQUARE_LENGTH := 2304
@export var SQUARE_HEIGHT := 1500

var grid_width = 40
var grid_height = 40


func _process( delta :float ) -> void:
	if camera != null:
		if camera.global_position != camera_position:
			camera_position = camera.global_position
			queue_redraw()

func _draw() -> void:
	var x_offset = camera_position.x - (10 * SQUARE_LENGTH) - (int(floor(camera_position.x)) % SQUARE_LENGTH)
	var y_offset = camera_position.y - (10 * SQUARE_HEIGHT) - (int(floor(camera_position.y)) % SQUARE_HEIGHT)
	#print_debug(x_offset)
	#print_debug(y_offset)
	var odd_start =  ( abs(int( floor(x_offset/SQUARE_LENGTH))) + abs(int( floor(y_offset/SQUARE_HEIGHT))) ) % 2 == 1
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
