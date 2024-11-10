using MONOPOLYTestTaskConsole.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONOPOLYTestTaskConsole.Models
{
    public class Box : WarehouseItem
    {
        public DateTime? ExpirationDate { get; private set; }
        public DateTime? ProductionDate { get; private set; }

        public Box(int id, double width, double height, double depth, double weight, DateTime? productionDate = null, DateTime? expirationDate = null)
            : base(id, width, height, depth, weight)
        {
            if (!productionDate.HasValue && !expirationDate.HasValue)
            {
                throw new ArgumentException("Either ProductionDate or ExpirationDate must be specified.");
            }

            if (expirationDate.HasValue)
            {
                ExpirationDate = expirationDate;
                ProductionDate = productionDate ?? expirationDate.Value.AddDays(-100);
            }
            else if (productionDate.HasValue)
            {
                ProductionDate = productionDate;
                ExpirationDate = productionDate.Value.AddDays(100);
            }
        }
    }
}
