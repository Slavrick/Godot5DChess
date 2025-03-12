extends Control

signal load_game()


func _ready() -> void:
	$"MenuVbox/Play Game".pressed.connect(play_game)
	$MenuVbox/Settings.pressed.connect(show_settings)
	$Settings.close.connect(close_settings)
	$MenuVbox/Exit.pressed.connect(exit_program)


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


func exit_program():
	get_tree().root.propagate_notification(NOTIFICATION_WM_CLOSE_REQUEST)
	get_tree().quit()
