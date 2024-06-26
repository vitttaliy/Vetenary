using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Vetenary.Classes;
using Vetenary.Enums;
using Image = System.Windows.Controls.Image;

namespace Vetenary
{
    public partial class MainWindow : Window//Встановлює розмір шрифту.
                                            //Налаштовує представлення колекції тварин і фільтр для неї.
                                            //Завантажує дані з файлу при запуску програми.
    {
        UIElement current_editing = null;
        HashSet<Animal> animals = new HashSet<Animal>();
        ICollectionView animalsView;
        Array animal_types = Enum.GetValues(typeof(AnimalTypes));
        Animal prev_record = null;

        public MainWindow()
        {
            InitializeComponent();
            Species.ItemsSource = animal_types;
            FontSize = 20;
            animalsView = CollectionViewSource.GetDefaultView(animals);
            animalsView.Filter = FilterAnimals;

            try
            {
                string executableDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = System.IO.Path.Combine(executableDirectory, "animals.dat");
                ObservableCollection<Animal> deserializedProduct = JsonConvert.DeserializeObject<ObservableCollection<Animal>>(File.ReadAllText(filePath));
                animals = FileSystem.DeserializeFromFile(filePath);
                UpdatePanel();
            }
            catch (Exception ex)
            {
                // Handle or log the exception
            }
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        private void UpdatePanel()//Очищує панель записів RecordStackPanel і заново додає до неї всі записи з колекції тварин

        {
            RecordStackPanel.Children.Clear();
            foreach (var animal in animals)
            {
                AddRecord(animal);
            }
        }

        public ImageSource ImageSourceFromBitmap(Bitmap bmp)//Перетворює об'єкт Bitmap у ImageSource для використання в інтерфейсі WPF.
        {
            nint handle = 0;
            if (bmp != null)
            {
                handle = bmp.GetHbitmap();
            }
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) { }

        public void AddRecord(Animal animal)//Додає нову запис про тварину до колекції та інтерфейсу.
        {
            animals.Add(animal);
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

            // Add edit button
            Button editButton = new Button
            {
                Content = "Переглянути",
                Margin = new Thickness(10, 0, 0, 0),
                Width = 100,
                Height = 25
            };
            editButton.Click += (sender, e) => EditRecord(ref animal, recordPanel);
            recordPanel.Children.Add(editButton);

            // Add delete button
            Button deleteButton = new Button
            {
                Content = "Видалити",
                Margin = new Thickness(10, 0, 0, 0),
                Width = 100,
                Height = 25
            };
            deleteButton.Click += (sender, e) => DeleteRecord(recordPanel);
            recordPanel.Children.Add(deleteButton);

            // Add the record panel to the StackPanel
            RecordStackPanel.Children.Add(recordPanel);
        }

        private void EditRecord(ref Animal animal, UIElement recordPanel)//Видаляє тварину з колекції, відкриває вікно для редагування запису, оновлює інтерфейс після редагування
        {
            animals.Remove(animal);
            PatientInfo secondaryWindow = new PatientInfo(ref animal);
            current_editing = recordPanel;
            secondaryWindow.WindowClosedWithClass += SecondaryWindow_WindowClosedWithClass;
            secondaryWindow.Show();
        }

        private void SecondaryWindow_WindowClosedWithClass(object sender, ClassEventArgs e)//Оновлює запис про тварину після закриття вікна редагування, оновлює інтерфейс.
        {
            Animal returnedData = e.ReturnedClass;
            DeleteRecord(current_editing);
            AddRecord(returnedData);
            animalsView.Refresh();
            RecordStackPanel.InvalidateVisual();
        }

        private void DeleteRecord(UIElement recordPanel)//Видаляє запис про тварину з колекції та інтерфейсу.
        {
            StackPanel stackPanel = recordPanel as StackPanel;
            if (stackPanel != null)
            {
                Animal animalToRemove = stackPanel.Tag as Animal;
                if (animalToRemove != null)
                {
                    animals.Remove(animalToRemove);
                }
                RecordStackPanel.Children.Remove(recordPanel);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)//Обробляє натискання кнопки для входу в систему, перевіряє логін і пароль.
        {
            if (Login.Text == "admin" && Password.Password == "admin1")
            {
                LoginPage.Visibility = Visibility.Collapsed;
                RecordGrid.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Невірний логін/пароль!");
            }
        }

        private void AddNewRecord_Click(object sender, RoutedEventArgs e)//Відкриває вікно для додавання нової тварини
        {
            new NewPatient().ShowDialog();
        }

        private void Species_SelectionChanged(object sender, SelectionChangedEventArgs e)//Змінює список порід у ComboBox Breed в залежності від вибраного виду тварини.
        {
            Breed.ItemsSource = null;
            if (Species.SelectedItem.ToString() == AnimalTypes.Cat.ToString())
            {
                Breed.ItemsSource = Enum.GetValues(typeof(CatBreed));
            }
            else if (Species.SelectedItem.ToString() == AnimalTypes.Dog.ToString())
            {
                Breed.ItemsSource = Enum.GetValues(typeof(DogBreed));
            }
            else if (Species.SelectedItem.ToString() == AnimalTypes.Rabbit.ToString())
            {
                Breed.ItemsSource = Enum.GetValues(typeof(RabbitBreed));
            }
            else if (Species.SelectedItem.ToString() == AnimalTypes.Parrot.ToString())
            {
                Breed.ItemsSource = Enum.GetValues(typeof(ParrotBreed));
            }
        }

        private void Window_Closed(object sender, EventArgs e)//Зберігає дані про тварин у файл при закритті програми
        {
            string executableDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = System.IO.Path.Combine(executableDirectory, "animals.dat");
            FileSystem.SerializeToFile(animals, filePath);
        }

        private void FilterChanged(object sender, RoutedEventArgs e)//Оновлює фільтр колекції тварин при зміні критеріїв фільтрації
        {
            animalsView.Refresh();
        }

        private bool FilterAnimals(object obj)//Застосовує фільтр до колекції тварин на основі введених користувачем критеріїв.
        {
            if (obj is Animal animal)
            {
                bool nameMatches = !string.IsNullOrEmpty(Name.Text) && animal.Name.Contains(Name.Text, StringComparison.OrdinalIgnoreCase);
                bool ageMatches = !string.IsNullOrEmpty(Age.Text) && animal.Age.ToString().Contains(Age.Text);
                bool speciesMatches = Species.SelectedItem != null && animal.Type.ToString() == Species.SelectedItem.ToString();
                bool breedMatches =animal.Breed!=null && Breed.SelectedItem != null && animal.Breed.ToString() == Breed.SelectedItem.ToString();

                return (nameMatches && ageMatches) ||
                       (nameMatches && speciesMatches) ||
                       (nameMatches && breedMatches) ||
                       (ageMatches && speciesMatches) ||
                       (ageMatches && breedMatches) ||
                       (speciesMatches && breedMatches) ||
                       (nameMatches && ageMatches && speciesMatches) ||
                       (nameMatches && ageMatches && breedMatches) ||
                       (nameMatches && speciesMatches && breedMatches) ||
                       (ageMatches && speciesMatches && breedMatches) ||
                       (nameMatches && ageMatches && speciesMatches && breedMatches);
            }

            return false;


        }

        private void FiltrButton(object sender, RoutedEventArgs e)//Відкриває або фокусується на вікні результатів фільтрації, додає відфільтрованих тварин до цього вікна
        {
            FilterResult fr = Application.Current.Windows.OfType<FilterResult>().FirstOrDefault();


            if(fr != null)
            {
                fr.ResetView();
            }
            else
            {
                fr = new FilterResult();
                fr.Show();

            }

            foreach (var an in animals)
            {
                if (FilterAnimals(an))
                {
                    fr.AddFilterPatient(an);
                }
            }
           
            fr.Focus();

        }

        private void Species_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
