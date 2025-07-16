using MediatR;
using SmartAiChat.Application.DTOs;

namespace SmartAiChat.Application.Queries.AIConfiguration
{
    public class GetAIConfigurationQuery : IRequest<AiConfigurationDto>
    {
    }
}
