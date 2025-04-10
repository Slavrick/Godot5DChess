extends Control

signal load_game
signal load_analysis_game(file_path : String)


func _ready() -> void:
	$"Main/MenuVbox/Play Game".pressed.connect(play_game)
	$Main/MenuVbox/Settings.pressed.connect(show_settings)
	$Main/MenuVbox/Exit.pressed.connect(exit_program)
	$Settings.close.connect(close_settings)
	$VariantSelectionMenu.variant_selected.connect(on_variant_selected)
	$Main/MenuVbox/Analysis.pressed.connect(analysis_pressed)
	$Main/MenuVbox/Attribution.pressed.connect(on_attribution_pressed)
	$Attribution.close_attribution.connect(close_attribution)
	$VariantSelectionMenu.back_pressed.connect(on_variant_back_pressed)

func play_game():
	$Main.hide()
	$VariantSelectionMenu.show()


func show_settings():
	$Main.hide()
	$Settings.show()


func close_settings():
	$Settings.hide()
	$Main.show()


func analysis_pressed():
	load_analysis_game.emit("res://PGN/Variations/Standard-T0.PGN5.txt")

func exit_program():
	get_tree().root.propagate_notification(NOTIFICATION_WM_CLOSE_REQUEST)
	get_tree().quit()


func on_variant_selected(variantfilepath : String):
	load_game.emit(variantfilepath)


func on_attribution_pressed():
	$Main.hide()
	$Attribution.show()


func close_attribution():
	$Attribution.hide()
	$Main.show()


func on_variant_back_pressed():
	$Main.show()
	$VariantSelectionMenu.hide()
