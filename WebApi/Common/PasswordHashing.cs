using System.Security.Cryptography;
using System.Text;

namespace WebApi.Common;

public class PasswordHasher
{
    // Salt size in bytes
    private const int SaltSize = 16;

    // Hash size in bytes
    private const int HashSize = 20;

    // Iterations count
    private const int Iterations = 562200;

    public static string HashPassword(string password)
    {
        // Generate a random salt
        byte[] salt;
        new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

        // Create a new instance of the Rfc2898DeriveBytes class
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);

        // Get the hash value
        byte[] hash = pbkdf2.GetBytes(HashSize);

        // Combine salt and hash
        byte[] hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        // Convert to base64
        string hashedPassword = Convert.ToBase64String(hashBytes);

        return hashedPassword;
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        // Convert the base64 string to byte array
        byte[] hashBytes = Convert.FromBase64String(hashedPassword);

        // Extract the salt from the hashed password
        byte[] salt = new byte[SaltSize];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);

        // Compute the hash on the password the user entered
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
        byte[] hash = pbkdf2.GetBytes(HashSize);

        // Compare the hashes
        for (int i = 0; i < HashSize; i++)
        {
            if (hashBytes[i + SaltSize] != hash[i])
                return false;
        }

        return true;
    }
}






//public class PasswordHashing
//{
//    const int keySize = 64;
//    const int iterations = 350000;
//    HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

//    public string HashPassword(string password, out byte[] salt)
//    {
//        salt = RandomNumberGenerator.GetBytes(keySize);
//        var hash = Rfc2898DeriveBytes.Pbkdf2(
//            Encoding.UTF8.GetBytes(password),
//            salt,
//            iterations,
//            hashAlgorithm,
//            keySize);
//        return Convert.ToHexString(hash);
//    }
//    public bool VerifyPassword(string password, string hash, byte[] salt)
//    {
//        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);
//        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
//    }



//    public static byte[] GenerateSalt()
//    {
//        var randomNumberGenerator = new RNGCryptoServiceProvider();
//        var salt = new byte[16];
//        randomNumberGenerator.GetBytes(salt);
//        return salt;
//    }


//    public static string HashPassword(string password, byte[] salt)
//    {
//        var keyDerivation = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
//        var hashedPassword = Convert.ToBase64String(keyDerivation.GetBytes(32));
//        return hashedPassword;
//    }
//}
