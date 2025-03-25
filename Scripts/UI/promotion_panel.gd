extends PanelContainer


signal promotion_chose( piece : int )

func _ready() -> void:
	#These are indexed by the enums in board.cs
	$PanelContainer/Pawn.pressed.connect(promotion_clicked.bind(1))
	$PanelContainer/Knight.pressed.connect(promotion_clicked.bind(2))
	$PanelContainer/Bishop.pressed.connect(promotion_clicked.bind(3))
	$PanelContainer/Rook.pressed.connect(promotion_clicked.bind(4))
	$PanelContainer/Queen.pressed.connect(promotion_clicked.bind(6))
	$PanelContainer/King.pressed.connect(promotion_clicked.bind(7))
	$PanelContainer/CommonKing.pressed.connect(promotion_clicked.bind(12))

func set_available_promotions(promotions : int):
	pass
	#TODO


func promotion_clicked( piece : int):
	promotion_chose.emit(piece)
