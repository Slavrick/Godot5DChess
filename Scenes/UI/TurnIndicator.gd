extends Panel

var white_indicator = load("res://Resources/white_indicator.tres")
var black_indicator = load("res://Resources/black_indicator.tres")


func _ready() -> void:
	set_turn_indicator(true)


func set_turn_indicator( color : bool ):
	if color:
		add_theme_stylebox_override("panel",white_indicator)
	else:
		add_theme_stylebox_override("panel",black_indicator)


func _on_game_turn_changed(player: bool, present: int) -> void:
	set_turn_indicator(player)
