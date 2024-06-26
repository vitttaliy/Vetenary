using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Vetenary.Enums;

namespace Vetenary.Classes
{
    public class Rabbit: Animal
    {
        string _name = "";
        dynamic _breed = RabbitBreed.Unknown;
        List<string> _attention = new List<string>();
        int _age = 0;
        Gender gender;
        string _owner = "";
        string _procedures = "";
        string _medicine = "";
        string _color = "";
        public override string Name { get => _name; set => _name = value; }
        public override List<string> AttentionDates { get => _attention; }
        public override int Age { get => _age; set => _age = value; }
        public override Gender Gender { get => gender; set => gender = value; }
        public override string Owner { get => _owner; set => _owner = value; }
        public override string ProceduresAndAttentionDates { get => _procedures; set => _procedures = value; }
        public override string MedicineAndMedHistory { get => _medicine; set => _medicine = value; }
        public override string Color { get => _color; set => _color = value; }
        private byte[] img_bytes = null;

        /// <summary>
        /// Image stored in byte array
        /// </summary>
        public override byte[] ImageSource { get => img_bytes; set => img_bytes = value; }
        public override string BreedString { get => _stringbreed; set => _stringbreed = value; }
        string _stringbreed = "";
    }
}
