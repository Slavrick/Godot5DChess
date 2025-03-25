extends Node2D

@export var game : Control
@export var camera : Node2D
@export var camera_position := Vector2(10,10)
@export var light_color := Color(0.412, 0.573, 0.243)
@export var dark_color := Color(0.306, 0.471, 0.216)
@export var coord_font : Font
@export var coord_font_size := 200
@export var even_start := false
@export var SQUARE_LENGTH := 2304
@export var SQUARE_HEIGHT := 1500
@export var grid_width = 40
@export var grid_height = 40
@export_group("Inactive Sqaures")
@export var present : int = 100
@export var min_active_tl : int = -10 
@export var max_active_tl : int = 10

var inactive_light : Color
var inactive_dark : Color

func _ready():
	VisualSettings.visual_theme_changed.connect(update_theme)
	VisualSettings.view_changed.connect(on_view_changed)
	update_theme("null")
	VisualSettings.game_changed.connect(update_dimensions)
	if camera != null:
		if camera.position != camera_position:
			camera_position = camera.position
			queue_redraw()
	if game != null:
		game.ActiveAreaChanged.connect(update_active_area)


func _process( delta :float ) -> void:
	if camera != null:
		if camera.position != camera_position:
			camera_position = camera.position
			queue_redraw()


func update_theme(visual_changed : String):
	dark_color = VisualSettings.black_multiverse_color
	light_color = VisualSettings.white_multiverse_color
	inactive_light = desaturate_color(light_color,.4)
	inactive_dark = desaturate_color(dark_color,.4)
	queue_redraw()


func desaturate_color(c : Color, d : float) -> Color:
	return c.lerp(Color.SADDLE_BROWN,d)


func update_dimensions():
	SQUARE_LENGTH = VisualSettings.multiverse_tile_width
	SQUARE_HEIGHT = VisualSettings.multiverse_tile_height
	queue_redraw()

#TODO This doesn't display the right label in the case of a flipped perspective. ( in other words -1L needs to be 1L)
func _draw() -> void:
	var x_offset = camera_position.x - (10 * SQUARE_LENGTH) - (int(floor(camera_position.x)) % SQUARE_LENGTH)
	var y_offset = camera_position.y - (10 * SQUARE_HEIGHT) - (int(floor(camera_position.y)) % SQUARE_HEIGHT)
	
	var odd_start =  ( abs(int( floor(x_offset/SQUARE_LENGTH))) + abs(int( floor(y_offset/SQUARE_HEIGHT))) ) % 2 == 1
	
	#TODO starting_time and starting layer are bugged.
	var starting_time
	var starting_layer
	if y_offset < 0:
		starting_layer = (int(y_offset) / SQUARE_HEIGHT) - 2
	else:
		starting_layer = (int(y_offset) / SQUARE_HEIGHT) - 1
	if x_offset < 0:
		starting_time = int(x_offset) / SQUARE_LENGTH
	else:
		starting_time = (int(x_offset) / SQUARE_LENGTH) + 1
	if int(y_offset) % SQUARE_HEIGHT == 0:
		starting_layer += 1
	if int(x_offset) % SQUARE_LENGTH == 0:
		starting_time += 1
	
	for x in range(grid_width):
		for y in range(grid_height):
			var rect = Rect2(x * SQUARE_LENGTH + x_offset, y * SQUARE_HEIGHT + y_offset,SQUARE_LENGTH, SQUARE_HEIGHT)	
			if odd_start:
				if (x + y) % 2 == 0:
					if(starting_time + x > present or starting_layer + y + 1 > max_active_tl or starting_layer + y + 1 < min_active_tl):
						draw_rect(rect,inactive_light,true)
					else:
						draw_rect(rect,light_color,true)
				else:
					if(starting_time + x > present or starting_layer + y + 1 > max_active_tl or starting_layer + y + 1 < min_active_tl):
						draw_rect(rect,inactive_dark,true)
					else:
						draw_rect(rect,dark_color,true)
			else:
				if (x + y) % 2 == 0:
					if(starting_time + x > present or starting_layer + y + 1 > max_active_tl or starting_layer + y + 1 < min_active_tl):
						draw_rect(rect,inactive_dark,true)
					else:
						draw_rect(rect,dark_color,true)
				else:
					if(starting_time + x > present or starting_layer + y + 1> max_active_tl or starting_layer + y + 1 < min_active_tl):
						draw_rect(rect,inactive_light,true)
					else:
						draw_rect(rect,light_color,true)
			if coord_font_size > 0:
				if VisualSettings.perspective:
					draw_string(coord_font, Vector2(x * SQUARE_LENGTH + x_offset, y * SQUARE_HEIGHT + y_offset), str(starting_layer + y) + "L" + str(starting_time + x) + "T",HORIZONTAL_ALIGNMENT_LEFT,-1,coord_font_size)
				else:
					draw_string(coord_font, Vector2(x * SQUARE_LENGTH + x_offset, y * SQUARE_HEIGHT + y_offset), str(-1 * (starting_layer + y)) + "L" + str(starting_time + x) + "T",HORIZONTAL_ALIGNMENT_LEFT,-1,coord_font_size)


func on_view_changed( perspective : bool , view ):
	update_dimensions()


func update_active_area( new_present :int, minTL : int, maxTL : int):
	present = new_present
	min_active_tl = minTL
	max_active_tl = maxTL
	
