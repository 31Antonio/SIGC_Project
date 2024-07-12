using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;

namespace SIGC_PROJECT.Helper
{
    public class HashHelper
    {
        //METODO PARA CREAR UN SALT DERIVADO DEL NOMBRE DE USUARIO
        private static byte[] SaltDerivada(string username)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(username));
            }
        }

        //METODO PARA GENERAR UN HASH Y SALT PARA UNA CONTRASEÑA
        public static HashedPassword Hash(string password, string username)
        {
            //Crear un array de bytes para el salt (sacado del nombre de usuario)
            byte[] salt = SaltDerivada(username);

            //Generar el hash de la contraseña usando PBKDF2 con HMACSHA256
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8)); // Longitud del hash en bytes (32 bytes)

                return new HashedPassword() { Password = hashed};

        }

        //METODO PARA VERIFICAR QUE LA CONTRASEÑA COINCIDA CON SU HASH 
        public static bool CheckHash(string attemptedPassword, string hash, string username)
        {
            //Generar el hash de la contraseña usando el salt que proporciona el metodo
            byte[] salt = SaltDerivada(username);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: attemptedPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8 ));

            //Comparar el hash almacenado con el hash almacenado
            return hash == hashed;
        }

        // METODO PARA OBTENER UN HASH SHA256 DE UNA CONTRASEÑA Y EL NOMBRE DE USUARIO
        public static byte[] GetHash(string password, string username)
        {
            // Combinar el salt y la contraseña en un solo string y convertirlo a bytes
            byte[] unhashedBytes = Encoding.Unicode.GetBytes(string.Concat(username, password));

            //Crear una instancia de SHA256Managed para generar el hash
            SHA256Managed sha256 = new SHA256Managed();

            //Calcular el hash SHA256 de los bytes combinados de la contraseña y el salt
            byte[] hashedBytes = sha256.ComputeHash(unhashedBytes);
            return hashedBytes;
        }

        // METODO PARA VERIFICAR LA CONTRASEÑA ANTES DE ACTUALIZAR
        public static bool VerifyPass (string currentPassword, string dbPassword, string userName)
        {
            var hashedPassword = Hash(currentPassword, userName);
            return hashedPassword.Password == dbPassword;
        }
    }

    public class HashedPassword
    {
        public string Password { get; set; }
    }
}
