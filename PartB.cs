using System;

namespace Lab1
{
    class PartB
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

        // крок МЖВ
        public static bool ModifiedJordanStep(double[,] table, int r, int c, string[] left, string[] top)
        {
            int h = table.GetLength(0);
            int w = table.GetLength(1);
            double mainElement = table[r, c];

            if (Math.Abs(mainElement) < 1e-9)
            {
                Console.WriteLine("  Розв. елемент = 0, пропуск");
                return false;
            }

            Console.WriteLine("  Розв. елемент [" + r + "][" + c + "] = " + mainElement.ToString("F4"));

            double[,] saved = CopyMatrix(table);

            table[r, c] = 1.0;

            for (int i = 0; i < h; i++)
            {
                if (i != r)
                    table[i, c] = -saved[i, c];
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

        // пошук опорного розв'язку
        public static bool FindBase(double[,] table, string[] left, string[] top, int m)
        {
            int colB = table.GetLength(1) - 1;

            Console.WriteLine("Пошук опорного розв'язку");
            TablePrint(table, left, top, "Початкова таблиця:");

            for (int step = 1; step <= 50; step++)
            {
                int negRow = -1;
                for (int i = 0; i < m; i++)
                {
                    if (table[i, colB] < -1e-9)
                    {
                        negRow = i;
                        break;
                    }
                }

                if (negRow == -1)
                {
                    Console.WriteLine("Опорний розв'язок знайдено");
                    PrintResult(table, left, top, m);
                    return true;
                }

                Console.WriteLine("Крок " + step);
                Console.WriteLine("  Від'ємний b в рядку " + left[negRow] + " = " + table[negRow, colB].ToString("F4"));

                int selectedCol = -1;
                for (int j = 0; j < colB; j++)
                {
                    if (table[negRow, j] < -1e-9)
                    {
                        selectedCol = j;
                        break;
                    }
                }

                if (selectedCol == -1)
                {
                    Console.WriteLine("  Немає від'ємних в рядку, розв'язків нема");
                    return false;
                }

                Console.WriteLine("  Розв. стовпець: " + top[selectedCol]);

                int selectedRow = -1;
                double best = double.MaxValue;
                for (int i = 0; i < m; i++)
                {
                    if (table[i, selectedCol] > 1e-9)
                    {
                        double val = table[i, colB] / table[i, selectedCol];
                        if (val >= -1e-9 && val < best)
                        {
                            best = val;
                            selectedRow = i;
                        }
                    }
                }

                if (selectedRow == -1)
                {
                    Console.WriteLine("  Немає додатних в стовпці, розв'язків нема");
                    return false;
                }

                Console.WriteLine("  Розв. рядок: " + left[selectedRow] + " (min відн. = " + best.ToString("F4") + ")");

                ModifiedJordanStep(table, selectedRow, selectedCol, left, top);
                TablePrint(table, left, top, "Таблиця після кроку " + step + ":");
            }

            Console.WriteLine("Перевищено ліміт кроків");
            return false;
        }

        // пошук оптимального (max Z)
        public static bool FindMax(double[,] table, string[] left, string[] top, int m)
        {
            int rowZ = table.GetLength(0) - 1;
            int colB = table.GetLength(1) - 1;

            Console.WriteLine("Пошук оптимального розв'язку (max Z)");
            TablePrint(table, left, top, "Початкова таблиця:");

            bool negFound = false;
            for (int i = 0; i < m; i++)
                if (table[i, colB] < -1e-9) negFound = true;

            if (negFound)
            {
                Console.WriteLine("Є від'ємні b, спочатку шукаємо опорний");
                if (!FindBase(table, left, top, m))
                    return false;
            }
            else
            {
                Console.WriteLine("Всі b >= 0, опорний розв'язок є");
            }

            Console.WriteLine();

            for (int step = 1; step <= 50; step++)
            {
                int selectedCol = -1;
                double minZ = 0;
                for (int j = 0; j < colB; j++)
                {
                    if (table[rowZ, j] < minZ - 1e-9)
                    {
                        minZ = table[rowZ, j];
                        selectedCol = j;
                    }
                }

                if (selectedCol == -1)
                {
                    Console.WriteLine("Оптимальний розв'язок знайдено");
                    Console.WriteLine("Z(max) = " + table[rowZ, colB].ToString("F4"));
                    Console.WriteLine();
                    PrintResult(table, left, top, m);
                    return true;
                }

                Console.WriteLine("Крок " + step);
                Console.WriteLine("  Розв. стовпець: " + top[selectedCol] + " (Z = " + table[rowZ, selectedCol].ToString("F4") + ")");

                int selectedRow = -1;
                double bestMin = double.MaxValue;
                for (int i = 0; i < m; i++)
                {
                    if (table[i, selectedCol] > 1e-9)
                    {
                        double val = table[i, colB] / table[i, selectedCol];
                        Console.WriteLine("    " + left[i] + ": " + table[i, colB].ToString("F4") + " / " + table[i, selectedCol].ToString("F4") + " = " + val.ToString("F4"));
                        if (val < bestMin)
                        {
                            bestMin = val;
                            selectedRow = i;
                        }
                    }
                }

                if (selectedRow == -1)
                {
                    Console.WriteLine("  Z необмежена зверху");
                    return false;
                }

                Console.WriteLine("  Розв. рядок: " + left[selectedRow] + " (min відн. = " + bestMin.ToString("F4") + ")");

                ModifiedJordanStep(table, selectedRow, selectedCol, left, top);
                TablePrint(table, left, top, "Таблиця після кроку " + step + ":");
            }

            Console.WriteLine("Перевищено ліміт кроків");
            return false;
        }

        public static void PrintResult(double[,] table, string[] left, string[] top, int m)
        {
            int colB = table.GetLength(1) - 1;
            Console.WriteLine("Відповідь:");
            for (int i = 0; i < m; i++)
                Console.WriteLine("  " + left[i] + " = " + table[i, colB].ToString("F4"));
            for (int j = 0; j < top.Length; j++)
                if (top[j] != "b")
                    Console.WriteLine("  " + top[j] + " = 0");
            Console.WriteLine();
        }
    }
}