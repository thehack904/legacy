using ErsatzTV.Application.Scheduling;
using ErsatzTV.Core.Api.Decos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ErsatzTV.Controllers.Api;

[ApiController]
[EndpointGroupName("general")]
public class DecoController(IMediator mediator) : ControllerBase
{
    [HttpGet("/api/deco-groups", Name = "GetDecoGroups")]
    public async Task<List<DecoGroupResponseModel>> GetDecoGroups(CancellationToken cancellationToken)
    {
        List<DecoGroupViewModel> groups =
            await mediator.Send(new GetAllDecoGroups(), cancellationToken);

        return groups
            .Select(g => new DecoGroupResponseModel(g.Id, g.Name, g.DecoCount))
            .ToList();
    }

    [HttpGet("/api/deco-groups/{id:int}/decos", Name = "GetDecos")]
    public async Task<List<DecoResponseModel>> GetDecos(int id, CancellationToken cancellationToken)
    {
        List<DecoViewModel> decos =
            await mediator.Send(new GetDecosByDecoGroupId(id), cancellationToken);

        return decos
            .Select(d => new DecoResponseModel(d.Id, d.DecoGroupId, d.Name))
            .ToList();
    }

    [HttpGet("/api/playouts/{id:int}/default-deco", Name = "GetDefaultDeco")]
    public async Task<IActionResult> GetDefaultDeco(int id, CancellationToken cancellationToken)
    {
        var maybeDeco = await mediator.Send(new GetDecoByPlayoutId(id), cancellationToken);

        foreach (DecoViewModel deco in maybeDeco)
        {
            return Ok(new DecoResponseModel(deco.Id, deco.DecoGroupId, deco.Name));
        }

        return NoContent();
    }

    [HttpPut("/api/playouts/{id:int}/default-deco/{decoId:int}", Name = "SetDefaultDeco")]
    public async Task<IActionResult> SetDefaultDeco(int id, int decoId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateDefaultDeco(id, decoId), cancellationToken);

        return result.Match<IActionResult>(
            error => BadRequest(error.Value),
            Ok);
    }

    [HttpDelete("/api/playouts/{id:int}/default-deco", Name = "ClearDefaultDeco")]
    public async Task<IActionResult> ClearDefaultDeco(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateDefaultDeco(id, null), cancellationToken);

        return result.Match<IActionResult>(
            error => BadRequest(error.Value),
            Ok);
    }
}
