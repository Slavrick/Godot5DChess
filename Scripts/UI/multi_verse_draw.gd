extends Node2D


@export var camera_position := Vector2.ZERO
@export var light_color := Color.PALE_TURQUOISE
@export var dark_color := Color.NAVY_BLUE
@export var SQUARE_LENGTH := 1000
@export var SQUARE_HEIGHT := 500

var grid_width = 10
var grid_height = 10


func _draw() -> void:
	var x_offset
	var y_offset
	for x in range(grid_width):
		for y in range(grid_height):
			var rect = Rect2(x * SQUARE_LENGTH, y * SQUARE_HEIGHT,SQUARE_LENGTH, SQUARE_HEIGHT)
			if (x + y) % 2 == 0:
				draw_rect(rect,light_color,true)
			else:
				draw_rect(rect,dark_color,true)
