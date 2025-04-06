extends RichTextLabel


func _on_game_is_mated(player_mated: bool) -> void:
	show()
	if player_mated:
		text = "[center]Black Wins![/center]"
	else:
		text = "[center]White Wins![/center]"


func _on_game_game_loaded() -> void:
	hide()
