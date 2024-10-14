using Middleware_Endpoints.Interfaces;

namespace Middleware_Endpoints.Services;

public class PalindromeService : IPalindromeService
{
    public bool IsPalindrome(int number)
    {
        var str = number.ToString();
        return str == new string(str.Reverse().ToArray());
    }
    // Palindrome Algorithm - Second Way
    //public bool _isPalindrome(int number)
    //{
    //    string strNumber = number.ToString();
    //    int numberLength = strNumber.Length;
    //    for (int i = 0; i < numberLength; i++)
    //    { if (strNumber[i] != strNumber[numberLength - i - 1]) return false; }
    //    return true;
    //}
}