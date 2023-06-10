guidebook-reagent-effect-description =
    { $chance ->
        [1] { $effect }
       *[other] Has a { NATURALPERCENT($chance, 2) } chance to { $effect }
    }{ $conditionCount ->
        [0] .
       *[other] { " " }when { $conditions }.
    }
guidebook-reagent-name = [bold][color={ $color }]{ CAPITALIZE($name) }[/color][/bold]
guidebook-reagent-recipes-header = Рецепт
guidebook-reagent-recipes-reagent-display = [bold]{ $reagent }[/bold] \[{ $ratio }\]
guidebook-reagent-recipes-mix = Смешать
guidebook-reagent-effects-header = Эффекты
guidebook-reagent-effects-metabolism-group-rate = [bold]{ $group }[/bold] [color=gray]({ $rate } units per second)[/color]
guidebook-reagent-physical-description = Кажется { $description }.
