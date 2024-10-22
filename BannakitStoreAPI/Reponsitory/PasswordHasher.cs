using BannakitStoreApi.Reponsitory.IReponsitory;
using System;
using System.Security.Cryptography;

namespace BannakitStoreApi.Reponsitory
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 128 / 8; // 16 byte เป็นการแบ่ง bit เป็น 1 byte (1 byte = 8 bit) เพื่อทำให้ขนาดข้อมูลสอดคล้องกัน
        private const int KeySize = 256 / 8; // 32 byte
        private const int Iteration = 10000; // จำนวนรอบที่ PBKDF2 จะทำการคำนวณแฮชซ้ำ ๆ เพื่อทำให้การแฮชรหัสผ่านใช้เวลานานขึ้นและป้องกันการโจมตีแบบ brute force
        private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;
        private static char Delimiter = ';';

        public string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize); // 16 byte
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iteration, _hashAlgorithmName, KeySize); // 32 byte

            return string.Join(Delimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash)); // Convert salt and hash are Base64 string และรวมกันด้วยเครื่องหมาย ;
        }

        public bool Verify(string passwordHash, string inputPassword)
        {
            var elements = passwordHash.Split(Delimiter);
            var salt = Convert.FromBase64String(elements[0]); // 16 byte
            var hash = Convert.FromBase64String(elements[1]); // 32 byte

            var hashInput = Rfc2898DeriveBytes.Pbkdf2(inputPassword, salt, Iteration, _hashAlgorithmName, KeySize); // 32 byte

            return CryptographicOperations.FixedTimeEquals(hash, hashInput);
        }
    }
}
