﻿Feature: Survival_DrinkWater
	Чтобы ввести микроменеджмент ресурсов и состояния персонажей
	Как игроку
	Мне нужно, чтобы при употреблении еды повышалась сытость персонажа

@survival @dev0
Scenario: Употребление еды
	Given Есть произвольная карта
	And Есть актёр игрока
	And В инвентаре у актёра есть еда: water количество: 1
	When Актёр использует предмет water на себя
	Then Значение воды повысилось на 10 единиц и уменьшилось на 1 за игровой цикл и стало 59
	And Предмет water отсутствует в инвентаре актёра