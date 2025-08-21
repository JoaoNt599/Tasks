using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Tasks_Backend.Data;
using Tasks_Backend.DTOs;
using Tasks_Backend.Entities;

namespace Tasks_Backend.Services
{
    public class TarefaService
    {
        private readonly AppDbContext _context;

        public TarefaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Tarefa> CriarTarefa(TarefaDto dto, ClaimsPrincipal user)
        {
            // Obtém ID do usuário logado
            var email = user.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            var responsavel = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (responsavel == null)
                throw new Exception("Usuário responsável não encontrado");

            var tarefa = new Tarefa
            {
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                ResponsavelId = responsavel.Id,
                Status = dto.Status,
                DataCriacao = DateTime.UtcNow
            };

            _context.Tarefas.Add(tarefa);
            await _context.SaveChangesAsync();

            return tarefa;
        }

        public async Task<List<TarefaDto>> ListarTarefas()
        {
            var tarafas = await _context.Tarefas
                .Include(t => t.Responsavel)
                .ToListAsync();

            var resultado = tarafas.Select(t => new TarefaDto
            {
                Id = t.Id,
                Titulo = t.Titulo,
                Descricao = t.Descricao,
                ResponsavelId = t.ResponsavelId,
                NomeResponsavel = t.Responsavel.Nome,
                EmailResponsavel = t.Responsavel.Email,
                Status = t.Status,
                DataCriacao = t.DataCriacao
            }).ToList();

            return resultado;
        }

        public async Task<TarefaDto?> BuscarPorId(int id)
        {
            var tarafa = await _context.Tarefas
                .Include(t => t.Responsavel)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tarafa == null)
                return null;

            return new TarefaDto
            {
                Id = tarafa.Id,
                Titulo = tarafa.Titulo,
                Descricao = tarafa.Descricao,
                ResponsavelId = tarafa.ResponsavelId,
                NomeResponsavel = tarafa.Responsavel.Nome,
                EmailResponsavel = tarafa.Responsavel.Email,
                Status = tarafa.Status,
                DataCriacao = tarafa.DataCriacao
            };
        }

        public async Task<TarefaDto?> AtualizarTarefa(int id, TarefaDto dto, ClaimsPrincipal user)
        {
            var email = user.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (usuario == null)
                throw new Exception("Usuário não encontrado");

            var tarafa = await _context.Tarefas.Include(t => t.Responsavel).FirstOrDefaultAsync(t => t.Id == id);
            if (tarafa == null)
                return null;

            // Verifica se o usuário é o responsável ou admin
            var perfil = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var isAdmin = perfil == "0" || perfil == "Administrador";
            var isResponsavel = tarafa.ResponsavelId == usuario.Id;

            if (!isAdmin && !isResponsavel)
                throw new UnauthorizedAccessException("Você não tem permissão para atualizar esta tarefa.");

            tarafa.Titulo = dto.Titulo;
            tarafa.Descricao = dto.Descricao;
            tarafa.Status = dto.Status;

            await _context.SaveChangesAsync();

            return new TarefaDto
            {
                Id = tarafa.Id,
                Titulo = tarafa.Titulo,
                Descricao = tarafa.Descricao,
                ResponsavelId = tarafa.ResponsavelId,
                NomeResponsavel = tarafa.Responsavel.Nome,
                EmailResponsavel = tarafa.Responsavel.Email,
                Status = tarafa.Status,
                DataCriacao = tarafa.DataCriacao
            };
        }

        public async Task<bool> DeletarTarefa(int id, ClaimsPrincipal user)
        {
            var email = user.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (usuario == null)
                throw new Exception("Usuário não encontrado.");

            var tarafa = await _context.Tarefas.FirstOrDefaultAsync(t => t.Id == id);
            if (tarafa == null)
                return false;

            var perfil = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var isAdmin = perfil == "0" || perfil == "Administrador";
            var isResponsavel = tarafa.ResponsavelId == usuario.Id;

            if (!isAdmin && !isResponsavel)
                throw new UnauthorizedAccessException("Você não tem permissão para excluir esta tarefa.");

            _context.Tarefas.Remove(tarafa);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}