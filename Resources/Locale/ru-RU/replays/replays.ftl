# Экран загрузки

replay-loading = Загрузка ({ $cur }/{ $total })
replay-loading-reading = Чтение файлов
replay-loading-processing = Обработка файлов
replay-loading-spawning = Порождение сущностей
replay-loading-initializing = Инициализация сущностей
replay-loading-starting = Запуск сущностей
replay-loading-failed =
    Не удалось загрузить воспроизведение:
    { $reason }
# Главное меню
replay-menu-subtext = Клиент воспроизведения
replay-menu-load = Загрузить выбранный повтор
replay-menu-select = Выбрать воспроизведение
replay-menu-open = Открыть папку воспроизведения
replay-menu-none = Не найдено ни одного повтора.
# Информационное окно главного меню
replay-info-title = Информация о воспроизведении
replay-info-none-selected = Повтор не выбран
replay-info-invalid = [color=red]Выбран недействительный повтор[/color]
replay-info-info =
    { "[" }color=gray]Выбрано:[/color] { $name } ({ $file })
    { "[" }color=gray]Время:[/color] { $time }
    { "[" }color=gray]ID раунда:[/color] { $roundId }
    { "[" }color=gray]Продолжительность:[/color] { $duration }
    { "[" }color=gray]ForkId:[/color] { $forkId }
    { "[" }color=gray]Версия:[/color] { $version }
    { "[" }color=gray]Engine:[/color] { $engVersion }
    { "[" }color=gray]Type Hash:[/color] { $hash }
    { "[" }color=gray]Comp Hash:[/color] { $compHash }
# Окно выбора воспроизведения
replay-menu-select-title = Выбрать воспроизведение
# Глаголы, связанные с воспроизведением
replay-verb-spectate = Смотреть
# команда
cmd-replay-spectate-help = replay_spectate [сущность]
cmd-replay-spectate-desc = Прикрепляет или открепляет локального игрока к заданному uid сущности.
cmd-replay-spectate-hint = Необязательный идентификатор сущности
