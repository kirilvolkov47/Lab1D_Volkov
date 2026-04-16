using System;
using System.Text;

namespace Lab1
{
    class Program
    {
        static double[,] ReadMatrix()
        {
            int rows = 0;
            while (rows <= 0)
            {
                Console.Write("Кількість рядків: ");
                if (!int.TryParse(Console.ReadLine(), out rows) || rows <= 0)
                    Console.WriteLine("Введіть коректне число");
            }

            int cols = 0;
            while (cols <= 0)
            {
                Console.Write("Кількість стовпців: ");
                if (!int.TryParse(Console.ReadLine(), out cols) || cols <= 0)
                    Console.WriteLine("Введіть коректне число");
            }

            double[,] mat = new double[rows, cols];
            Console.WriteLine("Вводіть рядки через пробіл:");
            for (int i = 0; i < rows; i++)
            {
                bool ok = false;
                while (!ok)
                {
                    Console.Write("  Рядок " + (i + 1) + ": ");
                    string[] parts = Console.ReadLine().Split(' ');
                    if (parts.Length != cols)
                    {
                        Console.WriteLine("Невірна кількість елементів");
                        continue;
                    }
                    ok = true;
                    for (int j = 0; j < cols; j++)
                    {
                        if (!double.TryParse(parts[j], out mat[i, j]))
                        {
                            Console.WriteLine("Невірний формат числа");
                            ok = false;
                            break;
                        }
                    }
                }
            }
            return mat;
        }

        static double[] ReadVector(int n)
        {
            while (true)
            {
                Console.Write("Вектор B через пробіл: ");
                string[] parts = Console.ReadLine().Split(' ');
                if (parts.Length != n)
                {
                    Console.WriteLine("Невірна кількість елементів (потрібно " + n + ")");
                    continue;
                }
                double[] vec = new double[n];
                bool ok = true;
                for (int i = 0; i < n; i++)
                {
                    if (!double.TryParse(parts[i], out vec[i]))
                    {
                        Console.WriteLine("Невірний формат числа");
                        ok = false;
                        break;
                    }
                }
                if (ok) return vec;
            }
        }

        static void ReadTask(out double[,] table, out string[] left, out string[] top, out int m)
        {
            m = 0;
            while (m <= 0)
            {
                Console.Write("Кількість обмежень: ");
                if (!int.TryParse(Console.ReadLine(), out m) || m <= 0)
                    Console.WriteLine("Введіть коректне число");
            }

            int n = 0;
            while (n <= 0)
            {
                Console.Write("Кількість змінних: ");
                if (!int.TryParse(Console.ReadLine(), out n) || n <= 0)
                    Console.WriteLine("Введіть коректне число");
            }

            table = new double[m + 1, n + 1];

            Console.WriteLine("Коефіцієнти обмежень + b через пробіл:");
            for (int i = 0; i < m; i++)
            {
                bool ok = false;
                while (!ok)
                {
                    Console.Write("  Обмеження " + (i + 1) + ": ");
                    string[] parts = Console.ReadLine().Split(' ');
                    if (parts.Length != n + 1)
                    {
                        Console.WriteLine("Невірна кількість елементів (потрібно " + (n + 1) + ")");
                        continue;
                    }
                    ok = true;
                    for (int j = 0; j <= n; j++)
                    {
                        if (!double.TryParse(parts[j], out table[i, j]))
                        {
                            Console.WriteLine("Невірний формат числа");
                            ok = false;
                            break;
                        }
                    }
                }
            }

            bool zOk = false;
            while (!zOk)
            {
                Console.Write("Z-рядок (коеф. зі знаком мінус + 0): ");
                string[] zp = Console.ReadLine().Split(' ');
                if (zp.Length != n + 1)
                {
                    Console.WriteLine("Невірна кількість елементів (потрібно " + (n + 1) + ")");
                    continue;
                }
                zOk = true;
                for (int j = 0; j <= n; j++)
                {
                    if (!double.TryParse(zp[j], out table[m, j]))
                    {
                        Console.WriteLine("Невірний формат числа");
                        zOk = false;
                        break;
                    }
                }
            }

            left = null;
            bool leftOk = false;
            while (!leftOk)
            {
                Console.Write("Базисні змінні через пробіл: ");
                string[] lp = Console.ReadLine().Split(' ');
                if (lp.Length != m)
                {
                    Console.WriteLine("Невірна кількість (потрібно " + m + ")");
                    continue;
                }
                leftOk = true;
                left = new string[m + 1];
                for (int i = 0; i < m; i++)
                    left[i] = lp[i];
                left[m] = "Z";
            }

            top = null;
            bool topOk = false;
            while (!topOk)
            {
                Console.Write("Вільні змінні через пробіл: ");
                string[] tp = Console.ReadLine().Split(' ');
                if (tp.Length != n)
                {
                    Console.WriteLine("Невірна кількість (потрібно " + n + ")");
                    continue;
                }
                topOk = true;
                top = new string[n + 1];
                for (int j = 0; j < n; j++)
                    top[j] = tp[j];
                top[n] = "b";
            }
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            double[,] A = {
                { 5, -1, -1 },
                { 3,  2, -1 },
                { 1, -2, -3 }
            };
            double[] B = { 5, 6, 2 };

            // Part B
            double[,] tableVar = {
                {  2,  2,  1,  2,  1,  4 },
                {  2,  1,  0,  3,  2,  4 },
                {  3,  0,  1,  2,  5,  9 },
                { -1,  0,  0,  0, -3,  0 }
            };
            string[] leftVar = { "x6", "x7", "x8", "Z" };
            string[] topVar = { "x1", "x2", "x3", "x4", "x5", "b" };

            // Part C
            double[,] tableVarC = {
                {  2,  2,  1,  2,  1,  4 },
                {  -2,  -1,  0,  -3,  -2,  -4 },
                {  3,  0,  1,  2,  5,  9 },
                { -1,  0,  0,  0, -3,  0 }
            };
            string[] leftVarC = { "01", "x7", "x8", "Z" };
            string[] topVarC = { "x1", "x2", "x3", "x4", "x5", "b" };

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("1 - Знайти обернену матрицю (за варіантом)");
                Console.WriteLine("2 - Обчислити ранг матриці (за варіантом)");
                Console.WriteLine("3 - Розв'язати СЛАР (за варіантом)");
                Console.WriteLine("4 - Знайти обернену матрицю (ввести вручну)");
                Console.WriteLine("5 - Обчислити ранг матриці (ввести вручну)");
                Console.WriteLine("6 - Розв'язати СЛАР (ввести вручну)");
                Console.WriteLine("7 - Знайти опорний розв'язок (за варіантом)");
                Console.WriteLine("8 - Знайти оптимальний розв'язок (за варіантом)");
                Console.WriteLine("9 - Знайти опорний розв'язок (ввести вручну)");
                Console.WriteLine("10 - Знайти оптимальний розв'язок (ввести вручну)");
                Console.WriteLine("11 - Розв'язати Part C (за варіантом)");
                Console.WriteLine("12 - Розв'язати Part C (ввести вручну)");
                Console.WriteLine("13 - Метод Гоморі (за варіантом)");
                Console.WriteLine("14 - Метод Гоморі (ввести вручну)");
                Console.WriteLine("0 - Вихід");
                Console.Write("Введіть номер >> ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        PartA.InverseMatrix(A);
                        break;
                    case "2":
                        PartA.FindRank(A);
                        break;
                    case "3":
                        PartA.SolveLinear(A, B);
                        break;
                    case "4":
                        {
                            double[,] mat = ReadMatrix();
                            PartA.InverseMatrix(mat);
                            break;
                        }
                    case "5":
                        {
                            double[,] mat = ReadMatrix();
                            PartA.FindRank(mat);
                            break;
                        }
                    case "6":
                        {
                            double[,] mat = ReadMatrix();
                            double[] vec = ReadVector(mat.GetLength(0));
                            PartA.SolveLinear(mat, vec);
                            break;
                        }
                    case "7":
                        {
                            double[,] tbl = PartB.CopyMatrix(tableVar);
                            string[] left = (string[])leftVar.Clone();
                            string[] top = (string[])topVar.Clone();
                            PartB.FindBase(tbl, left, top, 3);
                            break;
                        }
                    case "8":
                        {
                            double[,] tbl = PartB.CopyMatrix(tableVar);
                            string[] left = (string[])leftVar.Clone();
                            string[] top = (string[])topVar.Clone();
                            PartB.FindMax(tbl, left, top, 3);
                            break;
                        }
                    case "9":
                        {
                            double[,] tbl;
                            string[] left, top;
                            int m;
                            ReadTask(out tbl, out left, out top, out m);
                            PartB.FindBase(tbl, left, top, m);
                            break;
                        }
                    case "10":
                        {
                            double[,] tbl;
                            string[] left, top;
                            int m;
                            ReadTask(out tbl, out left, out top, out m);
                            PartB.FindMax(tbl, left, top, m);
                            break;
                        }
                    case "11":
                        {
                            double[,] tbl = PartC.CopyMatrix(tableVarC);
                            string[] left = (string[])leftVarC.Clone();
                            string[] top = (string[])topVarC.Clone();
                            if (PartC.DeleteZeroRows(ref tbl, left, ref top, 3))
                            {
                                if (PartC.FindBase(tbl, left, top, 3))
                                    PartC.FindMax(tbl, left, top, 3);
                            }
                            break;
                        }
                    case "12":
                        {
                            double[,] tbl;
                            string[] left, top;
                            int m;
                            ReadTask(out tbl, out left, out top, out m);
                            if (PartC.DeleteZeroRows(ref tbl, left, ref top, m))
                            {
                                if (PartC.FindBase(tbl, left, top, m))
                                    PartC.FindMax(tbl, left, top, m);
                            }
                            break;
                        }
                    case "13":
                        {
                            double[,] tbl = PartD.CopyMatrix(tableVar);
                            string[] left = (string[])leftVar.Clone();
                            string[] top = (string[])topVar.Clone();
                            PartD.GomoryMethod(tbl, left, top, 3, 5);
                            break;
                        }
                    case "14":
                        {
                            double[,] tbl;
                            string[] left, top;
                            int m;
                            ReadTask(out tbl, out left, out top, out m);
                            int n = top.Length - 1;
                            PartD.GomoryMethod(tbl, left, top, m, n);
                            break;
                        }
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Невірний вибір");
                        break;
                }

                Console.WriteLine();
            }
        }
    }
}