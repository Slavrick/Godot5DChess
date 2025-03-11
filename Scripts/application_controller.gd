extends Control


var game_path = "res://Scenes/UI/game.tscn"
var main_menu_path = "res://Scenes/UI/main_menu.tscn"

@onready var main_menu = $MainMenu

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$MainMenu.load_game.connect(load_game)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func load_game(file_path : String):
	#TODO make this work better. use string provided. loading screen.
	file_path = "res://PGN/Variations/Standard-T0.PGN5.txt"
	var game_node = load(game_path).instantiate()
	add_child(game_node)
	game_node.call("LoadGame", file_path)
	main_menu.queue_free()


func load_puzzles():
	pass


func exit():
	pass
