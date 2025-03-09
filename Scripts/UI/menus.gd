extends Control

signal submit_turn
signal load_game
signal goto_present
signal undo_turn


func _ready() -> void:
	$HBoxContainer2/Submit.pressed.connect(submit_pressed)
	$HBoxContainer2/Undo.pressed.connect(undo_turn_pressed)
	$"HBoxContainer/Load Game".pressed.connect(load_game_pressed)


func submit_pressed():
	submit_turn.emit()


func undo_turn_pressed():
	undo_turn.emit()


func load_game_pressed():
	load_game.emit()


func set_turn_label(color : bool, present : int):
	if color:
		$ColorRect/RichTextLabel.text = "White's Turn, Present: " + str(present)
	else:
		$ColorRect/RichTextLabel.text = "Blacks's Turn, Present: " + str(present)
