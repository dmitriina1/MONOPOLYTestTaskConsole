using MONOPOLYTestTaskConsole.Database;
using MONOPOLYTestTaskConsole.Models;
using MONOPOLYTestTaskConsole.Service;

namespace ConsoleAppTest
{
    public class PalletTests
    {
        [Fact]
        public void Pallet_AddBox_WithCorrectDimensions()
        {
            var pallet = new Pallet(1, 100, 100, 150);
            var box = new Box(1, 50, 50, 50, 10, productionDate: DateTime.Now);

            pallet.AddBox(box);

            Assert.Contains(box, pallet.Boxes);
        }

        [Fact]
        public void Pallet_AddBox_WithIncorrectDimensions()
        {
            var pallet = new Pallet(1, 100, 100, 150);
            var box = new Box(2, 120, 50, 50, 10, productionDate: DateTime.Now);

            Assert.Throws<ArgumentException>(() => pallet.AddBox(box));
        }

        [Fact]
        public void Pallet_Weight_SumBoxesWeight()
        {
            var pallet = new Pallet(1, 100, 100, 150);
            var box = new Box(3, 50, 50, 50, 10, productionDate: DateTime.Now);

            pallet.AddBox(box);

            Assert.Equal(40, pallet.Weight); 
        }

        [Fact]
        public void Pallet_ExpirationDate_FromEarliestBoxExpirationDate()
        {
            var pallet = new Pallet(1, 100, 100, 150);
            var box1 = new Box(4, 50, 50, 50, 10, expirationDate: new DateTime(2023, 5, 1));
            var box2 = new Box(5, 50, 50, 50, 10, expirationDate: new DateTime(2023, 4, 1));

            pallet.AddBox(box1);
            pallet.AddBox(box2);

            Assert.Equal(new DateTime(2023, 4, 1), pallet.ExpirationDate);
        }
    }
}