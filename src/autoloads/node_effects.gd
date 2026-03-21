extends Node2D

func jiggle(node: CanvasItem, n_jiggles: int = 3, magnitude: float = 10.0, speed: float = 10.0) -> Tween:
	var tween = node.create_tween()

	var duration = (1.0 / speed)
	
	tween.tween_property(node, "rotation_degrees", magnitude / 2, duration / 2).as_relative()

	var dir_sign = -1
	for i in range(n_jiggles - 1):
		tween.tween_property(node, "rotation_degrees", dir_sign * magnitude, duration).as_relative()
		dir_sign = - dir_sign
	
	tween.tween_property(node, "rotation_degrees", dir_sign * magnitude / 2, duration / 2).as_relative()

	return tween

func shake(node: CanvasItem, n_shakes: int = 5, magnitude: float = 10.0, speed: float = 30.0) -> Tween:
	var tween = node.create_tween()
	var duration = (1.0 / speed)
	var original_position = node.position
	var offset = Vector2(randf_range(-magnitude, magnitude), randf_range(-magnitude, magnitude))

	for i in range(n_shakes):
		tween.tween_property(node, "position", offset, duration).as_relative()
		offset = (-offset.rotated(randf_range(-PI / 2, PI / 2))).normalized() * magnitude
	
	tween.tween_property(node, "position", original_position, duration)

	return tween

func move_by(node: CanvasItem, delta: Vector2, duration: float) -> Tween:
	var tween = node.create_tween()
	
	tween.tween_property(node, "position", delta, duration).as_relative()
	
	return tween

func move_to(node: CanvasItem, target: Vector2, duration: float) -> Tween:
	var tween = node.create_tween().set_ease(Tween.EASE_IN_OUT).set_trans(Tween.TRANS_CUBIC)
	tween.tween_property(node, "position", target, duration)
	
	return tween

func dust(node: Control) -> void:
	var n = Create.dust()
	n.setup_up(node.size)
	node.add_child(n)

	var e = Create.dust()
	e.setup_right(node.size)
	node.add_child(e)

	var s = Create.dust()
	s.setup_down(node.size)
	node.add_child(s)

	var w = Create.dust()
	w.setup_left(node.size)
	node.add_child(w)
	
	for child in [n, e, s, w]:
		child.show_behind_parent = true
		child.trigger()

func sheen(node: CanvasItem, duration: float = 0.5) -> Tween:
	var effect = Create.sheen()
	node.add_child(effect)
	var tween = effect.play(duration)
	return tween
