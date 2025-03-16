extends Control

signal load_game()


func _ready() -> void:
	$"Main/MenuVbox/Play Game".pressed.connect(play_game)
	$Main/MenuVbox/Settings.pressed.connect(show_settings)
	$Main/MenuVbox/Exit.pressed.connect(exit_program)
	$Settings.close.connect(close_settings)
	$VariantSelectionMenu.variant_selected.connect(on_variant_selected)

func play_game():
	$Main.hide()
	$VariantSelectionMenu.show()


func show_settings():
	$Main.hide()
	$Settings.show()


func close_settings():
	$Settings.hide()
	$Main.show()


func exit_program():
	get_tree().root.propagate_notification(NOTIFICATION_WM_CLOSE_REQUEST)
	get_tree().quit()


func on_variant_selected(variantfilepath : String):
	load_game.emit(variantfilepath)
