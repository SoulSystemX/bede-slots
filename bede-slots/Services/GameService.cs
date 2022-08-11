using bede_slots.Domain;
using bede_slots.Models;
using bede_slots.ViewModels;

namespace bede_slots.Services
{

    public interface IGameService
    {
        public void Deposit(decimal amount, int playerId);
        public decimal GetBalance(int playerId);
         public List<List<SlotItem>> Spin(decimal stake, int playerId);
        public List<Player> GetPlayer();
        public List<List<SlotItem>> GetGameBoard(IEnumerable<SlotItem> slotItems);
        SlotItem GetRandomSlotItem(IEnumerable<SlotItem> slotItems);
        public List<decimal> CalcuateWinningRowsCoefficents (List<List<SlotItem>> gameBoard);
        public decimal CalculateWinnings(decimal stake, int playerId, decimal sumOfCoefficents) ;
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

        public decimal GetBalance(int playerId)
        {
            return _unitOfWork.PlayerRepo.Get(playerId).Result.Balance;
        }

        public List<List<SlotItem>> Spin(decimal stake, int playerId)
        {

            if (GetBalance(playerId) == 0)
                throw new Exception("Player's balance is 0");
            // get all the slot items
            var slotItems = _unitOfWork.SlotItemRepo.GetAll();

            var gameBoard = GetGameBoard(slotItems);

            return gameBoard;
        }

        public List<decimal> CalcuateWinningRowsCoefficents (List<List<SlotItem>> gameBoard)
        {
           List<decimal> rowResults = new();

           foreach(var row in gameBoard)
           {
            var coefficents = IsRowAWinner(row);
            decimal finalCoefficent = 0.0m;
            foreach(var coefficent in coefficents)
            {
                finalCoefficent += coefficent;
            }


            rowResults.Add(finalCoefficent);
                
           }

           return rowResults;
        } 

        public bool IsWildcard(SlotItem slotItem) => slotItem.Symbol == '*' ? true : false;

        public List<decimal> IsRowAWinner(List<SlotItem> row)
        {
            List<decimal> coefficents = new();
            char? matchResult = null;

            foreach(var item in row)
            {
                if (item.Symbol == '*')
                {
                    coefficents.Add(item.Coefficent);
                    continue;
                }

                if(matchResult == null)
                {
                    matchResult = item.Symbol;
                    coefficents.Add(item.Coefficent); 
                    continue;
                }

                if(item.Symbol != matchResult && item.Symbol != '*')
                    return new List<decimal>() { 0.0m, 0.0m,0.0m};
                 
                coefficents.Add(item.Coefficent); 
            }

            return coefficents;
        }
        

        public decimal CalculateWinnings(decimal stake, int playerId, decimal sumOfCoefficents) 
        {
            var deposit = GetBalance(playerId);
            var winnings = stake + sumOfCoefficents;

            return deposit - stake + winnings;

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

        public List<SlotItem> GetRow(int numberOfColumns, IEnumerable<SlotItem> validSlotItems){
            List<SlotItem> row = new List<SlotItem>();

            for(int i = 0; i < numberOfColumns; i++)
                row.Add(GetRandomSlotItem(validSlotItems));
            
            return row;
        }

        public SlotItem GetRandomSlotItem(IEnumerable<SlotItem> slotItems)
        {
            // work out the max number from all the percentages from the slot items
            int max =  GetMaxPercentageOfSlotItems(slotItems);
            int selectedSlot = GetRandomFromCount(max);

            // add the probabilities to the one previously i.e. (first 45, second 35, third 15 and fourth 5)
            // so the probability mapper will have first id as 45, next id as 80, next 95 and final 100
            Dictionary<int,int> probabilityRange = GetProbabilityRange(slotItems.ToList());

            // Randomly pick a number from the max number of slot item probabilities ( this example we have 100 is the max so it has a 45% chance to pick the first item)
            foreach(var item in probabilityRange)
            {
                if(Enumerable.Range(0, item.Value).Contains(selectedSlot))
                { 
                    return slotItems.Where(w => w.Id == item.Key).First();
                }

            }

            // since we have a key for the randomly selected slotitem grab it and return it
            throw new Exception("Random number did not appear within the probability range");
        }

        private Dictionary<int,int> GetProbabilityRange(List<SlotItem> slotItems){

            var probabilityIndex = new Dictionary<int, int>();

            probabilityIndex.Add(slotItems[0].Id, slotItems[0].ProbabilityOfAppearance);

            for(int i = 1; i < slotItems.Count(); i++)
            {
                probabilityIndex.Add(slotItems[i].Id, slotItems[i].ProbabilityOfAppearance + probabilityIndex.Last().Value);
            }

            return probabilityIndex;
        }

        private int GetMaxPercentageOfSlotItems(IEnumerable<SlotItem> slotItems){
            int maxPercentage = 0;

            foreach(var slotItem in slotItems){
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
