extends Node

signal game_changed
signal visual_theme_changed(type : String)

var light_square_color : Color = Color(0.961, 0.961, 0.863):
	set(value):
		light_square_color = value
		visual_theme_changed.emit("white_square_color")
var dark_square_color : Color = Color(0.292, 0.292, 0.292):
	set(value):
		dark_square_color = value
		visual_theme_changed.emit("white_square_color")
var white_multiverse_color : Color = Color(0.412, 0.573, 0.243):
	set(value):
		white_multiverse_color = value
		visual_theme_changed.emit("white_multiverse_color")
var black_multiverse_color : Color = Color(0.306, 0.471, 0.216):
	set(value):
		black_multiverse_color = value
		visual_theme_changed.emit("black_multiverse_color")

var pieces_file := "res://Resources/res/Pieces-hirez.png"

var game_board_dimensions := Vector2(8,8)
var board_padding := 20
var board_horizontal_margin := 20
var timeline_vertical_margin := 700
