namespace Stagehand.SharedKernel;

public interface IDomainEvent
{
    DateTimeOffset OccurredOn { get; }
}
