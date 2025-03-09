extends Control

signal submit_turn
signal load_game
signal goto_present
signal undo_turn

func _ready() -> void:
	$HBoxContainer2/Submit.pressed.connect(submit_pressed)
	$HBoxContainer2/Undo.pressed.connect(undo_turn_pressed)

func submit_pressed():
	submit_turn.emit()


func undo_turn_pressed():
	undo_turn.emit()
