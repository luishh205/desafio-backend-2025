using Dapper;
using desafio_backend_2025.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using static desafio_backend_2025.Repositories.ReceitaWSService;
using static System.Net.Mime.MediaTypeNames;

namespace desafio_backend_2025.Repositories
{
    public class UsuarioRepository
    {
        private readonly DatabaseConnection _db;
        private readonly IConfiguration _config;

        public UsuarioRepository(DatabaseConnection db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<Response<IEnumerable<Usuario>>> GetAll()
        {
            try
            {
                using var conn = _db.GetConnection();
                var usuarios = await conn.QueryAsync<Usuario>("SELECT * FROM usuarios");

                return Response<IEnumerable<Usuario>>.Ok(usuarios);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<Usuario>>.Error($"Erro ao buscar todas as usuarios: {ex.Message}");
            }
        }

        public async Task<Response<Usuario>> GetById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Response<Usuario>.Error("ID deve ser maior que zero.");
                }

                using var conn = _db.GetConnection();

                var usuario = await conn.QuerySingleOrDefaultAsync<Usuario>(
                    "SELECT * FROM usuarios WHERE id = @id",
                    new { id })
                    .ConfigureAwait(false);

                if (usuario == null)
                {
                    return Response<Usuario>.Error($"Usuario com ID {id} não encontrada.");
                }

                return Response<Usuario>.Ok(usuario);
            }
            catch (Exception ex)
            {
                return Response<Usuario>.Error($"Erro ao buscar usuario pelo ID: {id} - {ex.Message}");
            }
        }

        public async Task<Response<Usuario>> GetUsuarioEmail(string email)
        {
            try
            {
                if (email == null)
                {
                    return Response<Usuario>.Error("E-mail não pode ser nulo.");
                }

                bool validarUsuario = ValidarEmail(email);
                if (!validarUsuario)
                {
                    return Response<Usuario>.Error($"E-mail invalido: {email}");
                }

                using var conn = _db.GetConnection();

                var usuario = await conn.QuerySingleOrDefaultAsync<Usuario>(
                    "SELECT * FROM usuarios WHERE email = @email",
                    new { email })
                    .ConfigureAwait(false);

                if (usuario == null)
                {
                    return Response<Usuario>.Error($"Usuario com E-mail {email} não encontrada.");
                }

                return Response<Usuario>.Ok(usuario);
            }
            catch (Exception ex)
            {
                return Response<Usuario>.Error($"Erro ao buscar usuario pelo E-mail: {email} - {ex.Message}");
            }
        }


        public string GerarToken(Usuario usuario)
        {
            var jwtKey = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                return "A chave JWT não está configurada.";
            }

            var key = Encoding.ASCII.GetBytes(jwtKey);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, usuario.email),
                    new Claim(ClaimTypes.NameIdentifier, usuario.id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<Response<int>> Create(Usuario usuario)
        {
            try
            {
                bool validarUsuario = ValidarEmail(usuario.email);
                if (!validarUsuario)
                {
                    return Response<int>.Error($"E-mail invalido: {usuario.email}");
                }

                using var conn = _db.GetConnection();

                conn.Open();

                using var transaction = conn.BeginTransaction();

                try
                {
                    var result = await conn.ExecuteAsync(
                        "INSERT INTO usuarios ( email, password) " +
                        "VALUES ( @email, @password)",
                        usuario, transaction);


                    transaction.Commit();

                    return Response<int>.Ok(result);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Response<int>.Error($"Erro ao criar usuario com E-mail: {usuario.email} - {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return Response<int>.Error($"Erro ao criar usuario com E-mail: {usuario.email} - {ex.Message}");
            }
        }

        public static bool ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
            return Regex.IsMatch(email, pattern);
        }

        public async Task<Response<int>> Update(Usuario usuario)
        {
            try
            {
                using var conn = _db.GetConnection();

                var result = await conn.ExecuteAsync(
                   "UPDATE usuarios SET email = @email, password = @password  WHERE id = @id",
                   usuario);
                
                return Response<int>.Ok(result);
            }
            catch (Exception ex)
            {
                return Response<int>.Error($"Erro ao atualizar usuario com ID: {usuario.id}  {ex.Message}");
            }
        }

        public async Task<Response<bool>> Delete(int id)
        {
            try
            {
                using var conn = _db.GetConnection();

                var result = await conn.ExecuteAsync("DELETE FROM usuarios WHERE id = @Id", new { Id = id });

                if (result == 0)
                {
                    return Response<bool>.Error($"Nenhum usuario encontrada com o ID: {id}");
                }

                return Response<bool>.Ok(true);

            }
            catch (Exception ex)
            {
                return Response<bool>.Error($"Erro ao deletar usuario com ID: {id} {ex.Message}");
            }
        }

    }
}
