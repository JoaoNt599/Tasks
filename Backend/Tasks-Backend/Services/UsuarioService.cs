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
            // Valida email duplicado
            var exists = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email);
            if (exists)
                return null; // TODO: Criar exceção personalizada

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
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<List<Usuario>> ListarUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario?> AtualizarUsuario(int id, UsuarioDto dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return null;

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