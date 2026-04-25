using AutoMapper;

public sealed class AutoMapperMissingMapIssueHandler(IMapper mapper)
{
    public IResult Handle()
    {
        var request = new SampleMappingRequest("demo-order", 3);
        return Results.Ok(mapper.Map<SampleMappingResponse>(request));
    }
}
