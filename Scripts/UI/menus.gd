extends Control

signal submit_turn
signal load_game
signal save_game
signal goto_present
signal undo_turn
signal flip_perspective
signal change_view(view_type)
signal promotion_chosen(piece : int)
signal turntree_item_selected(index : int)

@export var game : Node

func _ready() -> void:
	$HBoxContainer2/Submit.pressed.connect(submit_pressed)
	$HBoxContainer2/Undo.pressed.connect(undo_turn_pressed)
	$HBoxContainer2/Present.pressed.connect(gotopresent_pressed)
	$HBoxContainer2/Perspective.pressed.connect(flip_perspective_pressed)
	$"HBoxContainer/Load Game".pressed.connect(load_game_pressed)
	$HBoxContainer2/OptionButton.item_selected.connect(view_changed)
	$"HBoxContainer/Save Game".pressed.connect(save_game_pressed)
	$HBoxContainer2/Perspective.button_pressed = VisualSettings.perspective
	$PromotionPanel.promotion_chose.connect(on_promotion_chose)
	$HSplitContainer/TurnTree/ItemList.item_selected.connect(on_turntree_item_selected)
	if game != null:
		game.TurnChanged.connect(get_turn_tree)
		game.GameLoaded.connect(on_game_loaded)


func show_promotion():
	$PromotionPanel.show()

func set_analysis_mode():
	$HBoxContainer.show()
	$HSplitContainer.show()

func submit_pressed():
	submit_turn.emit()


func undo_turn_pressed():
	undo_turn.emit()


func load_game_pressed():
	load_game.emit()

func save_game_pressed():
	save_game.emit()

func gotopresent_pressed():
	goto_present.emit()


func flip_perspective_pressed():
	VisualSettings.perspective = !VisualSettings.perspective 
	flip_perspective.emit()#Slated for removal to emit this, redundant.


func set_turn_label(color : bool, present : int):
	if color:
		$ColorRect/RichTextLabel.text = "White's Turn, Present: " + str(present)
	else:
		$ColorRect/RichTextLabel.text = "Blacks's Turn, Present: " + str(present)


func view_changed(index : int):
	match index:
		0:
			VisualSettings.multiverse_view = VisualSettings.FULL_VIEW
		1:
			VisualSettings.multiverse_view = VisualSettings.BLACK_VIEW
		2:
			VisualSettings.multiverse_view = VisualSettings.WHITE_VIEW


func on_promotion_chose( piece : int):
	$PromotionPanel.hide()
	promotion_chosen.emit(piece)


func get_turn_tree(player : bool, present : int):
	var labels = game.GetLinearLabels()#probably bad, o well.
	$HSplitContainer/TurnTree/ItemList.clear()
	for label in labels :
		$HSplitContainer/TurnTree/ItemList.add_item(label)
	$HSplitContainer/TurnTree/ItemList.set_item_text(0,"Starting Position")


func on_game_loaded():
	get_turn_tree(false,0)
	$HSplitContainer/TurnTree.scroll_vertical = 0


func on_turntree_item_selected( index : int):
	turntree_item_selected.emit(index)
