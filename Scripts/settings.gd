extends Control

signal close


func _ready() -> void:
	$TabContainer/Colors/whitesquare/ColorPicker.color_changed.connect(change_light_color)
	$TabContainer/Colors/blacksquare/ColorPicker.color_changed.connect(change_dark_color)
	$TabContainer/Colors/whitemultiversesquare/ColorPicker.color_changed.connect(white_multiverse_color_change)
	$TabContainer/Colors/blackmultiversesquare/ColorPicker.color_changed.connect(black_multiverse_color_change)
	
	VisualSettings.load_user_settings()
	set_colorpickers()
	$Control/return.pressed.connect(returntomain)
	$TabContainer/Colors/Pallets/DefaultPalette.pressed.connect(set_pallet.bind(VisualSettings.default_palette))
	$TabContainer/Colors/Pallets/GrayScalePalette.pressed.connect(set_pallet.bind(VisualSettings.grayscale_palette))
	$TabContainer/Colors/Pallets/GlassPalette.pressed.connect(set_pallet.bind(VisualSettings.glass_palette))
	$TabContainer/Colors/Pallets/WoodPalette.pressed.connect(set_pallet.bind(VisualSettings.wood_palette))
	$TabContainer/Colors/Pallets/CheckersPalette.pressed.connect(set_pallet.bind(VisualSettings.checkers_palette))
	
	
	$TabContainer/Colors/TLVertMargin/SpinBox.value_changed.connect(TLMarginChanged)
	$TabContainer/Colors/BoardPadding/SpinBox.value_changed.connect(board_padding_changed)
	$TabContainer/Colors/BoardMargin/SpinBox.value_changed.connect(board_margin_changed)


func change_light_color( color : Color):
	VisualSettings.light_square_color = color


func change_dark_color( color : Color ):
	VisualSettings.dark_square_color = color


func white_multiverse_color_change( color : Color ):
	VisualSettings.white_multiverse_color = color


func black_multiverse_color_change( color : Color ):
	VisualSettings.black_multiverse_color = color


func set_colorpickers():
	$TabContainer/Colors/whitesquare/ColorPicker.color = VisualSettings.light_square_color
	$TabContainer/Colors/blacksquare/ColorPicker.color = VisualSettings.dark_square_color
	$TabContainer/Colors/whitemultiversesquare/ColorPicker.color = VisualSettings.white_multiverse_color
	$TabContainer/Colors/blackmultiversesquare/ColorPicker.color = VisualSettings.black_multiverse_color



func returntomain():
	close.emit()
	VisualSettings.save_user_settings()


func set_pallet(pallet_dicitionary : Dictionary):
	VisualSettings.load_dictionary(pallet_dicitionary)
	set_colorpickers()


func TLMarginChanged(value : float):
	VisualSettings.timeline_vertical_margin = value


func board_margin_changed(value: float):
	VisualSettings.board_horizontal_margin = value


func board_padding_changed(value:float):
	VisualSettings.board_padding = value
