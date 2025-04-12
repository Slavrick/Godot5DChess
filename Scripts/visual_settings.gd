extends Node

signal game_changed
signal visual_theme_changed(type : String)
signal view_changed( perspective : bool, multiverse_view )

enum {
	WHITE_VIEW,
	BLACK_VIEW,
	FULL_VIEW,
}

#Colors
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
var timeline_color : Color = Color.PURPLE
var font_color : Color = Color.BLACK

#Spacing
var square_width := 128
var board_padding := 20 #Distance between squares and edge of board.
var board_horizontal_margin := 100  #Distance between boards.
var timeline_vertical_margin := 700 #Distance between timelines
var multiverse_view := FULL_VIEW :
	set(value):
		multiverse_view = value
		calculate_dimensions()
		view_changed.emit(perspective, multiverse_view)
#True for white, false for black
var perspective := true:
	set(value):
		perspective = value
		view_changed.emit(perspective, multiverse_view)

var pieces_file := "res://Resources/res/Pieces-hirez.png"
var visual_settings_path := "user://5dvisuals.json"

var game_board_dimensions := Vector2(8,8)

var board_width : float
var board_height : float
var multiverse_tile_width : float
var multiverse_tile_height : float


func change_game():
	calculate_dimensions()
	game_changed.emit()


func calculate_dimensions():
	board_width = square_width * game_board_dimensions.x
	board_height = square_width * game_board_dimensions.y
	multiverse_tile_height = (square_width * game_board_dimensions.y
		+ timeline_vertical_margin)
	if multiverse_view == FULL_VIEW:
		multiverse_tile_width = (square_width * game_board_dimensions.x * 2
			+ board_padding * 4 
			+ board_horizontal_margin * 3)
	else:
		multiverse_tile_width = (square_width * game_board_dimensions.x
			+ board_padding * 2 
			+ board_horizontal_margin)


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
	if not FileAccess.file_exists(filepath):
		print_debug("Settings dont exist")
		return false
	var file := FileAccess.open(visual_settings_path, FileAccess.READ)
	var json_settings := file.get_line()
	var visuals_dictionary = parsable_json_to_dict(json_settings)
	load_dictionary(visuals_dictionary)
	return true


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

#Needs to return top left of the tile.
func position_of_multiverse_tile(tile : Vector2) -> Vector2:
	tile = Vector2(tile.y-1,tile.x)
	if(perspective or tile.y==0):
		return tile * Vector2(multiverse_tile_width,multiverse_tile_height)
	else:
		return tile * Vector2(multiverse_tile_width,-multiverse_tile_height)


func position_of_coordinate(coord : Coord5):
	var position_ = position_of_multiverse_tile(Vector2(coord.v.z,coord.v.w))
	if perspective:
		position_ += Vector2(coord.v.x,game_board_dimensions.y - coord.v.y) * square_width
		position_ += Vector2(square_width/2,-square_width/2)
		position_.y += board_padding + (VisualSettings.multiverse_tile_height / 2.0) - ((game_board_dimensions.y * 128) / 2.0)
	else:
		position_ += Vector2(game_board_dimensions.x - coord.v.x - 1, coord.v.y) * square_width
		position_ += Vector2(square_width/2,square_width/2)
		position_.y += board_padding + (VisualSettings.multiverse_tile_height / 2.0) - ((game_board_dimensions.y * 128) / 2.0)
	match multiverse_view:
		FULL_VIEW:
			position_.x += board_padding+board_horizontal_margin
			if !coord.color and multiverse_view == FULL_VIEW:
				position_.x += multiverse_tile_width/2
		_:
			position_.x += board_padding+board_horizontal_margin/2
	return position_

func position_to_coordinate(position_ : Vector2) -> Coord5:
	var position_in_tile = Vector2(int(position_.x)  % int(multiverse_tile_width), int(position_.y)  % int(multiverse_tile_height))
	var multiverse_position = Vector2(0,0)
	var tile_position = Vector2(0,0)
	var color = true
	if position_in_tile.x > multiverse_tile_width / 2 :
		color = false
		position_in_tile.x -=  multiverse_tile_width / 2 
	position_in_tile.x -= board_horizontal_margin 
	position_in_tile.y -= timeline_vertical_margin/2
	position_in_tile -= Vector2(board_padding,board_padding)
	tile_position.x = (int(position_in_tile.x) / int(square_width))
	tile_position.y = (int(position_in_tile.y) / int(square_width))
	multiverse_position.y = floor(position_.x / multiverse_tile_width)  + 1
	multiverse_position.x = floor(position_.y / multiverse_tile_height)
	if perspective:
		tile_position.y = game_board_dimensions.y - tile_position.y - 1
	else:
		tile_position.x = game_board_dimensions.x - tile_position.x - 1
	if multiverse_position.x < 0 :
		multiverse_position.x += 1
	if multiverse_position.y <= 0 :
		multiverse_position.y += 1
	return Coord5.Create(Vector4(tile_position.x,tile_position.y,multiverse_position.x,multiverse_position.y),color)


func reset_view():
	multiverse_view = FULL_VIEW
	perspective = true


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
