extends Node

signal game_changed
signal visual_theme_changed(type : String)

var light_square_color : Color = Color(0.961, 0.961, 0.863):
	set(value):
		light_square_color = value
		visual_theme_changed.emit("light_square_color")
var dark_square_color : Color = Color(0.292, 0.292, 0.292):
	set(value):
		dark_square_color = value
		visual_theme_changed.emit("dark_square_color")
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

var board_width
var board_height
var multiverse_tile_width
var multiverse_tile_height


func load_user_settings():
	pass


func save_user_settings():
	pass


func import_user_settings(filepath:String):
	pass


func load_dictionary(settings_dictionary : Dictionary):
	if settings_dictionary.has("light_square_color"):
		light_square_color = settings_dictionary["light_square_color"]
	else:
		light_square_color = Color(0.961, 0.961, 0.863)
	if settings_dictionary.has("dark_square_color"):
		dark_square_color = settings_dictionary["dark_square_color"]
	else:
		dark_square_color = Color(0.292, 0.292, 0.292)
	if settings_dictionary.has("white_multiverse_color"):
		white_multiverse_color = settings_dictionary["white_multiverse_color"]
	else:
		white_multiverse_color = Color(0.412, 0.573, 0.243)
	if settings_dictionary.has("black_multiverse_color"):
		black_multiverse_color = settings_dictionary["black_multiverse_color"]
	else:
		black_multiverse_color = Color(0.306, 0.471, 0.216)










#------------------------------Pallets----------------------------------------#

var default_pallet = {
	"light_square_color":Color(0.961, 0.961, 0.863),
	"dark_square_color":  Color(0.292, 0.292, 0.292),
	"white_multiverse_color": Color(0.412, 0.573, 0.243),
	"black_multiverse_color":  Color(0.306, 0.471, 0.216),
}


var grayscale_pallet = {
	"light_square_color": Color(0.96, 0.96, 0.96),
	"dark_square_color": Color(0.065, 0.065, 0.065),
	"white_multiverse_color": Color(0.63, 0.63, 0.63),
	"black_multiverse_color":  Color(0.44, 0.44, 0.44),
}
