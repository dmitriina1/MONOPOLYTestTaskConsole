using MONOPOLYTestTaskConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppTest
{
    public class BoxTests
    {
        [Fact]
        public void Box_Creation_WithOnlyExpirationDate()
        {
            var expirationDate = new DateTime(2023, 5, 1);

            var box = new Box(1, 50, 50, 50, 10, expirationDate: expirationDate);

            Assert.Equal(expirationDate, box.ExpirationDate);
            Assert.Equal(expirationDate.AddDays(-100), box.ProductionDate);
        }

        [Fact]
        public void Box_Creation_WithOnlyProductionDate()
        {
            var productionDate = new DateTime(2023, 1, 1);

            var box = new Box(2, 50, 50, 50, 10, productionDate: productionDate);

            Assert.Equal(productionDate, box.ProductionDate);
            Assert.Equal(productionDate.AddDays(100), box.ExpirationDate);
        }

        [Fact]
        public void Box_Creation_WithoutDates()
        {
            Assert.Throws<ArgumentException>(() => new Box(3, 50, 50, 50, 10));
        }
    }
}
