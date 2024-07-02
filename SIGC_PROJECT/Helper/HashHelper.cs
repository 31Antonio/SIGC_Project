using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;

namespace SIGC_PROJECT.Helper
{
    public class HashHelper
    {
        //METODO PARA GENERAR UN HASH Y SALT PARA UNA CONTRASEÑA
        public static HashedPassword Hash(string password)
        {
            //Crear un array de bytes para el salt (especificamente 16)
            byte[] salt = new byte[128 / 8];

            //Genera números aleatorios para llenar el salt
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt); //Aquí se llena el salt con los numero generados
            }

            //Generar el hash de la contraseña usando PBKDF2 con HMACSHA256
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8)); // Longitud del hash en bytes (32 bytes)

                return new HashedPassword() { Password = hashed,  Salt = Convert.ToBase64String(salt) };

        }

        //METODO PARA VERIFICAR QUE LA CONTRASEÑA COINCIDA CON SU HASH 
        public static bool CheckHash(string attemptedPassword, string hash, string salt)
        {
            //Generar el hash de la contraseña usando el salt que proporciona la base de datos
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: attemptedPassword,
                salt: Convert.FromBase64String(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8 ));

            //Comparar el hash almacenado con el hash almacenado
            return hash == hashed;
        }

        // Método para obtener un hash SHA256 de una contraseña y un salt
        public static byte[] GetHash(string password, string salt)
        {
            // Combinar el salt y la contraseña en un solo string y convertirlo a bytes
            byte[] unhashedBytes = Encoding.Unicode.GetBytes(string.Concat(salt, password));

            //Crear una instancia de SHA256Managed para generar el hash
            SHA256Managed sha256 = new SHA256Managed();

            //Calcular el hash SHA256 de los bytes combinados de la contraseña y el salt
            byte[] hashedBytes = sha256.ComputeHash(unhashedBytes);
            return hashedBytes;
        }
    }

    public class HashedPassword
    {
        public string Password { get; set; }
        public string Salt { get; set; }
    }
}
