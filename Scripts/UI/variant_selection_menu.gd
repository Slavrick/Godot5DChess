extends MarginContainer


signal variant_selected(variant_filepath : String)
signal back_pressed

var variant_dict = {
	"Standard": {
		"Standard":{
			"Path":"",
			"Description":"",
		}
	},
	"CheckmatePractice":{
		"CheckMate Practice-Knight":{
			"Path":"",
			"Description":"Checkmate Practice for the knight",
		},
		"CheckMate Practice-Bishop":{
			"Path":"",
			"Description":"Checkmate Practice for the Bishop",
		},
		"CheckMate Practice-Rook":{
			"Path":"",
			"Description":"Checkmate Practice for the Rook",
		},
		"CheckMate Practice-Queen":{
			"Path":"",
			"Description":"Checkmate Practice for the Queen",
		},
		"CheckMate Practice-Pawns":{
			"Path":"",
			"Description":"Checkmate Practice for the Pawns",
		},
		
	},
	"SinglePiece":{
		"Standard":{
			"Path":"",
			"Description":"",
		}
	},
	"Misc":{
		
	},
}


func _ready() -> void:
	$TabContainer/Standard/VBoxContainer/Standard/Button.pressed.connect(on_variant_selected.bind("Standard"))
	$TabContainer/Standard/VBoxContainer/StandardT0/Button.pressed.connect(on_variant_selected.bind("StandardT0"))
	$"TabContainer/Standard/VBoxContainer/Defended Pawn/Button".pressed.connect(on_variant_selected.bind("DefendedPawn"))
	$"TabContainer/Standard/VBoxContainer/Princess/Button".pressed.connect(on_variant_selected.bind("Princess"))
	$"TabContainer/Focused-Single Piece/VBoxContainer/Only Brawns/Button".pressed.connect(on_variant_selected.bind("OnlyBrawns"))
	$"TabContainer/Focused-Single Piece/VBoxContainer/Only Unicorns/Button".pressed.connect(on_variant_selected.bind("OnlyUnicorns"))
	$"TabContainer/Misc Variants/VBoxContainer/TimelineStrategos/Button".pressed.connect(on_variant_selected.bind("Strategos"))
	$Node/Back.pressed.connect(func():
		back_pressed.emit()
	)
	load_buttons()


func load_buttons():
	var packed_button = load("res://Scenes/UI/VariantSelector.tscn")
	for key in variant_dict["CheckmatePractice"].keys():
		var new_button = packed_button.instantiate()
		new_button.set_labels(key,variant_dict["CheckmatePractice"][key]["Description"])
		new_button.pressed.connect(on_variant_selected.bind(key))
		$"TabContainer/Checkmate Practice/VBoxContainer".add_child(new_button)


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
		"CheckMate Practice-Knight":
			variant_selected.emit("res://PGN/Variations/CheckMatePractice-Knight.txt")
		"CheckMate Practice-Bishop":
			variant_selected.emit("res://PGN/Variations/CheckMatePractice-Bishop.txt")
		"CheckMate Practice-Rook":
			variant_selected.emit("res://PGN/Variations/CheckMatePractice-Rook.txt")
		"CheckMate Practice-Queen":
			variant_selected.emit("res://PGN/Variations/CheckMatePractice-Queen.txt")
		"CheckMate Practice-Pawns":
			variant_selected.emit("res://PGN/Variations/CheckMatePractice-Pawns.txt")
