using bede_slots.Services;
using bede_slots.Models;
using FakeItEasy;
using bede_slots.Domain;
using Microsoft.EntityFrameworkCore;

namespace bede_slots.tests
{

    public class UnitTest1
    {
        private GameService _target;
        private readonly AppDbContext _appDbContext;
        private readonly UnitOfWork _unitOfWork;

        public UnitTest1()
        {
            _appDbContext = new AppDbContext(new DbContextOptions<AppDbContext>());
            _unitOfWork = A.Fake<UnitOfWork>(opt => opt.WithArgumentsForConstructor(new List<object>() { _appDbContext }));
            _target = new GameService(_unitOfWork);
        }
        [Fact]
        public void CalcuateWinningRowsCoefficents_WithNullGameboard_ShouldThrowAnNullReferenceException()
        {
            // Arrange
            List<List<SlotItem>> gameBoard = new List<List<SlotItem>> {
                new List<SlotItem> { null,null,null},
                new List<SlotItem> { null,null,null},
                new List<SlotItem> { null,null,null},
                new List<SlotItem> { null,null,null},
            };


             // Act
            Action result = () => _target.CalcuateWinningRowsCoefficents(gameBoard);

            // Assert
            result.Should().Throw<NullReferenceException>().WithMessage("Gameboard has missing slotitems");
         }

        [Fact]
        public void CalcuateWinningRowsCoefficents_WithNoWinningRows_ShouldReturnZero()
        {
            // Arrange
            List<List<SlotItem>> gameBoard = new List<List<SlotItem>> {
                new List<SlotItem> { new SlotItem(),new SlotItem(),new SlotItem() },
                new List<SlotItem> { new SlotItem(),new SlotItem(),new SlotItem() },
                new List<SlotItem> { new SlotItem(),new SlotItem(),new SlotItem() },
                new List<SlotItem> { new SlotItem(), new SlotItem(), new SlotItem() },
            }; ;

            // Act
            var result = _target.CalcuateWinningRowsCoefficents(gameBoard);

            // Assert
            result.Should().BeEquivalentTo(new List<decimal> { 0.0m, 0.0m, 0.0m, 0.0m});
        }

        // public bool IsWildcard(SlotItem slotItem) => slotItem.Symbol == '*' ? true : false;

        [Theory]
         [InlineData('!', false)]
         [InlineData('a', false)]
         [InlineData('*', true)]
        public void IsWildcard_WithNoWinningRows_ShouldReturmZero(char symbol, bool expectedResult)
        {
            // Arrange
            SlotItem slotItem = new SlotItem{Symbol= symbol};

            // Act
            var result = _target.IsWildcard(slotItem);

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}