using System.Security.Cryptography;

namespace Market.Utils;

public static class Helper
{
    
    public static long GenerateRandomNumber(int numberOfDigits)
    {
        if (numberOfDigits <= 0)
        {
            throw new ArgumentException("Please provide some numbers.");
        }

        var rng = RandomNumberGenerator.Create();
        var bytes = new byte[numberOfDigits];

        rng.GetBytes(bytes);
        
        var maxNumber = (int)Math.Pow(10, numberOfDigits);
        var randomValue = BitConverter.ToInt32(bytes, 0) % maxNumber;

        return randomValue;

    }
    
    public static string ToTitleCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }
        
        var words = input.ToLower().Split(' ');
        for (var i = 0; i < words.Length; i++)
        {
            if (!string.IsNullOrEmpty(words[i]))
                words[i] = char.ToUpper(words[i][0]) + words[i][1..];
        }
        return string.Join(" ", words);
    }
    
    public static string ToUpper(string input)
    {
        return string.IsNullOrEmpty(input) ? input : input.ToUpper();
    }
}