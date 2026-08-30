using MediatR;

namespace Stagehand.SharedKernel;

public interface IDomainEvent : INotification
{
    DateTimeOffset OccurredOn { get; }
}
