extends RichTextLabel



func _on_game_is_mated(player_mated: bool) -> void:
	show()
	if player_mated:
		text = "Black Wins!"
	else:
		text = "White Wins!"
