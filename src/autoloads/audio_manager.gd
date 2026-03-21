extends Node2D

@export var n_static_players: int = 4
@export var n_2d_players: int = 4

@export var db_adjust : float


var _static_p: Array[AudioStreamPlayer] = []
var _2d_p: Array[AudioStreamPlayer2D] = []


func _ready() -> void:
	_allocate_players()

func _allocate_players():
	for i in range(n_static_players):
		var p = AudioStreamPlayer.new()
		p.volume_db = db_adjust
		add_child(p)
		_static_p.push_front(p)
	
	for i in range(n_2d_players):
		var p = AudioStreamPlayer2D.new()
		p.volume_db = db_adjust
		add_child(p)
		_2d_p.push_front(p)


func play(audio_stream: AudioStream, global_pos := Vector2.ZERO):
	var p = _get_player(global_pos)
	if (p == null):
		return
	p.stream = audio_stream
	p.pitch_scale = randf_range(.98, 1.02)
	p.play()
	p.finished.connect(_release_player.bind(p), CONNECT_ONE_SHOT)

func _get_player(global_pos := Vector2.ZERO) -> Variant:
	if (global_pos != Vector2.ZERO):
		if _2d_p.is_empty():
			push_warning("Out of spatial audio sources. Consider increasing capacity")
			return
		
		var p = _2d_p.pop_back()
		p.global_position = global_pos
		return p
	else:
		if _static_p.is_empty():
			push_warning("Out of static audio sources. Consider increasing capacity")
			return
		var p = _static_p.pop_back()
		return p

func _release_player(p):
	p.stop()
	p.stream = null
	
	if (p is AudioStreamPlayer2D):
		_2d_p.push_front(p)
	else:
		_static_p.push_front(p)
