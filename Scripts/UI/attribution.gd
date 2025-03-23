extends CenterContainer

signal close_attribution

func _ready() -> void:
	$Control/Button.pressed.connect(on_main_menu_pressed)


func on_main_menu_pressed():
	close_attribution.emit()
