class_name Hand
extends Control

@onready var cards: HBoxContainer = %Cards
@onready var title: RichTextLabel = %Title

var id : int
var atom : ZoneAtom

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	title.text = atom.get_definition_name()
	pass # Replace with function body.

func add_card(card: Card):
	cards.add_child(card)
	print("Added %s" % card.atom.get_definition_name())
