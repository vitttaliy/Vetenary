using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using Vetenary.Enums;

namespace Vetenary
{
    /// <summary>
    /// Interaction logic for NewPatient.xaml
    /// </summary>
    public partial class NewPatient : Window
    {
        string img = "";
        public NewPatient()
        {
            InitializeComponent();
            Species.ItemsSource = Enum.GetValues(typeof(AnimalTypes));
            Gender.ItemsSource = Enum.GetValues(typeof(Gender));
            FontSize = 20;
        }
        private Bitmap ImageSourceToBitmap(ImageSource imageSource)
        {
            Bitmap bitmap = null;
            using (MemoryStream stream = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder(); // Or any other encoder
                encoder.Frames.Add(BitmapFrame.Create(imageSource as BitmapSource));
                encoder.Save(stream);

                bitmap = new Bitmap(stream);
            }
            return bitmap;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //AddRecord(new Cat { Color = "Сірий", ImageUri = "https://via.placeholder.com/100", Name = "Бадді" });

            MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            Animal animal = null;

            if (Species.SelectedItem.ToString() == AnimalTypes.Cat.ToString())
            {
                animal = new Cat();
            }
            else if (Species.SelectedItem.ToString() == AnimalTypes.Dog.ToString())
            {
                animal = new Dog();
            }
            else if (Species.SelectedItem.ToString() == AnimalTypes.Rabbit.ToString())
            {
                animal = new Rabbit();

            }
            else if (Species.SelectedItem.ToString() == AnimalTypes.Parrot.ToString())
            {
                animal = new Parrot();
                
            }

            MemoryStream ms = new MemoryStream();

            // Create a BitmapEncoder and save the image to the MemoryStream
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)Photo.Source));
            encoder.Save(ms);


            animal.ImageSource = ms.ToArray();
            animal.Name = Name.Text;
            animal.Age = int.Parse(Age.Text);
            animal.Owner = Owner.Text;
            animal.Gender = (Gender)Gender.SelectedItem;
            animal.ProceduresAndAttentionDates = ProceduresAndMedicine.Text;
            animal.MedicineAndMedHistory = MedHistory.Text;
            animal.Type = (AnimalTypes)Species.SelectedItem;

            if (Species.SelectedItem.ToString() == AnimalTypes.Cat.ToString())
            {
                animal.Breed = (CatBreed)Breed.SelectedItem;
            }
            else if (Species.SelectedItem.ToString() == AnimalTypes.Dog.ToString())
            {
                animal.Breed = (DogBreed)Breed.SelectedItem;
            }
            else if (Species.SelectedItem.ToString() == AnimalTypes.Rabbit.ToString())
            {
                animal.Breed = (RabbitBreed)Breed.SelectedItem;


            }
            else if (Species.SelectedItem.ToString() == AnimalTypes.Parrot.ToString())
            {
                animal.Breed = (ParrotBreed)Breed.SelectedItem;

            }


            //animal.ImageUri = Photo.;
            mainWindow.AddRecord(animal);
            //DialogResult = true;
            this.Close();
        }

        private void AnimalsTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void Photo_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = openFileDialog.FileName;

                // Display selected image
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedImagePath);
                bitmap.EndInit();
                Photo.Source = bitmap;
            }

        }
    }
}
