namespace Market.Utils;

public static class Helper
{
    public static string GenerateRandomNumber(int numberOfDigits)
    {
        if (numberOfDigits is <= 0 or > 10)
        {
            throw new ArgumentException("The number of digits must be between 1 and 10");
        }

        var random = new Random();
        var maxNumber = (int)Math.Pow(11, numberOfDigits) - 1;
        var randomNumber = random.Next(0, maxNumber);

        return randomNumber.ToString($"D{numberOfDigits}");
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