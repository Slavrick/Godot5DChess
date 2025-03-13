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
var visual_settings_path := "user://5dvisuals.json"

var game_board_dimensions := Vector2(8,8)
var board_padding := 20
var board_horizontal_margin := 20
var timeline_vertical_margin := 700

var board_width
var board_height
var multiverse_tile_width
var multiverse_tile_height


func load_user_settings():
	if not FileAccess.file_exists(visual_settings_path):
		print_debug("Settings dont exist")
		return
	var file := FileAccess.open(visual_settings_path, FileAccess.READ)
	var json_settings := file.get_line()
	var visuals_dictionary = parsable_json_to_dict(json_settings)
	load_dictionary(visuals_dictionary)

func save_user_settings() -> void:
	var save_data = dict_to_parsable_json(current_settings_to_dictionary())
	var file := FileAccess.open(visual_settings_path,FileAccess.WRITE)
	file.store_line(JSON.stringify(save_data))


func current_settings_to_dictionary() -> Dictionary:
	var data_dictionary = Dictionary()
	data_dictionary["light_square_color"] = light_square_color
	data_dictionary["dark_square_color"] = dark_square_color
	data_dictionary["white_multiverse_color"] = white_multiverse_color
	data_dictionary["black_multiverse_color"] = black_multiverse_color
	return data_dictionary


func dict_to_parsable_json(dictionary : Dictionary):
	var new_dict = Dictionary()
	for key in dictionary.keys():
		new_dict[key] = dictionary[key].to_html()
	return new_dict

func parsable_json_to_dict(json : String) -> Dictionary:
	var json_dict = JSON.parse_string(json)
	var new_dict = Dictionary()
	new_dict["light_square_color"] = Color(json_dict["light_square_color"])
	new_dict["dark_square_color"] = Color(json_dict["dark_square_color"])
	new_dict["white_multiverse_color"] = Color(json_dict["white_multiverse_color"])
	new_dict["black_multiverse_color"] = Color(json_dict["black_multiverse_color"])
	return new_dict

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










#------------------------------Palette----------------------------------------#

var default_palette = {
	"light_square_color": Color(0.961, 0.961, 0.863),
	"dark_square_color":  Color(0.292, 0.292, 0.292),
	"white_multiverse_color": Color(0.412, 0.573, 0.243),
	"black_multiverse_color":  Color(0.306, 0.471, 0.216),
}


var grayscale_palette = {
	"light_square_color": Color(0.96, 0.96, 0.96),
	"dark_square_color": Color(0.065, 0.065, 0.065),
	"white_multiverse_color": Color(0.63, 0.63, 0.63),
	"black_multiverse_color":  Color(0.44, 0.44, 0.44),
}

var glass_palette = {
	"light_square_color": Color(0.871, 0.89, 0.902),
	"dark_square_color":Color(0.549, 0.635, 0.678),
	"white_multiverse_color": Color(0.412, 0.447, 0.514),
	"black_multiverse_color":  Color(0.149, 0.176, 0.227),
}

var wood_palette = {
	"light_square_color":Color(0.792, 0.667, 0.478),
	"dark_square_color":  Color(0.51, 0.373, 0.235),
	"white_multiverse_color": Color(0.412, 0.573, 0.243),
	"black_multiverse_color":  Color(0.306, 0.471, 0.216),
}

var checkers_palette = {
	"light_square_color": Color(0.78, 0.298, 0.318),
	"dark_square_color":Color(0.188, 0.188, 0.188),
	"white_multiverse_color": Color(0.291, 0.28, 0.264),
	"black_multiverse_color": Color(0.188, 0.18, 0.169),
}
