using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

namespace Labyrinth
{
	internal delegate bool Transformer(int integer);
	public class Labyrinth
	{
		[DllImport("user32.dll")]
		public static extern int GetAsyncKeyState(int i);
		private readonly Random _rand;
		private Point _begin;
		private Point _end;
		private readonly Stack<Point> _way;
		private int _rows;
		private int _cols;
		private int _animationSpeed;
		private char[,] _field;
		private char[,] _fieldClone;
		private readonly bool[] _exception;
		private bool _animation;
		private char[,] _progress;

		public int AmountOfSuccessfulGenerations { get; set; }
		public int AmountOfFailedGenerations { get; set; }
		public int AmountOfResolvedLabyrinths { get; set; }
		public int AmountOfUnresolvedLabyrinths { get; set; }

		public Labyrinth(int rows, int cols)
		{
			_rand = new Random();
			_begin = new Point(0, 2);
			_end = new Point(rows - 1, cols - 1);
			_way = new Stack<Point>();
			_rows = rows;
			_cols = cols;
			_animationSpeed = 20;
			_exception = new bool[4];
			_animation = false;
			AmountOfSuccessfulGenerations = 0;
			AmountOfFailedGenerations = 0;
			AmountOfResolvedLabyrinths = 0;
			AmountOfUnresolvedLabyrinths = 0;
			Resize(rows, cols);
		}

		public void Build(bool trueOrFalse)
		{
			Console.SetCursorPosition(0, 0);
			_animation = trueOrFalse;
			Fill(9608);
			_way.Clear();
			_way.Push(_begin);
			Array.Clear(_exception, 0, 4);
			var count = 0;
			_field[_begin.X, _begin.Y] = ' ';
			if (trueOrFalse)
			{
				Print(false);
			}
			while (true)
			{
				if (Check())
				{
					AmountOfSuccessfulGenerations++;
					return;
				}

				//if (count > 60)
				//{
				//	randValue = _rand.Next(15);
				//	for (var i = 0; i < randValue; i++)
				//	{
				//		try
				//		{
				//			if (trueOrFalse) Thread.Sleep(_animationSpeed);
				//			temp = _way.Pop();
				//			Draw(temp.Y, temp.X, 32);
				//			temp = _way.Peek();
				//			Draw(temp.Y, temp.X, 8729);
				//		}
				//		catch (InvalidOperationException)
				//		{
				//			Build(trueOrFalse);
				//			AmountOfFailedGenerations++;
				//			return;
				//		}
				//	}
				//	count = 0;
				//	continue;
				//}

				int randLength;
				int randDirection;
				do
				{
					randLength = _rand.Next(4, 6);
					randDirection = _rand.Next(4);
				} while (_exception[randDirection]);
				Transformer builder;
				switch (randDirection)
				{
					case 0:
						builder = BuildUp;
						break;
					case 1:
						builder = BuildRight;
						break;
					case 2:
						builder = BuildDown;
						break;
					default:
						builder = BuildLeft;
						break;
				}
				if (builder(randLength))
				{
					Array.Clear(_exception, 0, 4);
					count++;
				}
				else
				{
					_exception[randDirection] = true;
				}

				if (!_exception[0] || !_exception[1] || !_exception[2] || !_exception[3]) continue;
				{
					Point temp;
					if (count > 30)
					{
						var randValue = _rand.Next(3, 15);
						for (var i = 0; i < randValue; i++)
						{
							try
							{
								if (trueOrFalse) Thread.Sleep(_animationSpeed);
								temp = _way.Pop();
								Draw(temp.Y, temp.X, 32);
								temp = _way.Peek();
								Draw(temp.Y, temp.X, 8729);
							}
							catch (InvalidOperationException)
							{
								Build(trueOrFalse);
								AmountOfFailedGenerations++;
								return;
							}
						}
						count = 0;
					}
					else
					{
						try
						{
							if (trueOrFalse) Thread.Sleep(_animationSpeed);
							temp = _way.Pop();
							Draw(temp.Y, temp.X, 32);
							temp = _way.Peek();
							Draw(temp.Y, temp.X, 8729);
						}
						catch (InvalidOperationException)
						{
							Build(trueOrFalse);
							AmountOfFailedGenerations++;
							return;
						}
					}
					Array.Clear(_exception, 0, 4);
				}
			}
		}

		private bool BuildUp(int length)
		{
			Point pos;
			try
			{
				pos = _way.Peek();
			}
			catch (InvalidOperationException)
			{
				return false;
			}

			if (pos.Y <= 1 || pos.Y >= _cols - 2 || pos.X <= 2) return false;
			while (pos.X - length < 2)
				length--;
			for (var i = pos.X - 1; i > pos.X - length - 1 && length > 2; i--)
				if (_field[i, pos.Y] == ' ' || _field[i, pos.Y - 1] == ' ' || _field[i, pos.Y + 1] == ' ')
				{
					length--;
					i = pos.X;
				}

			if (length <= 2) return false;
			{
				for (var i = pos.X - 1; i > pos.X - length; i--)
				{
					Draw(pos.Y, i + 1, 32);
					Draw(pos.Y, i, 8729);
					_field[i, pos.Y] = ' ';
					_way.Push(new Point(i, pos.Y));
				}
				return true;
			}
		}

		private bool BuildRight(int length)
		{
			Point pos;
			try
			{
				pos = _way.Peek();
			}
			catch (InvalidOperationException)
			{
				return false;
			}

			if (pos.X <= 1 || pos.X >= _rows - 2 || pos.Y >= _cols - 3) return false;
			while (pos.Y + length > _cols - 2)
				length--;
			for (var i = pos.Y + 1; i < pos.Y + length + 1; i++)
				if (_field[pos.X, i] == ' ' || _field[pos.X - 1, i] == ' ' || _field[pos.X + 1, i] == ' ')
				{
					length--;
					i = pos.Y;
				}

			if (length <= 2) return false;
			{
				for (var i = pos.Y + 1; i < pos.Y + length; i++)
				{
					Draw(i - 1, pos.X, 32);
					Draw(i, pos.X, 8729);
					_field[pos.X, i] = ' ';
					_way.Push(new Point(pos.X, i));
				}
				return true;
			}
		}

		private bool BuildDown(int length)
		{
			Point pos;
			try
			{
				pos = _way.Peek();
			}
			catch (InvalidOperationException)
			{
				return false;
			}

			if (pos.Y <= 1 || pos.Y >= _cols - 2 || pos.X >= _rows - 3) return false;
			while (pos.X + length > _rows - 2)
				length--;
			for (var i = pos.X + 1; i < pos.X + length + 1; i++)
				if (_field[i, pos.Y] == ' ' || _field[i, pos.Y - 1] == ' ' || _field[i, pos.Y + 1] == ' ')
				{
					length--;
					i = pos.X;
				}

			if (length <= 2) return false;
			{
				for (var i = pos.X + 1; i < pos.X + length; i++)
				{
					Draw(pos.Y, i - 1, 32);
					Draw(pos.Y, i, 8729);
					_field[i, pos.Y] = ' ';
					_way.Push(new Point(i, pos.Y));
				}
				return true;
			}
		}

		private bool BuildLeft(int length)
		{
			Point pos;
			try
			{
				pos = _way.Peek();
			}
			catch (InvalidOperationException)
			{
				return false;
			}

			if (pos.X <= 1 || pos.X >= _rows - 2 || pos.Y <= 2) return false;
			while (pos.Y - length < 2)
				length--;
			for (var i = pos.Y - 1; i > pos.Y - length - 1; i--)
				if (_field[pos.X, i] == ' ' || _field[pos.X - 1, i] == ' ' || _field[pos.X + 1, i] == ' ')
				{
					length--;
					i = pos.Y;
				}

			if (length <= 2) return false;
			{
				for (var i = pos.Y - 1; i > pos.Y - length; i--)
				{
					Draw(i + 1, pos.X, 32);
					Draw(i, pos.X, 8729);
					_field[pos.X, i] = ' ';
					_way.Push(new Point(pos.X, i));
				}
				return true;
			}
		}

		private bool Check()
		{
			Point pos;
			try
			{
				pos = _way.Peek();
			}
			catch (InvalidOperationException)
			{
				return false;
			}

			if (pos.X <= _end.X - 10 || pos.X >= _end.X || pos.Y <= _end.Y - 10 || pos.Y >= _end.Y) return false;
			while (pos.Y != _cols - 3)
			{
				pos.Y++;
				Draw(pos.Y - 1, pos.X, 32);
				Draw(pos.Y, pos.X, 8729);
				_field[pos.X, pos.Y] = ' ';
				_way.Push(pos);
			}
			while (pos.X != _rows)
			{
				pos.X++;
				if (pos.X == _rows - 1)
				{
					Draw(pos.Y, pos.X, 9829);
					_field[pos.X, pos.Y] = (char)9829; // '#'
					_way.Clear();
					if (_animation) Console.SetCursorPosition(0, _rows);
					FillingTheVoids();
					return true;
				}
				Draw(pos.Y, pos.X - 1, 32);
				Draw(pos.Y, pos.X, 8729);
				_field[pos.X, pos.Y] = ' ';
				_way.Push(pos);
			}
			return false;
		}
		private void FillingTheVoids()
		{

			List<Point> emptyPoints = new List<Point>();
			for (int i = 1; i < _cols - 1; i++)
			{
				_progress[1, i] = (char)32;
			}
			for (int i = 0; i < _rows; i++)
			{
				for (int j = 0; j < _cols; j++)
				{
					if (_field[i, j] == ' ')
					{
						emptyPoints.Add(new Point(i, j));
					}
				}
			}

			int currentSpeed = _animationSpeed;
			_animationSpeed = 1;
			for (int i = 0; i < emptyPoints.Count; i++)
			{
				Build(emptyPoints[i]);
				DrawProgress(i * 100 / (emptyPoints.Count - 1));
			}

			_animationSpeed = currentSpeed;
			if (_animation)
			{
				Console.Clear();
				Print(false);
			}
		}
		private void Build(Point pos)
		{
			_way.Clear();
			_way.Push(pos);
			Array.Clear(_exception, 0, 4);
			var count = 0;
			_field[pos.X, pos.Y] = ' ';
			while (true)
			{
				//if (count > 60)
				//{
				//	randValue = _rand.Next(15);
				//	for (var i = 0; i < randValue; i++)
				//	{
				//		try
				//		{
				//			if (_animation) Thread.Sleep(_animationSpeed);
				//			temp = _way.Pop();
				//			Draw(temp.Y, temp.X, 32);
				//			temp = _way.Peek();
				//			Draw(temp.Y, temp.X, 8729);
				//		}
				//		catch (InvalidOperationException)
				//		{
				//			return;
				//		}
				//	}
				//	count = 0;
				//	continue;
				//}

				int randLength;
				int randDirection;
				do
				{
					randLength = _rand.Next(4, 6);
					randDirection = _rand.Next(4);
				} while (_exception[randDirection]);
				Transformer builder;
				switch (randDirection)
				{
					case 0:
						builder = BuildUp;
						break;
					case 1:
						builder = BuildRight;
						break;
					case 2:
						builder = BuildDown;
						break;
					default:
						builder = BuildLeft;
						break;
				}
				if (builder(randLength))
				{
					Array.Clear(_exception, 0, 4);
					count++;
				}
				else
				{
					_exception[randDirection] = true;
				}

				if (!_exception[0] || !_exception[1] || !_exception[2] || !_exception[3]) continue;
				{
					Point temp;
					if (count > 30)
					{
						var randValue = _rand.Next(3, 15);
						for (var i = 0; i < randValue; i++)
						{
							try
							{
								if (_animation) Thread.Sleep(_animationSpeed);
								temp = _way.Pop();
								Draw(temp.Y, temp.X, 32);
								temp = _way.Peek();
								Draw(temp.Y, temp.X, 8729);
							}
							catch (InvalidOperationException)
							{
								return;
							}
						}
						count = 0;
					}
					else
					{
						try
						{
							if (_animation) Thread.Sleep(_animationSpeed);
							temp = _way.Pop();
							Draw(temp.Y, temp.X, 32);
							temp = _way.Peek();
							Draw(temp.Y, temp.X, 8729);
						}
						catch (InvalidOperationException)
						{
							return;
						}
					}
					Array.Clear(_exception, 0, 4);
				}
			}
		}
		public void Print(bool type)
		{
			for (var i = 0; i < _rows; i++)
			{
				for (var j = 0; j < _cols; j++)
				{

					if (type)
					{
						switch (_fieldClone[i, j])
						{
							case (char)8592:
							case (char)8593:
							case (char)8594:
							case (char)8595:
								Console.ForegroundColor = ConsoleColor.DarkGreen;
								break;
							case (char)9830:
								Console.ForegroundColor = ConsoleColor.DarkCyan;
								break;
							case (char)9608:
							case (char)9618:
								Console.ForegroundColor = ConsoleColor.DarkBlue;
								break;
							case (char)9552:
							case (char)9553:
							case (char)9556:
							case (char)9562:
							case (char)9559:
							case (char)9565:
								Console.ForegroundColor = ConsoleColor.DarkRed;
								break;
							default:
								Console.ForegroundColor = ConsoleColor.Gray;
								break;
						}
						Console.Write(_fieldClone[i, j]);
					}
					else
					{
						if (_field[i, j] == (char) 9608)
						{
							Console.ForegroundColor = ConsoleColor.DarkBlue;
						}
						else if (_field[i, j] == (char) 9829)
						{
							Console.ForegroundColor = ConsoleColor.White;
						}
						else
						{
							Console.ForegroundColor = ConsoleColor.DarkRed;
						}
						Console.Write(_field[i, j]);
					}
				}
				Console.WriteLine();
			}
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		private void Fill(int code)
		{
			for (var i = 0; i < _rows; i++)
			{
				for (var j = 0; j < _cols; j++)
				{
					if ((i == 0 || i == _rows - 1) && j != 0 && j != _cols - 1)
					{
						_field[i, j] = (char)9552;
					}
					else if ((j == 0 || j == _cols - 1) && i != 0 && i != _rows - 1)
					{
						_field[i, j] = (char)9553;
					}
					else
					{
						_field[i, j] = (char)code;
					}
				}
			}
			_field[0, 0] = (char)9556;
			_field[_rows - 1, 0] = (char)9562;
			_field[0, _cols - 1] = (char)9559;
			_field[_rows - 1, _cols - 1] = (char)9565;
		}
		public void SearchBegin(bool trueOrFalse)
		{
			_way.Clear();
			_animation = trueOrFalse;
			_fieldClone = (char[,])_field.Clone();
			if (Scan(_begin))
			{
				AmountOfResolvedLabyrinths++;
			}
			else
			{
				AmountOfUnresolvedLabyrinths++;
			}
			Console.ForegroundColor = ConsoleColor.Gray;
			_animation = false;
		}
		public bool Scan(Point pos)
		{
			var temp = new Point();
			int count;
			_way.Push(pos);
			do
			{
				count = 0;
				if (CheckUp(pos)) count++;
				if (CheckRight(pos)) count++;
				if (CheckDown(pos)) count++;
				if (CheckLeft(pos)) count++;

				if (count != 1) continue;
				if (CheckUp(pos))
				{
					Draw(pos.Y, pos.X, 8593);
					_fieldClone[pos.X, pos.Y] = (char)8593; // нашу текущую позицию отмечаем как пройденной меткой "стрелка"
					if (_fieldClone[pos.X - 1, pos.Y] == (char)9829) // если обнаружен символ выхода из лабиринта wchar_t(9829) возвращаем true
					{
						pos.X--;
						_way.Push(pos);
						if (_animation) Console.SetCursorPosition(0, _rows);
						return true;
					}
					pos.X--;
					_way.Push(pos);
				}
				else if (CheckRight(pos))
				{
					Draw(pos.Y, pos.X, 8594);
					_fieldClone[pos.X, pos.Y] = (char)8594;
					if (_fieldClone[pos.X, pos.Y + 1] == (char)9829)
					{
						pos.Y++;
						_way.Push(pos);
						if (_animation) Console.SetCursorPosition(0, _rows);
						return true;
					}
					pos.Y++;
					_way.Push(pos);
				}
				else if (CheckDown(pos))
				{
					Draw(pos.Y, pos.X, 8595);
					_fieldClone[pos.X, pos.Y] = (char)8595;
					if (_fieldClone[pos.X + 1, pos.Y] == (char)9829)
					{
						pos.X++;
						_way.Push(pos);
						if (_animation) Console.SetCursorPosition(0, _rows);
						return true;
					}
					pos.X++;
					_way.Push(pos);
				}
				else
				{
					Draw(pos.Y, pos.X, 8592);
					_fieldClone[pos.X, pos.Y] = (char)8592;
					if (_fieldClone[pos.X, pos.Y - 1] == (char)9829)
					{
						pos.Y--;
						_way.Push(pos);
						if (_animation) Console.SetCursorPosition(0, _rows);
						return true;
					}
					pos.Y--;
					_way.Push(pos);
				}
			} while (count == 1);
			if (count > 1 && count < 4)
			{
				Draw(pos.Y, pos.X, 9830);
				if (CheckUp(pos)) // если есть проход на верх
				{
					_fieldClone[pos.X, pos.Y] = (char)9830; // на текущую позицию ставлю метку разветления пути "S"
					if (_fieldClone[pos.X - 1, pos.Y] == (char)9829)
					{
						pos.X--;
						_way.Push(pos);
						if (_animation) Console.SetCursorPosition(0, _rows);
						return true;
					}
					temp.X = pos.X - 1;
					temp.Y = pos.Y;
					if (Scan(temp)) // запускаю рекурсивную функцию для иследования разветленного прохода
					{
						return true;
					}
				}
				if (CheckRight(pos)) // [ниже анологичные действия см. строки 114 - 121]
				{
					_fieldClone[pos.X, pos.Y] = (char)9830;
					if (_fieldClone[pos.X, pos.Y + 1] == (char)9829)
					{
						pos.Y++;
						_way.Push(pos);
						if (_animation) Console.SetCursorPosition(0, _rows);
						return true;
					}
					temp.X = pos.X;
					temp.Y = pos.Y + 1;
					if (Scan(temp))
					{
						return true;
					}
				}
				if (CheckDown(pos)) // [ниже анологичные действия см. строки 114 - 121]
				{
					_fieldClone[pos.X, pos.Y] = (char)9830;
					if (_fieldClone[pos.X + 1, pos.Y] == (char)9829)
					{
						pos.X++;
						_way.Push(pos);
						if (_animation) Console.SetCursorPosition(0, _rows);
						return true;
					}
					temp.X = pos.X + 1;
					temp.Y = pos.Y;
					if (Scan(temp))
					{
						return true;
					}
				}
				if (CheckLeft(pos)) // [ниже анологичные действия см. строки 114 - 121]
				{
					_fieldClone[pos.X, pos.Y] = (char)9830;
					if (_fieldClone[pos.X, pos.Y - 1] == (char)9829)
					{
						pos.Y--;
						_way.Push(pos);
						if (_animation) Console.SetCursorPosition(0, _rows);
						return true;
					}
					temp.X = pos.X;
					temp.Y = pos.Y - 1;
					if (Scan(temp))
					{
						return true;
					}
				}
			}

			if (count != 0) return false;
			while (true) // бесконечный цикл
			{
				// если метка на текущей позиции не равна метке разветления "wchar_t(9830)" или если метка является меткой разветления но вокруг нету проходов.
				if ((_fieldClone[pos.X, pos.Y] != (char)9830) || (_fieldClone[pos.X, pos.Y] == (char)9830 && !CheckUp(pos) && !CheckRight(pos) && !CheckDown(pos) && !CheckLeft(pos)))
				{
					Draw(pos.Y, pos.X, 9618);
					_fieldClone[pos.X, pos.Y] = (char)9618; // отмечаем текущую позицию как тупиковый проход
				}
				if (_fieldClone[pos.X, pos.Y] == (char)9830) // если мы дошли до позиции с меткой разветления "wchar_t(9830)"
				{
					_way.Push(pos);
					break; // выходим из бесконечного цикла
				}
				try
				{
					pos = _way.Pop();
				}
				catch (InvalidOperationException)
				{

				}
			}
			return false;
		}
		private bool CheckUp(Point pos) // ФУНКЦИЯ ПРОВЕРЯЕТ ЧТО НАХОДИТСЯ СВЕРХУ ОТ ТЕКУЩЕЙ ПОЗИЦИИ (ПРИНИМАЕТ КООРДИНАТЫ ТЕКУЩЕЙ ПОЗИЦИИ)
		{
			return pos.X != 0 && (_fieldClone[pos.X - 1, pos.Y] == ' ' || _fieldClone[pos.X - 1, pos.Y] == (char)9829);
		}

		private bool CheckRight(Point pos)  // [ниже анологичные действия см. строки 170 - 174]
		{
			return pos.Y != _cols - 1 && (_fieldClone[pos.X, pos.Y + 1] == ' ' || _fieldClone[pos.X, pos.Y + 1] == (char)9829);
		}

		public bool CheckDown(Point pos)  // [ниже анологичные действия см. строки 170 - 174]
		{
			return pos.X != _rows - 1 && (_fieldClone[pos.X + 1, pos.Y] == ' ' || _fieldClone[pos.X + 1, pos.Y] == (char)9829);
		}

		public bool CheckLeft(Point pos)  // [ниже анологичные действия см. строки 170 - 174]
		{
			return pos.Y != 0 && (_fieldClone[pos.X, pos.Y - 1] == ' ' || _fieldClone[pos.X, pos.Y - 1] == (char)9829);
		}
		public void SetAnimationSpeed(int speed)
		{
			if (speed >= 0 && speed <= 1000)
			{
				_animationSpeed = speed;
			}
			else
			{
				_animationSpeed = 20;
			}
		}
		public void ShowCoordinates()
		{
			var count = _way.Count();

			foreach (var item in _way)
			{
				Console.WriteLine(count-- + "\t[" + item.X + ", " + item.Y + "]");
			}
		}
		public void Resize(int rows, int cols)
		{
			if (cols > Console.BufferWidth - 2)
			{
				cols = Console.BufferWidth - 2;
			}

			if (rows > Console.BufferHeight - 2)
			{
				rows = Console.BufferHeight - 2;
			}

			if (rows < 10)
			{
				rows = 10;
			}

			if (cols < 10)
			{
				cols = 10;
			}
			_rows = rows;
			_cols = cols;
			_field = new char[_rows, _cols];
			_fieldClone = new char[_rows, _cols];
			_end = new Point(_rows - 1, _cols - 1);
			_progress = new char[3, _cols];
			for (var i = 0; i < 3; i++)
			{
				for (var j = 0; j < _cols; j++)
				{
					if ((i == 0 || i == 3 - 1) && j != 0 && j != _cols - 1)
					{
						_progress[i, j] = (char)9552;
					}
					else if ((j == 0 || j == _cols - 1) && i != 0 && i != 3 - 1)
					{
						_progress[i, j] = (char)9553;
					}
					else
					{
						_progress[i, j] = (char)32;
					}
				}
			}
			_progress[0, 0] = (char)9556;
			_progress[3 - 1, 0] = (char)9562;
			_progress[0, _cols - 1] = (char)9559;
			_progress[3 - 1, _cols - 1] = (char)9565;
		}
		// Функция для прорисовки лабиринта в реальном времени
		public void Draw(int left, int top, int code)
		{
			if (!_animation) return;

			switch (code)
			{
				case 8592:
				case 8593:
				case 8594:
				case 8595:
					Console.ForegroundColor = ConsoleColor.DarkGreen;
					break;
				case 9830:
					Console.ForegroundColor = ConsoleColor.DarkCyan;
					break;
				case 9608:
				case 9618:
					Console.ForegroundColor = ConsoleColor.DarkBlue;
					break;
				case 8729:
					Console.ForegroundColor = ConsoleColor.DarkMagenta;
					break;
				default:
					Console.ForegroundColor = ConsoleColor.DarkRed;
					break;
			}
			Console.SetCursorPosition(left, top);
			Console.Write((char)code);
			Thread.Sleep(_animationSpeed);
			Console.ForegroundColor = ConsoleColor.Gray;

		}
		private void DrawProgress(int percent)
		{
			if (!_animation) return;
			Console.SetCursorPosition(0, _rows + 2);
			int size = percent * (_cols - 1) / 100;
			for (int i = 1; i < size; i++)
			{
				_progress[1, i] = (char)9608;
			}
			Console.WriteLine("Заполнение пустот...");
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < _cols; j++)
				{
					Console.ForegroundColor = _progress[i, j] == 9608 ? ConsoleColor.DarkGreen : ConsoleColor.Gray;
					Console.Write(_progress[i, j]);
				}
				Console.WriteLine();
			}
			Console.WriteLine("Процент: " + percent + "%");
		}
		public void Editor()
		{
			var pos = new Point(_begin.X + 2, _begin.Y);
			Console.SetCursorPosition(0, _rows + 5);
			Console.WriteLine("1 - перемещение, 2 - разрушение, 3 - строение, 4 - очистить всё, 5 - заполнить всё, 6 - сгенерировать новый");
			Console.WriteLine("\nСтрелки или A, W, S, D - для управления, ESC - выход.");
			Console.ForegroundColor = ConsoleColor.DarkMagenta;
			Console.SetCursorPosition(pos.Y, pos.X);
			var mode = 1;
			Console.Write((char)8729);

			int[] keys = { 27, 37, 38, 39, 40, 65, 68, 83, 87, 97, 98, 99, 100, 101, 102 };
			while (true)
			{
				Thread.Sleep(1);
				foreach (var key in keys) // перебор кодов клавиш
				{
					var keyState = GetAsyncKeyState(key);
					if (keyState != 1 && keyState != -32767) continue;
					switch (key)
					{
						case 65:
						case 37: // влево
							if (pos.Y - 1 > 1)
							{
								Console.SetCursorPosition(pos.Y, pos.X);
								if (mode == 2)
								{
									Console.Write(' ');
									_field[pos.X, pos.Y] = ' ';
								}
								else if (mode == 3)
								{
									Console.ForegroundColor = ConsoleColor.DarkBlue;
									Console.Write((char)9608);
									_field[pos.X, pos.Y] = (char)9608;
								}
								else
								{
									Console.ForegroundColor = ConsoleColor.DarkBlue;
									Console.Write(_field[pos.X, pos.Y]);
								}
								Console.SetCursorPosition(pos.Y - 1, pos.X);
								Console.ForegroundColor = ConsoleColor.DarkMagenta;
								Console.Write((char)8729);
								pos.Y -= 1;
							}
							break;
						case 87:
						case 38: // вверх
							if (pos.X - 1 > 1)
							{
								Console.SetCursorPosition(pos.Y, pos.X);
								if (mode == 2)
								{
									Console.Write(' ');
									_field[pos.X, pos.Y] = ' ';
								}
								else if (mode == 3)
								{
									Console.ForegroundColor = ConsoleColor.DarkBlue;
									Console.Write((char)9608);
									_field[pos.X, pos.Y] = (char)9608;
								}
								else
								{
									Console.ForegroundColor = ConsoleColor.DarkBlue;
									Console.Write(_field[pos.X, pos.Y]);
								}
								Console.ForegroundColor = ConsoleColor.DarkMagenta;
								Console.SetCursorPosition(pos.Y, pos.X - 1);
								Console.Write((char)8729);
								pos.X -= 1;
							}
							break;
						case 68:
						case 39: // вправо
							if (pos.Y + 1 < _cols - 2)
							{
								Console.SetCursorPosition(pos.Y, pos.X);
								if (mode == 2)
								{
									Console.Write(' ');
									_field[pos.X, pos.Y] = ' ';
								}
								else if (mode == 3)
								{
									Console.ForegroundColor = ConsoleColor.DarkBlue;
									Console.Write((char)9608);
									_field[pos.X, pos.Y] = (char)9608;
								}
								else
								{
									Console.ForegroundColor = ConsoleColor.DarkBlue;
									Console.Write(_field[pos.X, pos.Y]);
								}
								Console.SetCursorPosition(pos.Y + 1, pos.X);
								Console.ForegroundColor = ConsoleColor.DarkMagenta;
								Console.Write((char)8729);
								pos.Y += 1;
							}
							break;
						case 83:
						case 40: // вниз
							if (pos.X + 1 < _rows - 2)
							{
								Console.SetCursorPosition(pos.Y, pos.X);
								if (mode == 2)
								{
									Console.Write(' ');
									_field[pos.X, pos.Y] = ' ';
								}
								else if (mode == 3)
								{
									Console.ForegroundColor = ConsoleColor.DarkBlue;
									Console.Write((char)9608);
									_field[pos.X, pos.Y] = (char)9608;
								}
								else
								{
									Console.ForegroundColor = ConsoleColor.DarkBlue;
									Console.Write(_field[pos.X, pos.Y]);
								}
								Console.SetCursorPosition(pos.Y, pos.X + 1);
								Console.ForegroundColor = ConsoleColor.DarkMagenta;
								Console.Write((char)8729);
								pos.X += 1;
							}
							break;
						case 97: // 1
							mode = 1;
							Console.SetCursorPosition(0, _rows + 2);
							Console.ForegroundColor = ConsoleColor.Gray;
							Console.WriteLine("{0, -30}", "Режим перемещения");
							break;
						case 98: // 2
							mode = 2;
							Console.SetCursorPosition(0, _rows + 2);
							Console.ForegroundColor = ConsoleColor.DarkMagenta;
							Console.WriteLine("{0, -30}", "Режим разрушения");
							break;
						case 99: // 3
							mode = 3;
							Console.SetCursorPosition(0, _rows + 2);
							Console.ForegroundColor = ConsoleColor.DarkGreen;
							Console.WriteLine("{0, -30}", "Режим строительства");
							break;
						case 100: // 4
							for (int i = 2; i < _rows - 2; i++)
							{
								for (int j = 2; j < _cols - 2; j++)
								{
									_field[i, j] = ' ';
								}
							}
							Console.SetCursorPosition(0, 0);
							Print(false);
							break;
						case 101: // 5
							for (int i = 2; i < _rows - 2; i++)
							{
								for (int j = 2; j < _cols - 2; j++)
								{
									_field[i, j] = (char)9608;
								}
							}
							Console.SetCursorPosition(0, 0);
							Print(false);
							break;
						case 102: // 6
							Console.SetCursorPosition(0, 0);
							Build(false);
							Print(false);
							break;
						case 27: // ESC
							return;
					}
				}
			}
		}
		public void Save(string filename = "Labyrinth.txt")
		{
			var sw = new StreamWriter(filename, false);
			for (var i = 0; i < _rows; i++)
			{
				for (var j = 0; j < _cols; j++)
				{
					sw.Write(_field[i, j]);
				}
				sw.Write("\n");
			}
			sw.Write("\n");
			for (var i = 0; i < _rows; i++)
			{
				for (var j = 0; j < _cols; j++)
				{
					sw.Write(_fieldClone[i, j]);
				}
				sw.Write("\n");
			}
			sw.Close();
		}
	}
}