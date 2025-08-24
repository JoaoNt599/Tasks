using Tasks_Backend.Entities;
using Tasks_Backend.enums;
using Tasks_Backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Tasks_Backend.DTOs;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;


namespace Tasks_Backend.Services
{
    public class UsuarioService
    {
        private readonly AppDbContext _context;

        public UsuarioService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario> CriarUsuario(UsuarioDto dto)
        {
            dto.Nome = dto.Nome?.Trim();
            dto.Email = dto.Email?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(dto.Nome))
                throw new ArgumentException("Campo 'Nome' é obrigatório.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Campo 'Email' é obrigatório.");

            if (string.IsNullOrWhiteSpace(dto.Senha) || dto.Senha.Length < 6)
                throw new ArgumentException("A senha deve ter ao menos 6 caracteres.");
            
            if (!Enum.IsDefined(typeof(Perfil), dto.Perfil))
                throw new ArgumentException("Perfil do usuário inválido.");

            // Validar email duplicado
            var exists = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email);
            if (exists)
                throw new ArgumentException("Já existe outro usuário com email informado.");

            var senhaHash = GerarHash(dto.Senha);

            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = senhaHash,
                Perfil = dto.Perfil
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        private static string GerarHash(string senha)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(senha);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public async Task<Usuario?> ObterPorId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("O ID do usuário deve ser um número positivo.");

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado.");

            return usuario;
        }

        public async Task<List<Usuario>> ListarUsuarios()
        {
            var usuarios = await _context.Usuarios.ToListAsync();

            if (usuarios == null || usuarios.Count == 0)
                throw new InvalidOperationException("Nenhum usuário encontrado.");

            return usuarios;
        }

        public async Task<Usuario?> AtualizarUsuario(int id, UsuarioDto dto)
        {
            dto.Nome = dto.Nome?.Trim();
            dto.Email = dto.Email?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(dto.Nome))
                throw new ArgumentException("Campo 'Nome' é obrigatório.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Campo 'Email' é obrigatório.");
            
            if (string.IsNullOrWhiteSpace(dto.Senha) || dto.Senha.Length < 6)
                throw new ArgumentException("A senha deve ter ao menos 6 caracteres.");
            
            if (!Enum.IsDefined(typeof(Perfil), dto.Perfil))
                throw new ArgumentException("Perfil do usuário inválido.");
            
            if (id <= 0)
                throw new ArgumentException("O ID do usuário deve ser um número positivo.");
            
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado.");
            
            var emailExistente = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email && u.Id != id);

            if (emailExistente)
                throw new ArgumentException("Já existe outro usuário com email informado.");

            usuario.Nome = dto.Nome;
            usuario.Email = dto.Email;
            usuario.Perfil = dto.Perfil;
            usuario.SenhaHash = GerarHash(dto.Senha);

            await _context.SaveChangesAsync();
            return usuario;
        }

        // JWT
        public string GerarToken(Usuario usuario)
        {
            var key = Encoding.UTF8.GetBytes("4WTH-51ZiC_ra1gU27q2_Tw61YdzxNiloZh5UHfRN82NcE-1wUThESXc7iWasGvPyYrjswNRWEn3PwAuwuv0dA");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Perfil.ToString())
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine($"Token gerado: {jwt}");

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Usuario?> Autenticar(string email, string senha)
        {
            var senhaHash = GerarHash(senha);
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.SenhaHash == senhaHash);
        }
    }
}