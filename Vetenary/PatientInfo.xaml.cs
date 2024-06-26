using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Vetenary.Classes;
using Vetenary.Enums;

namespace Vetenary
{
    public class ClassEventArgs : EventArgs//Ініціалізує новий екземпляр ClassEventArgs з переданим об'єктом Animal.
    {
        public Animal ReturnedClass { get; private set; }

        public ClassEventArgs(Animal returnedClass)
        {
            ReturnedClass = returnedClass;
        }
    }
    /// <summary>
    /// Interaction logic for PatientInfo.xaml
    /// </summary>
    public partial class PatientInfo : Window//Ініціалізує компоненти інтерфейсу.

    {
        public event EventHandler<ClassEventArgs> WindowClosedWithClass;

        public PatientInfo()
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)//Викликається при закритті вікна, піднімає подію з передачею даних про тварину
        {
            base.OnClosed(e);

            // Raise event with the data to return
            WindowClosedWithClass?.Invoke(this, new ClassEventArgs(pet));
        }
        Animal pet=null;
        Animal prev_pet = null;
        public PatientInfo(ref Animal patient)//Ініціалізує компоненти інтерфейсу, завантажує дані про тварину в поля форми.
        {
            InitializeComponent();
            pet = patient;
            prev_pet = pet;
            Photo.Source = FileSystem.LoadImage(patient.ImageSource);
            Name.Text = patient.Name;
            Owner.Text = patient.Owner;
            ProceduresAndMedicine.Text = patient.ProceduresAndAttentionDates;
            MedHistory.Text = patient.MedicineAndMedHistory;
            Age.Text = patient.Age.ToString();
            Species.ItemsSource = Enum.GetValues(typeof(AnimalTypes));
            Gender.ItemsSource = Enum.GetValues(typeof(Gender));
            Species.SelectedItem = patient.Type;
            Gender.SelectedItem = patient.Gender;
            //Breed.SelectedItem = patient.Breed;


            if (patient.Type == AnimalTypes.Cat)
            {
                Breed.SelectedItem = (CatBreed)patient.Breed;
            }
            else if (patient.Type == AnimalTypes.Dog)
            {
                Breed.SelectedItem = (DogBreed)patient.Breed;
            }
            else if (patient.Type == AnimalTypes.Rabbit)
            {
                Breed.SelectedItem = (RabbitBreed)patient.Breed;
            }
            else if (patient.Type == AnimalTypes.Parrot)
            {
                Breed.SelectedItem = (ParrotBreed)patient.Breed;
            }
            FontSize = 20;


        }
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public ImageSource ImageSourceFromBitmap(Bitmap bmp)//Перетворює об'єкт Bitmap у ImageSource для використання в інтерфейсі WPF.

        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }
        private void Button_Click(object sender, RoutedEventArgs e)//Закриває вікно при натисканні кнопки.
        {
            this.Close();
        }
        private Bitmap ImageSourceToBitmap(ImageSource imageSource)//Перетворює ImageSource у Bitmap.
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
        private void Button_Click_1(object sender, RoutedEventArgs e)//Зберігає внесені зміни про тварину, оновлює об'єкт Animal та закриває вікно
        {
            //AddRecord(new Cat { Color = "Сірий", ImageUri = "https://via.placeholder.com/100", Name = "Бадді" });

            MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();


            if (Species.SelectedItem.ToString() == AnimalTypes.Cat.ToString())
            {
                pet = new Cat();
            }
            else if (Species.SelectedItem.ToString() == AnimalTypes.Dog.ToString())
            {
                pet = new Dog();
            }
            else if (Species.SelectedItem.ToString() == AnimalTypes.Rabbit.ToString())
            {
                pet = new Rabbit();

            }
            else if (Species.SelectedItem.ToString() == AnimalTypes.Parrot.ToString())
            {
                pet = new Parrot();
            }


            MemoryStream ms = new MemoryStream();

            // Create a BitmapEncoder and save the image to the MemoryStream
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)Photo.Source));
            encoder.Save(ms);


            pet.ImageSource = ms.ToArray();
            pet.Name = Name.Text;
            pet.Age = int.Parse(Age.Text);
            pet.Owner = Owner.Text;
            pet.Gender = (Gender)Gender.SelectedItem;
            pet.ProceduresAndAttentionDates = ProceduresAndMedicine.Text;
            pet.MedicineAndMedHistory = MedHistory.Text;
            pet.Type = (AnimalTypes)Species.SelectedItem;
            if (Species.SelectedItem.ToString() == AnimalTypes.Cat.ToString())
            {
                pet.Breed = (CatBreed)Breed.SelectedItem;
            }
            else if (Species.SelectedItem.ToString() == AnimalTypes.Dog.ToString())
            {
                pet.Breed = (DogBreed)Breed.SelectedItem;
            }
            else if (Species.SelectedItem.ToString() == AnimalTypes.Rabbit.ToString())
            {
                pet.Breed = (RabbitBreed)Breed.SelectedItem;


            }
            else if (Species.SelectedItem.ToString() == AnimalTypes.Parrot.ToString())
            {
                pet.Breed = (ParrotBreed)Breed.SelectedItem;
            }            //animal.ImageUri = Photo.;

            //mainWindow.AddRecord(pet);
            this.Close();
        }
        private void AnimalsTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)//Оновлює список порід у ComboBox Breed в залежності від вибраного виду тварини.
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

        private void Photo_PreviewMouseDown(object sender, MouseButtonEventArgs e)//Відкриває діалогове вікно для вибору зображення, завантажує обране зображення і відображає його.
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Зображення (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|Усі файли (*.*)|*.*";
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
