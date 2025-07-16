using AutoMapper;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Domain.Entities;
using System.Text.Json;

namespace SmartAiChat.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Tenant, TenantDto>();
        CreateMap<User, UserDto>();
        CreateMap<ChatSession, ChatSessionDto>()
            .AfterMap((src, dest) => 
            {
                if (!string.IsNullOrEmpty(src.Tags))
                {
                    try 
                    {
                        dest.Tags = JsonSerializer.Deserialize<List<string>>(src.Tags) ?? new List<string>();
                    }
                    catch 
                    {
                        dest.Tags = new List<string>();
                    }
                }
            });
        
        CreateMap<ChatMessage, ChatMessageDto>();
        CreateMap<AiConfiguration, AiConfigurationDto>();
        CreateMap<TenantPlan, TenantPlanDto>();
        CreateMap<TenantSubscription, TenantSubscriptionDto>();
        CreateMap<FaqEntry, FaqEntryDto>()
            .AfterMap((src, dest) => 
            {
                if (!string.IsNullOrEmpty(src.Tags))
                {
                    try 
                    {
                        dest.Tags = JsonSerializer.Deserialize<List<string>>(src.Tags) ?? new List<string>();
                    }
                    catch 
                    {
                        dest.Tags = new List<string>();
                    }
                }
            });
        
        CreateMap<AiTrainingFile, AiTrainingFileDto>()
            .AfterMap((src, dest) => 
            {
                if (!string.IsNullOrEmpty(src.Tags))
                {
                    try 
                    {
                        dest.Tags = JsonSerializer.Deserialize<List<string>>(src.Tags) ?? new List<string>();
                    }
                    catch 
                    {
                        dest.Tags = new List<string>();
                    }
                }
            });
    }
} 