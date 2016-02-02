/* Tom Postler, 2016-02-02
 * Generates a default 100-character cryptographically secure random password consisting of the 
 * following characters: [a-zA-Z0-9]
 *
 * Usage:
 *    ./RandomPassword.exe [length]
 */

using System;
using System.Security.Cryptography;
using System.Text;

class RandomPassword
{
    private static RNGCryptoServiceProvider _rngCsp = new RNGCryptoServiceProvider();
    private static readonly string _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static void Main(string[] args)
    {
        ushort result;

        if (args.Length == 1 && UInt16.TryParse(args[0], out result))
        {
            GenerateAndDisplayString(result);
        }
        else
        {
            GenerateAndDisplayString();
        }
    }

    private static void GenerateAndDisplayString(ushort length = 100)
    {
        byte[] randomBit = new byte[1];
        byte[] randomBits = new byte[length];

        // Fill the random bits with approved numbers
        for (int i = 0; i < randomBits.Length; i++)
        {
            // Generate a valid random bit
            do
            {
                _rngCsp.GetBytes(randomBit);
            }
            while (randomBit[0] >= _chars.Length);

            randomBits[i] = randomBit[0];
        }

        // Convert the bits to chars
        StringBuilder sb = new StringBuilder(length);
        foreach (byte b in randomBits)
        {
            sb.Append(_chars[b]);
        }

        // Print string out
        Console.WriteLine(sb.ToString());
    }
}

