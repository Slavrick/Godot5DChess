extends Control

signal load_game()


func _ready() -> void:
	$"VBoxContainer/Play Game".pressed.connect(play_game)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func play_game():
	load_game.emit(" ")
