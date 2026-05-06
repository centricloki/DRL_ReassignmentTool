using System;
using System.Linq;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace DRL.Library
{
    public static class PinValidator
    {
        private static bool IsSequential(string pin)
        {
            // 1234, 2345... or 4321, 5432...
            if (pin.Length < 2) return false;

            bool ascending = true;
            bool descending = true;

            for (int i = 1; i < pin.Length; i++)
            {
                int prev = pin[i - 1] - '0';
                int curr = pin[i] - '0';

                if (curr - prev != 1) ascending = false;
                if (prev - curr != 1) descending = false;

                // early exit
                if (!ascending && !descending) return false;
            }
            return ascending || descending;
        }

        public static bool IsWeakPin(string pin)
        {
            if (string.IsNullOrWhiteSpace(pin) || pin.Length != 4 || !pin.All(char.IsDigit))
                return true;

            // 1) all same
            if (pin.Distinct().Count() == 1)
                return true;

            // 2) sequential
            if (IsSequential(pin))
                return true;

            return false;
        }

        // Framework-compatible random 0-9999
        private static int GetSecureRandom(int min, int max)
        {
            using (var rng = RandomNumberGenerator.Create()) // works in.NET 4.5+
            {
                byte[] bytes = new byte[4];
                rng.GetBytes(bytes);
                uint random = BitConverter.ToUInt32(bytes, 0);
                return (int)(min + (random % (uint)(max - min)));
            }
        }

        public static string GenerateStrongPin()
        {
            for (int i = 0; i < 50; i++)
            {
                int num = GetSecureRandom(0, 10000); // 0 to 9999
                string pin = num.ToString("D4"); // keeps leading zeros

                if (!IsWeakPin(pin))
                    return pin;
            }
            throw new InvalidOperationException("Failed to generate valid PIN");
        }
    }
}