extends MarginContainer


signal variant_selected(variant_filepath : String)


func _ready() -> void:
	$TabContainer/Standard/VBoxContainer/Standard/Button.pressed.connect(on_variant_selected.bind("Standard"))
	$TabContainer/Standard/VBoxContainer/StandardT0/Button.pressed.connect(on_variant_selected.bind("StandardT0"))
	$"TabContainer/Standard/VBoxContainer/Defended Pawn/Button".pressed.connect(on_variant_selected.bind("DefendedPawn"))
	$"TabContainer/Standard/VBoxContainer/Princess/Button".pressed.connect(on_variant_selected.bind("Princess"))
	$"TabContainer/Focused-Single Piece/VBoxContainer/Only Brawns/Button".pressed.connect(on_variant_selected.bind("OnlyBrawns"))
	$"TabContainer/Focused-Single Piece/VBoxContainer/Only Unicorns/Button".pressed.connect(on_variant_selected.bind("OnlyUnicorns"))
	$"TabContainer/Misc Variants/VBoxContainer/TimelineStrategos/Button".pressed.connect(on_variant_selected.bind("Strategos"))

func on_variant_selected( variant : String ):
	match variant:
		"Standard":
			variant_selected.emit("res://PGN/Standard.PGN5.txt")
		"StandardT0":
			variant_selected.emit("res://PGN/Variations/Standard-T0.PGN5.txt")
		"DefendedPawn":
			variant_selected.emit("res://PGN/Variations/Standard-DefendedPawn.txt")
		"Princess":
			variant_selected.emit("res://PGN/Variations/Standard-Princes.txt")
		"Two-Timeline":
			variant_selected.emit("res://PGN/Variations/Standard-T0.PGN5.txt")
		"OnlyBrawns":
			variant_selected.emit("res://PGN/Variations/JustBrawns.PGN5.txt")
		"OnlyUnicorns":
			variant_selected.emit("res://PGN/Variations/JustUnicorns.PGN5.txt")
		"Strategos":
			variant_selected.emit("res://PGN/Variations/Timeline-Strategos.PGN5.txt")
