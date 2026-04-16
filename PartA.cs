using System;

namespace Lab1
{
    class PartA
    {
        public static void TablePrint(double[,] table, string[] left, string[] top, string label = "")
        {
            if (label != "")
                Console.WriteLine(label);

            string line = "      ";
            for (int j = 0; j < top.Length; j++)
                line += top[j].PadLeft(12);
            Console.WriteLine(line);

            for (int i = 0; i < table.GetLength(0); i++)
            {
                line = left[i].PadRight(6);
                for (int j = 0; j < table.GetLength(1); j++)
                    line += table[i, j].ToString("F4").PadLeft(12);
                Console.WriteLine(line);
            }
            Console.WriteLine();
        }

        // крок ЗЖВ
        public static bool JordanStep(double[,] table, int r, int c, string[] left, string[] top)
        {
            int h = table.GetLength(0);
            int w = table.GetLength(1);
            double mainElement = table[r, c];

            if (Math.Abs(mainElement) < 1e-9)
            {
                Console.WriteLine("  [" + r + "][" + c + "] = 0, пропуск");
                return false;
            }

            Console.WriteLine("  Розв. ел. [" + r + "][" + c + "] = " + mainElement.ToString("F4"));

            double[,] saved = CopyMatrix(table);

            table[r, c] = 1.0;

            for (int j = 0; j < w; j++)
            {
                if (j != c)
                    table[r, j] = -saved[r, j];
            }

            for (int i = 0; i < h; i++)
            {
                if (i == r) continue;
                for (int j = 0; j < w; j++)
                {
                    if (j == c) continue;
                    table[i, j] = saved[i, j] * mainElement - saved[i, c] * saved[r, j];
                }
            }

            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    table[i, j] = table[i, j] / mainElement;

            string temp = left[r];
            left[r] = top[c];
            top[c] = temp;

            return true;
        }

        public static double[,] CopyMatrix(double[,] source)
        {
            int h = source.GetLength(0);
            int w = source.GetLength(1);
            double[,] copy = new double[h, w];
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    copy[i, j] = source[i, j];
            return copy;
        }

        // обернена через ЗЖВ по діагоналі
        public static double[,] InverseMatrix(double[,] source)
        {
            int n = source.GetLength(0);
            double[,] table = CopyMatrix(source);

            string[] left = new string[n];
            string[] top = new string[n];
            for (int i = 0; i < n; i++)
            {
                left[i] = "y" + (i + 1);
                top[i] = "x" + (i + 1);
            }

            Console.WriteLine("Обернена матриця");
            TablePrint(table, left, top, "Початок:");

            bool[] finished = new bool[n];
            int stepCount = 0;

            for (int i = 0; i < n; i++)
            {
                if (Math.Abs(table[i, i]) < 1e-9)
                {
                    Console.WriteLine("Нуль на [" + i + "][" + i + "], пропуск");
                    continue;
                }
                stepCount++;
                Console.WriteLine("Крок " + stepCount);
                if (JordanStep(table, i, i, left, top))
                {
                    finished[i] = true;
                    TablePrint(table, left, top, "Таблиця:");
                }
            }

            for (int i = 0; i < n; i++)
            {
                if (finished[i]) continue;
                if (Math.Abs(table[i, i]) < 1e-9)
                {
                    Console.WriteLine("Матриця вироджена");
                    return null;
                }
                stepCount++;
                Console.WriteLine("Повтор " + stepCount);
                if (JordanStep(table, i, i, left, top))
                {
                    finished[i] = true;
                    TablePrint(table, left, top, "Таблиця:");
                }
            }

            double[,] result = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                int rowIdx = Array.IndexOf(left, "x" + (i + 1));
                for (int j = 0; j < n; j++)
                {
                    int colIdx = Array.IndexOf(top, "y" + (j + 1));
                    result[i, j] = Math.Round(table[rowIdx, colIdx], 4);
                }
            }

            Console.WriteLine("Результат A^-1:");
            for (int i = 0; i < n; i++)
            {
                string line = "  ";
                for (int j = 0; j < n; j++)
                    line += result[i, j].ToString("F4").PadLeft(12);
                Console.WriteLine(line);
            }
            Console.WriteLine();

            return result;
        }

        // ранг (кількість вдалих кроків ЗЖВ)
        public static int FindRank(double[,] source)
        {
            int h = source.GetLength(0);
            int w = source.GetLength(1);
            double[,] table = CopyMatrix(source);

            string[] left = new string[h];
            string[] top = new string[w];
            for (int i = 0; i < h; i++)
                left[i] = "y" + (i + 1);
            for (int j = 0; j < w; j++)
                top[j] = "x" + (j + 1);

            Console.WriteLine("Ранг матриці");
            TablePrint(table, left, top, "Початок:");

            int diagSize = Math.Min(h, w);
            int rankVal = 0;

            for (int i = 0; i < diagSize; i++)
            {
                if (Math.Abs(table[i, i]) < 1e-9)
                {
                    Console.WriteLine("[" + i + "][" + i + "] = 0, пропуск");
                    continue;
                }
                rankVal++;
                Console.WriteLine("Крок " + rankVal);
                JordanStep(table, i, i, left, top);
                TablePrint(table, left, top, "Таблиця:");
            }

            Console.WriteLine("Ранг = " + rankVal);
            Console.WriteLine();
            return rankVal;
        }

        public static double[] MatMult(double[,] matrix, double[] vector)
        {
            int n = matrix.GetLength(0);
            double[] result = new double[n];
            for (int i = 0; i < n; i++)
            {
                double total = 0;
                for (int j = 0; j < vector.Length; j++)
                    total += matrix[i, j] * vector[j];
                result[i] = total;
            }
            return result;
        }

        // СЛАР: A^-1 * B = X
        public static double[] SolveLinear(double[,] A, double[] B)
        {
            double[,] inverted = InverseMatrix(A);
            if (inverted == null)
            {
                Console.WriteLine("Немає розв'язку");
                return null;
            }

            Console.WriteLine("X = A^-1 * B");
            double[] X = MatMult(inverted, B);

            Console.WriteLine("Відповідь:");
            for (int i = 0; i < X.Length; i++)
                Console.WriteLine("  x" + (i + 1) + " = " + Math.Round(X[i], 4));
            Console.WriteLine();

            return X;
        }
    }
}