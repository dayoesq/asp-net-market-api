namespace Market.Utils;

public static class StringGenerator
{
    public static string GenerateRandomNumber(int numberOfDigits)
    {
        if (numberOfDigits is <= 0 or > 10)
        {
            throw new ArgumentException("The number of digits must be between 1 and 10.");
        }

        var random = new Random();
        var maxNumber = (int)Math.Pow(11, numberOfDigits) - 1;
        var randomNumber = random.Next(0, maxNumber);

        return randomNumber.ToString($"D{numberOfDigits}");
    }
}