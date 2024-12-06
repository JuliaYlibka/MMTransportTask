using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MMTransportTask.Pages
{
    public partial class SZYPage : Page
    {
        public SZYPage()
        {
            InitializeComponent();
            result.Visibility = Visibility.Hidden;

        }

        private List<TextBox> textBoxes = new List<TextBox>();
        private TextBox[,] textBoxArray; 

        private void CreateTableButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(RowsInput.Text, out int rows) && int.TryParse(ColumnsInput.Text, out int columns))
            {
                if (rows <= 0 || columns <= 0)
                {
                    MessageBox.Show("Количество строк и столбцов должно быть больше нуля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                TableItems.Items.Clear();
                textBoxes.Clear();
                var grid = new Grid();
                textBoxArray = new TextBox[rows, columns]; 

                for (int i = 0; i < rows; i++)
                {
                    grid.RowDefinitions.Add(new RowDefinition());
                }
                for (int j = 0; j < columns; j++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                }

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        var textBox = new TextBox
                        {
                            Name = $"TB_{i}{j}",
                            Margin = new Thickness(5),
                            Text = "0"                            
                        };
                        textBox.GotFocus += TextBox_GotFocus;
                        textBox.PreviewTextInput += TextBox_PreviewTextInput;
                        Grid.SetRow(textBox, i);
                        Grid.SetColumn(textBox, j);
                        grid.Children.Add(textBox);
                        textBoxes.Add(textBox);
                        textBoxArray[i, j] = textBox; 

                    }
                }
                textBoxArray[rows-1,columns-1 ].Visibility = Visibility.Hidden;
                TableItems.Items.Add(grid);
                MessageBox.Show("Таблица успешно создана! Введите значения ресурсов, поставщиков и потребителей.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректные числовые значения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNaturalNumber(e.Text);
        }

        private bool IsNaturalNumber(string text)
        {
            return int.TryParse(text, out int number) && number >= 0;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll(); 
            }
        }

        private void check_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxes.All(tb => !string.IsNullOrWhiteSpace(tb.Text)))
            {
                int colCount = int.Parse(ColumnsInput.Text);
                int rowCount = int.Parse(RowsInput.Text);

                // Считаем сумму последней строки
                int sumLastRow = 0;
                for (int j = 0; j < colCount; j++)
                {
                    sumLastRow += int.Parse(textBoxArray[rowCount - 1, j].Text);
                }

                int sumLastColumn = 0;
                for (int i = 0; i < rowCount; i++)
                {
                    sumLastColumn += int.Parse(textBoxArray[i, colCount - 1].Text);
                }

                if (sumLastRow != sumLastColumn)
                {
                    if (sumLastRow > sumLastColumn)
                    {
                        MessageBox.Show("Задача не сбалансирована. необходимо добавить столбец перед последним, заполняя его нулями, в итоговом поле указать " + (sumLastRow - sumLastColumn) , "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Задача не сбалансирована. необходимо добавить строку перед последней, заполняя ее нулями, в итоговом поле указать " + (sumLastColumn - sumLastRow), "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Суммы последней строки и последнего столбца равны.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    SZY();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все текстовые поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddNewRowBeforeLast(int rowCount, int difference)
        {
            var grid = (Grid)TableItems.Items[0];

            // Добавляем новую строку на нужное место
            grid.RowDefinitions.Insert(rowCount, new RowDefinition());

            for (int j = 0; j < grid.ColumnDefinitions.Count; j++)
            {
                var textBox = new TextBox
                {
                    Name = $"TB_{rowCount}{j}", // Присваиваем имя текстовому полю
                    Margin = new Thickness(5),
                    Text ="0",//(j == grid.ColumnDefinitions.Count - 1) ? difference.ToString() : "0",
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                textBox.GotFocus += TextBox_GotFocus;
                textBox.PreviewTextInput += TextBox_PreviewTextInput;

                Grid.SetRow(textBox, rowCount);
                Grid.SetColumn(textBox, j);
                RowsInput.Text = rowCount.ToString();

                // Добавляем новый TextBox в сетку
                grid.Children.Add(textBox);
                textBoxes.Add(textBox); // Сохраняем ссылку на новый TextBox
            }

            // Обновляем отображение таблицы
            TableItems.Items.Clear();
            TableItems.Items.Add(grid);

            MessageBox.Show("Новая строка успешно добавлена, но задача не сбалансирована", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddNewColumnBeforeLast(int colCount, int difference)
        {
            var grid = (Grid)TableItems.Items[0];

            var newArray = new TextBox[grid.RowDefinitions.Count, colCount + 1];

            // Копируем старые значения
            for (int i = 0; i < grid.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    newArray[i, j] = textBoxArray[i, j]; // Копируем старый массив
                }
            }

            // Добавляем новый столбец перед последним
            for (int i = 0; i < grid.RowDefinitions.Count; i++)
            {
                var textBox = new TextBox
                {
                    Name = $"TB_0{i}{colCount}", // Присваиваем имя новому столбцу
                    Margin = new Thickness(5),
                    Text = (i == grid.RowDefinitions.Count - 1) ? difference.ToString() : "0",
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                textBox.GotFocus += TextBox_GotFocus;
                textBox.PreviewTextInput += TextBox_PreviewTextInput;

                Grid.SetRow(textBox, i);
                Grid.SetColumn(textBox, colCount - 1); // Новый столбец на позиции colCount - 1
                grid.Children.Add(textBox);
                newArray[i, colCount] = textBox; // Сохраняем ссылку на новый TextBox
            }

            // Копируем старый последний столбец в новый
            for (int i = 0; i < grid.RowDefinitions.Count; i++)
            {
                newArray[i, colCount - 1] = textBoxArray[i, colCount - 1];
            }

            // Обновляем массив
            textBoxArray = newArray;

            TableItems.Items.Clear();
            TableItems.Items.Add(grid);
            ColumnsInput.Text = colCount.ToString();

            MessageBox.Show("Новый столбец успешно добавлен, но задача не сбалансирована", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

        }
        private void SZY()
        {
            int x = int.Parse(ColumnsInput.Text); // Количество столбцов (спрос)
            int y = int.Parse(RowsInput.Text);    // Количество строк (предложение)

            // Создаем массивы для хранения издержек, предложения и спроса
            int[,] costs = new int[y, x];
            int[] supply = new int[y];
            int[] demand = new int[x];

            // Инициализация массивов
            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    costs[i, j] = 0;  
                }

                supply[i] = int.Parse(textBoxArray[i, x - 1].Text);
            }

            for (int i = 0; i < x; i++)
            {
                demand[i] = int.Parse(textBoxArray[y - 1, i].Text);
            }

            int curX = 0; // Индекс текущего столбца
            int curY = 0; // Индекс текущей строки

            double totalCost = 0;

            while (curX < x - 1 && curY < y - 1) 
            {
                int allocated = Math.Min(supply[curY], demand[curX]);
                costs[curY, curX] = allocated;
                supply[curY] -= allocated;
                demand[curX] -= allocated;     

                // Рассчитываем стоимость
                double cost = int.Parse(textBoxArray[curY, curX].Text);
                totalCost += allocated * cost;

                if (supply[curY] == 0) curY++; 
                if (demand[curX] == 0) curX++; 
            }


            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    if (i == y - 1 || j == x - 1)
                    {
                    }
                    else
                    {
                        textBoxArray[i, j].Text = costs[i, j].ToString(); 
                    }
                }
            }
            result.Text = $"Общая стоимость опорного плана: {totalCost}";
            result.Visibility = Visibility.Visible;

            MessageBox.Show($"Общая стоимость опорного плана: {totalCost}", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
            
        }
        private void MinEl()
        {
            int y = int.Parse(RowsInput.Text);
            int x = int.Parse(ColumnsInput.Text);

            int[,] costs = new int[y, x];
            int[] supply = new int[y];
            int[] demand = new int[x];

            // Fill costs array
            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    costs[i, j] = int.Parse(textBoxArray[i, j].Text);
                }
            }

            // Last row and column values
            for (int i = 0; i < x; i++)
                demand[i] = int.Parse(textBoxArray[y - 1, i].Text);

            for (int j = 0; j < y; j++)
                supply[j] = int.Parse(textBoxArray[j, x - 1].Text);

            int[,] plan = new int[y, x];
            int[] remainingSupply = new int[y];
            int[] remainingDemand = new int[x];

            Array.Copy(supply, remainingSupply, y); // Copy initial supplies
            Array.Copy(demand, remainingDemand, x); // Copy initial demands

            double totalCost = 0.0; // Initialize total cost

            while (!IsPlanComplete(remainingSupply, remainingDemand))
            {
                // Finding the minimum cost index
                int minCost = Int32.MaxValue;
                int minI = -1;
                int minJ = -1;

                for (int i = 0; i < y; i++)
                {
                    if (remainingSupply[i] == 0) continue; // Skip exhausted suppliers

                    for (int j = 0; j < x; j++)
                    {
                        if (remainingDemand[j] == 0) continue; // Skip satisfied consumers

                        if (costs[i, j] != -1 && costs[i, j] < minCost)
                        {
                            minCost = costs[i, j];
                            minI = i;
                            minJ = j;
                        }
                    }
                }

                // If no valid minimum was found
                if (minCost == Int32.MaxValue)
                    break;

                // Allocate the supply to the demand
                int quantity = Math.Min(remainingSupply[minI], remainingDemand[minJ]);
                plan[minI, minJ] = quantity;
                totalCost += quantity * minCost; // Add to total cost
                remainingSupply[minI] -= quantity;
                remainingDemand[minJ] -= quantity;
                costs[minI, minJ] = -1; // Marking this cell as processed
            }

            // Update the text boxes with the plan
            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    if (i == y - 1 || j == x - 1)
                    {
                        // Keep last row and last column unchanged
                    }
                    else
                    {
                        textBoxArray[i, j].Text = plan[i, j].ToString(); // Update with allocations
                    }
                }
            }

            result.Text = $"Общая стоимость опорного плана: {totalCost:#0.00}"; // Update total cost display
            result.Visibility = Visibility.Visible;

            MessageBox.Show("Решение получено методом минимальных элементов.", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);

        }
        private bool IsPlanComplete(int[] remainingSupply, int[] remainingDemand)
        {
            return remainingSupply.All(s => s == 0) && remainingDemand.All(d => d == 0);
        }

        private void checkMinEl_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxes.All(tb => !string.IsNullOrWhiteSpace(tb.Text)))
            {
                int colCount = int.Parse(ColumnsInput.Text);
                int rowCount = int.Parse(RowsInput.Text);

                // Считаем сумму последней строки
                int sumLastRow = 0;
                for (int j = 0; j < colCount; j++)
                {
                    sumLastRow += int.Parse(textBoxArray[rowCount - 1, j].Text);
                }

                int sumLastColumn = 0;
                for (int i = 0; i < rowCount; i++)
                {
                    sumLastColumn += int.Parse(textBoxArray[i, colCount - 1].Text);
                }

                if (sumLastRow != sumLastColumn)
                {
                    if (sumLastRow > sumLastColumn)
                    {
                        MessageBox.Show("Задача не сбалансирована. необходимо добавить столбец перед последним, заполняя его нулями, в итоговом поле указать " + (sumLastRow - sumLastColumn), "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Задача не сбалансирована. необходимо добавить строку перед последней, заполняя ее нулями, в итоговом поле указать " + (sumLastColumn - sumLastRow), "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Суммы последней строки и последнего столбца равны.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    MinEl();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все текстовые поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
