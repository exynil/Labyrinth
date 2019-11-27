using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Labyrinth
{
    internal class Program
	{
		
		// подключено для считывания нажатия клавиш
		[DllImport("user32.dll")]
		public static extern int GetAsyncKeyState(int i); // подключено для считывания нажатия клавиш

		// подключено для развертывания приложения на весь экран
		private const int StdOutputHandle = -11;

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetStdHandle(int handle);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleDisplayMode(IntPtr consoleHandle, uint flags, IntPtr newScreenBufferDimensions);
		private static void Main()
		{
			//Registry.SetValue("HKEY_CURRENT_USER\\Console", "ColorTable00", 1842204); // фон
			//Registry.SetValue("HKEY_CURRENT_USER\\Console", "ColorTable01", 6124952); // стены лабиринта
			//Registry.SetValue("HKEY_CURRENT_USER\\Console", "ColorTable02", 6009664); // стрелки
			//Registry.SetValue("HKEY_CURRENT_USER\\Console", "ColorTable03", 6458762); // разветление
			//Registry.SetValue("HKEY_CURRENT_USER\\Console", "ColorTable04", 726224);
			var hConsole = GetStdHandle(StdOutputHandle);
			SetConsoleDisplayMode(hConsole, 1, IntPtr.Zero);
			//Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
			int lrow = Console.WindowHeight - 15;
			int lcol = Console.WindowWidth - 2;
			//Console.SetWindowSize(lcol + 5, lrow + 10);
			var labyrinth = new Labyrinth(lrow, lcol);
			//Console.CursorVisible = false;
			labyrinth.Build(false);
			labyrinth.SearchBegin(false);
			labyrinth.Print(false);
			Console.WriteLine("\nЛабиринт №: " + labyrinth.AmountOfSuccessfulGenerations);
			while (true)
			{
				Console.WriteLine("\n1 - сгенерировать без анимации.          7 - показать координаты от входа до выхода.");
				Console.WriteLine("2 - сгенерировать с анимацией.           8 - изменить размеры лабиринта.");
				Console.WriteLine("3 - поиск выхода без анимации.           9 - сохранить исходный и решенный лабиринт в файл.");
				Console.WriteLine("4 - поиск выхода с анимацией.           10 - запустить тестирование.");
				Console.WriteLine("5 - изменить скорость анимации.         11 - редактировать лабиринт.");
				Console.WriteLine("6 - показать последний.                 12 - создатель.");
				Console.WriteLine("0 - выход.                           Enter - сгенерировать и найти выход.");
				Console.Write("\nВввод:  \b");
				int choice = -1;
				try
				{
					choice = Convert.ToInt32(Console.ReadLine());
				}
				catch (FormatException)
				{
					Console.Clear();
					labyrinth.Build(false);
					labyrinth.SearchBegin(false);
					labyrinth.Print(true);
					Console.WriteLine("\nЛабиринт №: " + labyrinth.AmountOfSuccessfulGenerations);
				}
				if (choice == 0)
					break;
				Console.CursorVisible = false;
				switch (choice)
				{
					case 1:
					{
						Console.Clear();
						labyrinth.Build(false);
						labyrinth.Print(false);
						Console.WriteLine("\nЛабиринт №: " + labyrinth.AmountOfSuccessfulGenerations);
						break;
					}
					case 2:
					{
						Console.Clear();
						labyrinth.Build(true);
						Console.WriteLine("\nЛабиринт №: " + labyrinth.AmountOfSuccessfulGenerations);
						break;
					}
					case 3:
					{
						Console.Clear();
						labyrinth.SearchBegin(false);
						labyrinth.Print(true);
						Console.WriteLine("\nРешеный лабиринт №: " + labyrinth.AmountOfSuccessfulGenerations);
						break;
					}
					case 4:
					{
						Console.Clear();
						labyrinth.Print(false);
						labyrinth.SearchBegin(true);
						Console.WriteLine("\nАнимация поиска выхода из лабиринта №: " + labyrinth.AmountOfSuccessfulGenerations);
						break;
					}
					case 5:
					{
						Console.Write("\nВведите скорость анимации (0 - 1000): ");
						var animationSpeed = Convert.ToInt32(Console.ReadLine());
						labyrinth.SetAnimationSpeed(1000 - animationSpeed);
						goto case 6;
					}
					case 6:
					{
						Console.Clear();
						labyrinth.Print(false);
						Console.WriteLine("\nЛабиринт №: " + labyrinth.AmountOfSuccessfulGenerations);
						break;
					}
					case 7:
					{
						Console.WriteLine("\nКоординаты от входа до выхода лабиринта №: " + labyrinth.AmountOfSuccessfulGenerations);
						labyrinth.SearchBegin(false);
						labyrinth.ShowCoordinates();
						break;
					}
					case 8:
					{
						Console.Write("Введите высоту (минимум 10, максимум " + (Console.BufferHeight - 2) + "): ");
						var rows = Convert.ToInt32(Console.ReadLine());
						Console.Write("Введите ширину (минимум 10, максимум " + (Console.BufferWidth - 2) + "): ");
						var cols = Convert.ToInt32(Console.ReadLine());
						labyrinth.Resize(rows, cols);
						Console.WriteLine("Размеры лабиринта изменены.");
						Console.Clear();
						labyrinth.Build(false);
						labyrinth.Print(false);
						break;
					}
					case 9:
					{
						Console.Clear();
						labyrinth.Print(false);
						labyrinth.Save();
						Console.WriteLine("\nЛабиринт №: " + labyrinth.AmountOfSuccessfulGenerations + " сохранен в файл Labyrinth.txt");
						break;
					}
					case 10:
					{
						Console.Clear();
						Directory.CreateDirectory("saves");
						DateTime startDateTime = DateTime.Now;
						labyrinth.AmountOfSuccessfulGenerations = 0;
						labyrinth.AmountOfFailedGenerations = 0;
						labyrinth.AmountOfResolvedLabyrinths = 0;
						labyrinth.AmountOfUnresolvedLabyrinths = 0;
						while (true)
						{
							labyrinth.SetAnimationSpeed(1);
							labyrinth.Build(false);
							labyrinth.Print(false);
							labyrinth.SearchBegin(true);
							//labyrinth.Save("saves\\" + labyrinth.AmountOfSuccessfulGenerations.ToString() + ".txt");
							if (GetAsyncKeyState(27) == 1 || GetAsyncKeyState(27) == -32767)
							{
								Console.Clear();
								labyrinth.Print(false);
								break;
							}

							Console.ForegroundColor = ConsoleColor.Gray;
							Console.Write("\nУдачных генераций: ");
							Console.ForegroundColor = ConsoleColor.DarkGreen;
							Console.WriteLine(labyrinth.AmountOfSuccessfulGenerations);
							Console.ForegroundColor = ConsoleColor.Gray;
							Console.Write("Не удачных генераций: ");
							Console.ForegroundColor = ConsoleColor.DarkMagenta;
							Console.WriteLine(labyrinth.AmountOfFailedGenerations);
							Console.ForegroundColor = ConsoleColor.Gray;
							Console.Write("Решенных лабиринтов: ");
							Console.ForegroundColor = ConsoleColor.DarkGreen;
							Console.WriteLine(labyrinth.AmountOfResolvedLabyrinths);
							Console.ForegroundColor = ConsoleColor.Gray;
							Console.Write("Не решенных лабиринтов: ");
							Console.ForegroundColor = ConsoleColor.DarkMagenta;
							Console.WriteLine(labyrinth.AmountOfUnresolvedLabyrinths);
							Console.ForegroundColor = ConsoleColor.Gray;
							Console.Write("Текущее поколение: ");
							Console.ForegroundColor = ConsoleColor.DarkGreen;
							Console.WriteLine(GC.GetGeneration(labyrinth));
							Console.ForegroundColor = ConsoleColor.Gray;
							Console.Write("Объем памяти объекта: ");
							Console.ForegroundColor = ConsoleColor.DarkGreen;
							Console.WriteLine("{0, -20:N0}", GC.GetTotalMemory(false));
							Console.ForegroundColor = ConsoleColor.Gray;
							Console.Write("Общий объем памяти: ");
							Console.ForegroundColor = ConsoleColor.DarkGreen;
							Console.WriteLine("{0, -20:N0}", Process.GetProcessesByName("Labyrinth")[0].WorkingSet64);
							Console.ForegroundColor = ConsoleColor.Gray;
							Console.Write("Время: ");
							Console.ForegroundColor = ConsoleColor.DarkGreen;
							var curreDateTime = DateTime.Now - startDateTime;
							Console.WriteLine(curreDateTime.ToString(@"hh\:mm\:ss"));
							Console.ForegroundColor = ConsoleColor.Gray;
							Console.WriteLine("ESC - выхода (необходимо подождать завершения поиска выхода)");
						}

						labyrinth.SetAnimationSpeed(20);
						break;
					}
					case 11:
					{
						Console.Clear();
						labyrinth.Print(false);
						labyrinth.Editor();
						Console.Clear();
						labyrinth.Print(false);
						Console.WriteLine("\nЛабиринт №: " + labyrinth.AmountOfSuccessfulGenerations);
						break;
					}
					case 12:
					{
						Console.Clear();
						labyrinth.Print(false);
						Console.ForegroundColor = ConsoleColor.DarkGreen;
						Console.WriteLine("\nСоздатель: Ким Максим. Год: 2018. Версия: 0.2");
						Console.ForegroundColor = ConsoleColor.Gray;
						break;
					}
				}
			}
		}
	}
}