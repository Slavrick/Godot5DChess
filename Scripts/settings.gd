extends Control

signal close


func _ready() -> void:
	$VBoxContainer/whitesquare/ColorPicker.color_changed.connect(change_light_color)
	$VBoxContainer/blacksquare/ColorPicker.color_changed.connect(change_dark_color)
	$VBoxContainer/whitemultiversesquare/ColorPicker.color_changed.connect(white_multiverse_color_change)
	$VBoxContainer/blackmultiversesquare/ColorPicker.color_changed.connect(black_multiverse_color_change)
	
	
	$VBoxContainer/whitesquare/ColorPicker.color = VisualSettings.light_square_color
	$VBoxContainer/blacksquare/ColorPicker.color = VisualSettings.dark_square_color
	$VBoxContainer/whitemultiversesquare/ColorPicker.color = VisualSettings.white_multiverse_color
	$VBoxContainer/blackmultiversesquare/ColorPicker.color = VisualSettings.black_multiverse_color
	
	$VBoxContainer/return.pressed.connect(returntomain)
	
	$VBoxContainer/Pallets/DefaultPallet.pressed.connect(set_pallet.bind(VisualSettings.default_pallet))
	$VBoxContainer/Pallets/GrayScalePallet.pressed.connect(set_pallet.bind(VisualSettings.grayscale_pallet))


func change_light_color( color : Color):
	VisualSettings.light_square_color = color

func change_dark_color( color : Color ):
	VisualSettings.dark_square_color = color


func white_multiverse_color_change( color : Color ):
	VisualSettings.white_multiverse_color = color

func black_multiverse_color_change( color : Color ):
	VisualSettings.black_multiverse_color = color


func returntomain():
	close.emit()


func set_pallet(pallet_dicitionary : Dictionary):
	VisualSettings.load_dictionary(pallet_dicitionary)
