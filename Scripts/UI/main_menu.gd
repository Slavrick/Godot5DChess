extends Control

signal load_game()


func _ready() -> void:
	$"MenuVbox/Play Game".pressed.connect(play_game)
	$MenuVbox/Settings.pressed.connect(show_settings)
	$Settings.close.connect(close_settings)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func play_game():
	load_game.emit(" ")


func show_settings():
	$MenuVbox.hide()
	$RichTextLabel.hide()
	$Settings.show()


func close_settings():
	$Settings.hide()
	$MenuVbox.show()
	$RichTextLabel.show()
