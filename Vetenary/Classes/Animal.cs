using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Vetenary.Enums;

namespace Vetenary.Classes
{
    public  class Animal
    {
        public virtual string Name { get; set; }
        public virtual List<string> AttentionDates { get; set; }
        public virtual int Age { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual string Owner { get; set; }
        public virtual string ProceduresAndAttentionDates { get; set; }
        public virtual string MedicineAndMedHistory { get; set; }
        public virtual string Color { get; set; }

        public AnimalTypes Type { get; set; }
        public virtual dynamic Breed { get; set; }

        public virtual string BreedString { get; set; }
        private byte[] img_bytes = null;

        /// <summary>
        /// Image stored in byte array
        /// </summary>
        public virtual byte[] ImageSource { get => img_bytes; set => img_bytes = value; }

    }
}
