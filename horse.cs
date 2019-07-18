using System;

namespace HorseGame
{
    class Horse
    {
        enum StepResult { Boom, Win, Go };
        // дорожка
        int[] _track;
        // генератор случайных чисел
        readonly Random _rnd;
        // скорость игры
        readonly int _speed;
        // Текущая позиция лошадки
        int _horsePosition;
        // Прыжок
        bool _horseJump;
        // его длина
        int _horseJumpLength;
        // кол-во шагов
        int _stepCount;

        // конструктора
        public Horse(int speed)
        {
            // Задаем генератор случайных чисел
            _rnd = new Random();
            // Скорость игры (отрисовки)
            _speed = speed;

            Clear();
        }

        public Horse() : this(250)
        {

        }

        // Сброс игры
        void Clear()
        {
            // Задаем дорожку по размеру консоли
            Array.Resize<int>(ref _track, Console.WindowWidth);
            // Сбрасываем трек
            for (int i = 0; i < _track.Length; i++)
                _track[i] = (i == _track.Length / 2) ? 1 : 0;
            // Лошадь в стартовую позицию
            _horsePosition = 1;
            // Не в прыжке
            _horseJump = false;
            _horseJumpLength = 0;
            _stepCount = 0;
        }

        // старт
        public void StartGame()
        {
            // настройка консоли
            Console.Title = "Лошадка :)";
            Console.Clear();
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.White;
            // правила игры
            Console.SetCursorPosition(0, 2);
            Console.WriteLine("Цель игры - доскакать до финиша, перепрыгивая через препятствия");
            Console.WriteLine("Клавиши управления:");
            Console.WriteLine("Прыжок - Spase");
            Console.WriteLine("Выйти  - Esc");
            Console.WriteLine("Старт  - F1");
            Console.WriteLine("Стоп   - F2");

            // Первая отрисовка игрового поля
            Paint();

            // Че делаем?
            ConsoleKeyInfo input;
            do
            {
                input = Console.ReadKey(true);

                if (input.Key == ConsoleKey.F1 && Run())
                    return;

            } while (input.Key != ConsoleKey.Escape);
        }

        // основной процесс
        bool Run()
        {
            // Заводские настройки
            Clear();

            ConsoleKeyInfo input = new ConsoleKeyInfo();
            do
            {
                if (Console.KeyAvailable)
                {
                    input = Console.ReadKey(true);
                    // останов
                    if (input.Key == ConsoleKey.F2)
                        return false;
                    // прыжок
                    if (!_horseJump && input.Key == ConsoleKey.Spacebar)
                    {
                        _horseJump = true;
                        _horseJumpLength = 0;
                    }

                }

                switch (NextStep())
                {
                    case StepResult.Boom:
                        Paint();
                        Console.Beep();
                        return false;
                    case StepResult.Win:
                        Paint();
                        Console.Beep();
                        return false;
                    case StepResult.Go:
                    default:
                        break;
                }

                Paint();

                if (Console.KeyAvailable)
                {
                    input = Console.ReadKey(true);
                    // останов
                    if (input.Key == ConsoleKey.F2)
                        return false;
                    // прыжок
                    if (!_horseJump && input.Key == ConsoleKey.Spacebar)
                    {
                        _horseJump = true;
                        _horseJumpLength = 0;
                    }

                }

                System.Threading.Thread.Sleep(_speed);

            } while (input.Key != ConsoleKey.Escape);

            return true;
        }

        // пересчитываем следующий шаг
        StepResult NextStep()
        {
            _stepCount++;

            // смещаем массив
            for (int i = 0; i < _track.Length - 1; i++)
                _track[i] = _track[i + 1];
            // в последнюю ячейку загоняем нолик
            _track[_track.Length - 1] = 0;
            // смотрим наличие 1 в последних ячейках
            int sum = 0;
            for (int i = _track.Length - Math.Max((_track.Length - _horsePosition) / 2, 6); i < _track.Length; i++)
                sum += _track[i];
            // если все ок, случайно генерим последнюю ячейку
            if (sum == 0)
                _track[_track.Length - 1] = _rnd.Next(0, 2);
            // лошадка врезалась
            if (_track[_horsePosition] == 1 && !_horseJump)
                return StepResult.Boom;
            // лошадка доскакала
            if (_horsePosition == _track.Length - 1)
                return StepResult.Win;

            _horseJump = (_horseJump & _horseJumpLength == 2) ? false : _horseJump;
            _horseJumpLength++;
            // Двигаем лошарку, каждые 3 хода
            if (_stepCount % 3 == 0)
                _horsePosition++;

            // лошадка врезалась
            if (_track[_horsePosition] == 1 && !_horseJump)
                return StepResult.Boom;
            // лошадка доскакала
            if (_horsePosition == _track.Length - 1)
                return StepResult.Win;
            // все ок
            return StepResult.Go;
        }

        // отрисовка экрана
        void Paint()
        {
            for (int i = 0; i < _track.Length; i++)
            {
                // 1
                Console.SetCursorPosition(i, 15);
                Console.Write((i == _horsePosition && _horseJump) ? '^' : ' ');
                // 2
                Console.SetCursorPosition(i, 16);
                if (i == _horsePosition && !_horseJump && _track[i] == 1)
                {
                    // лошадка долбанулась в барьер
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write('Ж');
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (i == _horsePosition && !_horseJump && _track[i] == 0)
                {
                    // лошадка
                    Console.Write((_stepCount % 2 == 0) ? 'Х' : 'Y');
                }
                else if (_track[i] == 1)
                {
                    // барьер
                    Console.Write('┬');
                }
                else if (_track[i] == 0)
                {
                    // defolt
                    Console.Write(' ');
                }
                // 3
                Console.SetCursorPosition(i, 17);
                if (_track[i] == 1)
                {
                    // барьер
                    Console.Write('┴');
                }
                else if (_track[i] == 0)
                {
                    // defolt
                    Console.Write('─');
                }
            }
        }
    }
}
