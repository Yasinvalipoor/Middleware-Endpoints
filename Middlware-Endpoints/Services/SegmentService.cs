using Middleware_Endpoints.Interfaces;

namespace Middleware_Endpoints.Services;

public class SegmentService : ISegmentService
{
    public string GetSegmentType(string segment)
    {
        if (int.TryParse(segment, out _)) return "Integer";
        if (bool.TryParse(segment, out _)) return "Boolean";
        return "String";
    }
}