extends Control


var game_path = "res://Scenes/UI/game.tscn"
var main_menu_path = "res://Scenes/UI/main_menu.tscn"

@onready var main_menu = $MainMenu

var game5D

func _ready() -> void:
	$MainMenu.load_game.connect(load_game)


func load_game(file_path : String):
	#TODO make this work better. use string provided. loading screen.
	file_path = "res://PGN/Variations/Standard-T0.PGN5.txt"
	var game_node = load(game_path).instantiate()
	add_child(game_node)
	game_node.call("LoadGame", file_path)
	game_node.ExitGame.connect(exit_game)
	game5D = game_node
	main_menu.queue_free()


func load_puzzles():
	pass


func exit():
	pass


func exit_game():
	main_menu = load(main_menu_path).instantiate()
	add_child(main_menu)
	main_menu.load_game.connect(load_game)
	game5D.queue_free()
	
