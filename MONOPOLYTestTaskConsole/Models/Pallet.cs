using MONOPOLYTestTaskConsole.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONOPOLYTestTaskConsole.Models
{
    public class Pallet : WarehouseItem
    {
        public List<Box> Boxes { get; set; } = new List<Box>();

        public Pallet(int id, double width, double height, double depth)
            : base(id, width, height, depth, 30)
        {
        }

        public override double Weight
        {
            get
            {
                return base.Weight + Boxes.Sum(box => box.Weight);
            }
            set
            {
                base.Weight = value;
            }
        }

        public double VolumeSum => base.Volume + Boxes.Sum(box => box.Volume);

        public DateTime? ExpirationDate => Boxes.Any() ? Boxes.Min(box => box.ExpirationDate?.Date) : null;

        public void AddBox(Box box)
        {
            if (box.Width > Width || box.Depth > Depth)
                throw new ArgumentException("Ширина, либо глубина коробки не подходит!");
            else
            {
                Boxes.Add(box);
            }
        }
    }
}
