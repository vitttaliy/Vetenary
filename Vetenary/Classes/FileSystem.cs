using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Vetenary.Classes
{
    public static class FileSystem
    {
        public static void SerializeToFile(HashSet<Animal> list, string filePath)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto // Include type information
            };

            string jsonString = JsonConvert.SerializeObject(list, settings);

            File.WriteAllText(filePath, jsonString);
        }
        public static HashSet<Animal> DeserializeFromFile(string filePath)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto // Include type information
            };

            string jsonString = File.ReadAllText(filePath);

            return JsonConvert.DeserializeObject<HashSet<Animal>>(jsonString, settings);
        }
        public static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
        public static List<Animal> DeserializeListFromFile(string filePath)
        {
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found.", filePath);
            }

            // Deserialize the list from the XML file
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Animal>));
                return (List<Animal>)serializer.Deserialize(fileStream);
            }
        }

        static void UpdateRecord()
        {

        }
    }
}
