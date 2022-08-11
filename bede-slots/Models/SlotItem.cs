using bede_slots.Domain;

namespace bede_slots.Models
{
    public class SlotItem : BaseEntity<int>
    {
        public char Symbol { get; set; }
        public string Name { get; set; }
        public decimal Coefficent { get; set; }
        public int ProbabilityOfAppearance { get; set; }
    }
}
