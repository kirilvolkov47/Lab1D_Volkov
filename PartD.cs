using System;

namespace Lab1
{
    class PartD
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
                {
                    double val = table[i, j];
                    if (Math.Abs(val) < 1e-9)
                        val = 0;
                    line += val.ToString("F4").PadLeft(12);
                }
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
            {
                for (int j = 0; j < w; j++)
                {
                    double val = table[i, j] / mainElement;
                    if (Math.Abs(val) < 1e-9)
                        val = 0;
                    table[i, j] = val;
                }
            }

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

        // дробова частина числа
        static double DecimalPart(double x)
        {
            double fracPart = x - Math.Floor(x);
            if (fracPart < 1e-9 || fracPart > 1 - 1e-9)
                return 0;
            return fracPart;
        }

        // додавання нового обмеження
        public static void AddNewRow(ref double[,] table, ref string[] left, string[] top, int m, int gomoryRow)
        {
            int rowsOld = table.GetLength(0);
            int cols = table.GetLength(1);
            int colB = cols - 1;

            double[,] newTable = new double[rowsOld + 1, cols];
            string[] newLeft = new string[left.Length + 1];

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < cols; j++)
                    newTable[i, j] = table[i, j];
                newLeft[i] = left[i];
            }

            for (int j = 0; j < colB; j++)
            {
                double val = -DecimalPart(table[gomoryRow, j]);
                if (Math.Abs(val) < 1e-9)
                    val = 0;
                newTable[m, j] = val;
            }

            double bVal = -DecimalPart(table[gomoryRow, colB]);
            if (Math.Abs(bVal) < 1e-9)
                bVal = 0;
            newTable[m, colB] = bVal;
           int maxX = 0;
            for (int i = 0; i < left.Length; i++)
            {
                if (left[i].StartsWith("x"))
                {
                    int num;
                    if (int.TryParse(left[i].Substring(1), out num) && num > maxX)
                        maxX = num;
                }
            }

            for (int j = 0; j < top.Length; j++)
            {
                if (top[j].StartsWith("x"))
                {
                    int num;
                    if (int.TryParse(top[j].Substring(1), out num) && num > maxX)
                        maxX = num;
                }
            }

            string newName = "x" + (maxX + 1);
            newLeft[m] = newName;

            for (int j = 0; j < cols; j++)
                newTable[m + 1, j] = table[m, j];
            newLeft[m + 1] = "Z";

            table = newTable;
            left = newLeft;
        }

        // метод Гоморі
        public static bool GomoryMethod(double[,] table, string[] left, string[] top, int m, int n)
        {
            Console.WriteLine("Метод Гоморі");
            Console.WriteLine();

            if (!FindBase(table, left, top, m))
                return false;

            if (!FindMax(table, left, top, m))
                return false;

            for (int step = 1; step <= 20; step++)
            {
                // чи є дробові
                bool allInt = true;
                int colB = table.GetLength(1) - 1;

                for (int k = 1; k <= n; k++)
                {
                    string name = "x" + k;
                    double val = 0;

                    for (int i = 0; i < m; i++)
                    {
                        if (left[i] == name)
                        {
                            val = table[i, colB];
                            break;
                        }
                    }

                    if (Math.Abs(val - Math.Round(val)) > 1e-9)
                    {
                        allInt = false;
                        break;
                    }
                }

                if (allInt)
                {
                    int rowZ = table.GetLength(0) - 1;
                    int colB2 = table.GetLength(1) - 1;

                    Console.WriteLine("Цілий розв'язок знайдено");
                    Console.WriteLine("Z(max) = " + table[rowZ, colB2].ToString("F4"));
                    Console.WriteLine();

                    Console.WriteLine("Відповідь:");
                    for (int k = 1; k <= n; k++)
                    {
                        string name = "x" + k;
                        double val = 0;

                        for (int i = 0; i < m; i++)
                        {
                            if (left[i] == name)
                            {
                                val = table[i, colB2];
                                break;
                            }
                        }

                        if (Math.Abs(val) < 1e-9)
                            val = 0;

                        if (Math.Abs(val - Math.Round(val)) < 1e-9)
                            val = Math.Round(val);

                        Console.WriteLine("  " + name + " = " + val.ToString("F4"));
                    }
                    Console.WriteLine();

                    return true;
                }

                // шукаємо рядок для нового обмеження
                int gomoryRow = -1;
                double best = 0;
                for (int i = 0; i < m; i++)
                {
                    double fracPart = DecimalPart(table[i, colB]);
                    if (fracPart > best + 1e-9)
                    {
                        best = fracPart;
                        gomoryRow = i;
                    }
                }

                if (gomoryRow == -1 || best < 1e-9)
                {
                    Console.WriteLine("Неможливо побудувати нове обмеження");
                    return false;
                }

                Console.WriteLine("Крок Гоморі " + step);
                Console.WriteLine("  Рядок для нового обмеження: " + left[gomoryRow] + " = " + table[gomoryRow, colB].ToString("F4"));
                Console.WriteLine("  Дробова частина = " + best.ToString("F4"));
                Console.WriteLine("  Нове обмеження Гоморі:");
                for (int j = 0; j < colB; j++)
                {
                    double val = -DecimalPart(table[gomoryRow, j]);
                    if (Math.Abs(val) < 1e-9)
                        val = 0;
                    Console.WriteLine("    " + top[j] + ": " + val.ToString("F4"));
                }

                double lastVal = -best;
                if (Math.Abs(lastVal) < 1e-9)
                    lastVal = 0;
                Console.WriteLine("    b: " + lastVal.ToString("F4"));
                Console.WriteLine();

                AddNewRow(ref table, ref left, top, m, gomoryRow);
                m++;

                TablePrint(table, left, top, "Таблиця після додавання обмеження:");

                if (!FindBase(table, left, top, m))
                    return false;

                if (!FindMax(table, left, top, m))
                    return false;
            }

            Console.WriteLine("Перевищено ліміт кроків Гоморі");
            return false;
        }

        public static void PrintResult(double[,] table, string[] left, string[] top, int m)
        {
            int colB = table.GetLength(1) - 1;
            Console.WriteLine("Відповідь:");
            for (int i = 0; i < m; i++)
            {
                double val = table[i, colB];
                if (Math.Abs(val) < 1e-9)
                    val = 0;
                Console.WriteLine("  " + left[i] + " = " + val.ToString("F4"));
            }

            for (int j = 0; j < top.Length; j++)
                if (top[j] != "b")
                    Console.WriteLine("  " + top[j] + " = 0");
            Console.WriteLine();
        }
    }
}