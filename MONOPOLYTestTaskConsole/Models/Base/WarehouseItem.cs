using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONOPOLYTestTaskConsole.Models.Base
{
    public class WarehouseItem
    {
        public int ID { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public virtual double Weight { get; set; }

        public double Volume => Width * Height * Depth;

        protected WarehouseItem(int id, double width, double height, double depth, double weight)
        {
            ID = id;
            Width = width;
            Height = height;
            Depth = depth;
            Weight = weight;
        }
    }
}
