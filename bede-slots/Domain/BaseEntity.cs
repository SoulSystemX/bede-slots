namespace bede_slots.Domain
{
    public abstract class BaseEntity<T>
    {
        public virtual T Id { get; set; }
    }
}
