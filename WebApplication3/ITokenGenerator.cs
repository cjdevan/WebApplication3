namespace WebApplication3
{
    public interface ITokenGenerator
    {
        string GenerateRandomToken(int length);
    }

    public class RandomTokenGenerator : ITokenGenerator
    {
        private readonly Random _random = new Random();

        public string GenerateRandomToken(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }

}
