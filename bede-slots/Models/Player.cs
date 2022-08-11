using bede_slots.Domain;

namespace bede_slots.Models
{
    public class Player : BaseEntity<int>
    {

        public decimal Balance { get; set; } = 0.0m;
     }
}
