using Canopy.API.Common.Models.DTO;
using ServiceBus.Events;

namespace Canopy.CMT.ActionPlans.Services.RPCCalls.RPCCalls;

public record UserValidationRPCCall(UserAuthRequestDTO EventData)
    : RPCCall("UserValidationRPCCall", "Canopy.Core.Identity.ValidateUser.Response");