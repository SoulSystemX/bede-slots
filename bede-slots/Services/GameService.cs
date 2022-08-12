using bede_slots.Domain;
using bede_slots.Models;
using bede_slots.ViewModels;

namespace bede_slots.Services
{

    public interface IGameService
    {
        public void Deposit(decimal amount, int playerId);
        public Task<decimal> GetBalance(int playerId);
        public  Task<List<List<SlotItem>>> Spin(decimal stake, int playerId);
        public List<Player> GetPlayer();
        public List<List<SlotItem>> GetGameBoard(IEnumerable<SlotItem> slotItems);
        SlotItem GetRandomSlotItem(IEnumerable<SlotItem> slotItems);
        public List<decimal> CalcuateWinningRowsCoefficents(List<List<SlotItem>> gameBoard);
        public  Task<decimal> CalculateWinnings(decimal stake, int playerId, decimal sumOfCoefficents);
    }

    public class GameService : IGameService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GameService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async void Deposit(decimal amount, int playerId)
        {
            var player = await _unitOfWork.PlayerRepo.Get(playerId);
            player.Balance += amount;

            _unitOfWork.PlayerRepo.Update(player);
            await _unitOfWork.SaveAsync();
        }

        public List<Player> GetPlayer()
        {
            return _unitOfWork.PlayerRepo.GetAll().ToList();
        }

        public async Task<decimal> GetBalance(int playerId)
        {
            var player = await _unitOfWork.PlayerRepo.Get(playerId);
            return player.Balance;
        }

        public async Task<List<List<SlotItem>>> Spin(decimal stake, int playerId)
        {

            var currentBalance = await GetBalance(playerId); 

            if (currentBalance == 0.0m)
                throw new Exception("Player's balance is 0");

            if(currentBalance - stake < 0.0m)
                throw new Exception("You haven't enough money");

            // get all the slot items
            var slotItems = _unitOfWork.SlotItemRepo.GetAll();

            //remove the players stake from their balance
            var player = await _unitOfWork.PlayerRepo.Get(playerId);
            player.Balance = player.Balance - stake;
            _unitOfWork.PlayerRepo.Update(player);
            await _unitOfWork.SaveAsync();

            // generate a new gameboard 
            return GetGameBoard(slotItems);
        }

        public List<decimal> CalcuateWinningRowsCoefficents(List<List<SlotItem>> gameBoard)
        {

            if (gameBoard.Any(a => a.Any(b => b == null)))
                throw new NullReferenceException("Gameboard has missing slotitems");

            List<decimal> rowResults = new();

            foreach (var row in gameBoard)
            {
                var coefficents = IsRowAWinner(row);

                decimal finalCoefficent = 0.0m;

                foreach (var coefficent in coefficents)
                {
                    finalCoefficent += coefficent;
                }

                rowResults.Add(finalCoefficent);

            }

            return rowResults;
        }

        // Should remove no longer needed
        public bool IsWildcard(SlotItem slotItem) => slotItem.Symbol == '*' ? true : false;

        public List<decimal> IsRowAWinner(List<SlotItem> row)
        {
            List<decimal> coefficents = new();
            char? matchResult = null;

            foreach (var item in row)
            {
                if (item.Symbol == '*')
                {
                    coefficents.Add(item.Coefficent);
                    continue;
                }

                if (matchResult == null)
                {
                    matchResult = item.Symbol;
                    coefficents.Add(item.Coefficent);
                    continue;
                }

                if (item.Symbol != matchResult && item.Symbol != '*')
                    return new List<decimal>() { 0.0m, 0.0m, 0.0m };

                coefficents.Add(item.Coefficent);
            }

            return coefficents;
        }


        public async Task<decimal> CalculateWinnings(decimal stake, int playerId, decimal sumOfCoefficents)
        {
            
            // should round to nearest penny
            var winnings = stake * sumOfCoefficents;

            if(winnings <= 0)
                return 0;

            var player = await _unitOfWork.PlayerRepo.Get(playerId);
            player.Balance += winnings;
             _unitOfWork.PlayerRepo.Update(player);
             await _unitOfWork.SaveAsync();

            return winnings;

        }


        public List<List<SlotItem>> GetGameBoard(IEnumerable<SlotItem> slotItems)
        {
            List<List<SlotItem>> gameBoard = new List<List<SlotItem>>()
            {
                new List<SlotItem>()
                {
                    GetRandomSlotItem(slotItems),
                    GetRandomSlotItem(slotItems),
                    GetRandomSlotItem(slotItems),
                },
                new List<SlotItem>()
                {
                    GetRandomSlotItem(slotItems),
                    GetRandomSlotItem(slotItems),
                    GetRandomSlotItem(slotItems),
                },
                new List<SlotItem>()
                {
                    GetRandomSlotItem(slotItems),
                    GetRandomSlotItem(slotItems),
                    GetRandomSlotItem(slotItems),
                },
                new List<SlotItem>()
                {
                    GetRandomSlotItem(slotItems),
                    GetRandomSlotItem(slotItems),
                    GetRandomSlotItem(slotItems),
                }
            };

            return gameBoard;

        }

        public SlotItem GetRandomSlotItem(IEnumerable<SlotItem> slotItems)
        {
            // work out the max number from all the percentages from the slot items
            int max = GetMaxPercentageOfSlotItems(slotItems);
            int selectedSlot = GetRandomFromCount(max);

            // add the probabilities to the one previously i.e. (first 45, second 35, third 15 and fourth 5)
            // so the probability mapper will have first id as 45, next id as 80, next 95 and final 100
            Dictionary<int, int> probabilityRange = GetProbabilityRange(slotItems.ToList());

            // Randomly pick a number from the max number of slot item probabilities ( this example we have 100 is the max so it has a 45% chance to pick the first item)
            foreach (var item in probabilityRange)
            {
                if (Enumerable.Range(0, item.Value).Contains(selectedSlot))
                {
            // since we have a key for the randomly selected slotitem grab it and return it
                    return slotItems.Where(w => w.Id == item.Key).First();
                }

            }

            //Something went wrong with the value we sent in
            throw new Exception("Random number did not appear within the probability range");
        }

        private Dictionary<int, int> GetProbabilityRange(List<SlotItem> slotItems)
        {

            var probabilityIndex = new Dictionary<int, int>();

            probabilityIndex.Add(slotItems[0].Id, slotItems[0].ProbabilityOfAppearance);

            for (int i = 1; i < slotItems.Count(); i++)
            {
                probabilityIndex.Add(slotItems[i].Id, slotItems[i].ProbabilityOfAppearance + probabilityIndex.Last().Value);
            }

            return probabilityIndex;
        }

        private int GetMaxPercentageOfSlotItems(IEnumerable<SlotItem> slotItems)
        {
            int maxPercentage = 0;

            foreach (var slotItem in slotItems)
            {
                maxPercentage += slotItem.ProbabilityOfAppearance;
            }

            return maxPercentage;
        }

        private int GetRandomFromCount(int max)
        {
            Random random = new();
            return random.Next(1, max);
        }
    }
}
