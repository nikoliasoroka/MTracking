namespace MTracking.Core.Entities.Abstractions
{
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
}
