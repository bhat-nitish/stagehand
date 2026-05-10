using MediatR;

namespace Stagehand.SharedKernel.Application.Messaging;

public interface IQuery<TResponse> : IRequest<TResponse>
{
}
