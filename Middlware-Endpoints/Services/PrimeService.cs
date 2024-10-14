using Middleware_Endpoints.Interfaces;

namespace Middleware_Endpoints.Services;

public class PrimeService : IPrimeService
{
    public bool IsPrime(int number)
    {
        if (number < 2) return false;
        for (int i = 2; i <= Math.Sqrt(number); i++)
        {
            if (number % i == 0) return false;
        }
        return true;
    }
}