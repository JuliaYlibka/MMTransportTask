using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MMTransportTask.Pages
{
    /// <summary>
    /// Логика взаимодействия для MinEl_Page.xaml
    /// </summary>
    public partial class MinEl_Page : Page
    {
        public MinEl_Page()
        {
            InitializeComponent();
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
                            Text = "0" // Initialize with zero
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

                TableItems.ItemsSource = new List<Grid> { grid }; // Show the grid in ItemsControl
                MessageBox.Show("Таблица успешно создана!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
                textBox.SelectAll(); // Выбирает весь текст в TextBox
            }
        }

        private void check_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxes.All(tb => !string.IsNullOrWhiteSpace(tb.Text)))
            {
                // Implement your calculation logic here
                var results = CalculateTransportationPlan(); // This is a placeholder for your calculation logic.

                // Display result in the 'result' TextBox
                result.Text = string.Join(Environment.NewLine, results);
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все текстовые поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private List<string> CalculateTransportationPlan()
        {
            // Implement the minimum elements method here.
            // This is a placeholder to represent your logic.
            var output = new List<string>();
            // .... (Calculation logic)
            // Example output, replace with actual results
            output.Add("Опорный план:");
            output.Add("..."); // Fill with actual results
            return output;
        }
    }
}