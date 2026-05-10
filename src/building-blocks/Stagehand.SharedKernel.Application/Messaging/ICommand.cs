using MediatR;

namespace Stagehand.SharedKernel.Application.Messaging;

public interface ICommand<TResponse> : IRequest<TResponse>
{
}
