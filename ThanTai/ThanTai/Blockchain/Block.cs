using System;
using System.Security.Cryptography;
using System.Text;

namespace ThanTai.Blockchain
{
    public class Block
    {
        public int Index { get; set; }
        public DateTime Timestamp { get; set; }
        public string PreviousHash { get; set; }
        public string Hash { get; set; }
        public string Data { get; set; }

        public Block(int index, string previousHash, string data)
        {
            Index = index;
            Timestamp = DateTime.UtcNow;
            PreviousHash = previousHash;
            Data = data;
            Hash = CalculateHash();
        }

        public string CalculateHash()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string rawData = $"{Index}-{Timestamp}-{PreviousHash}-{Data}";
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
