
using bede_slots.Models;

namespace bede_slots.ViewModels
{
    public class SpinResultVM
    {
             public List<List<SlotItem>> Gameboard {get;set;}

             public decimal Balance {get;set;}
             public decimal Winnings {get;set;}
    }
}
