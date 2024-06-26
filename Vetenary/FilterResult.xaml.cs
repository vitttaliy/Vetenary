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
using System.Windows.Shapes;
using Vetenary.Classes;

namespace Vetenary
{
    /// <summary>
    /// Interaction logic for FilterResult.xaml
    /// </summary>
    public partial class FilterResult : Window
    {
        public FilterResult()
        {
            InitializeComponent();
        }

        public void AddFilterPatient(Animal animal)
        {
            // Create a container for the record
            StackPanel recordPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(5)
            };

            // Add image
            Image image = new Image
            {
                Width = 100,
                Height = 100,
                Source = FileSystem.LoadImage(animal.ImageSource),
                Margin = new Thickness(5)
            };
            recordPanel.Children.Add(image);
            recordPanel.Tag = animal;

            // Add text block for name
            TextBlock nameBlock = new TextBlock
            {
                Text = $"Ім'я: {animal.Name}",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };
            recordPanel.Children.Add(nameBlock);

            // Add text block for age
            TextBlock ageBlock = new TextBlock
            {
                Text = $"Вік: {animal.Age}",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };
            recordPanel.Children.Add(ageBlock);

            // Add text block for breed
            TextBlock speciesBlock = new TextBlock
            {
                Text = $"Тварина: {animal.Type}",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };
            recordPanel.Children.Add(speciesBlock);

            // Add the record panel to the StackPanel
            RecordStackPanel.Children.Add(recordPanel);
        }

        public void ResetView()
        {
            RecordStackPanel.Children.Clear();
            RecordStackPanel.InvalidateVisual();
        }
    }
}
