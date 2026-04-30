using FoodDeliveryAPI.Context;
using FoodDeliveryAPI.Domains.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryAPI.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly FoodDeliveryAPIContext _context;
        private readonly ILogger<UsuarioRepository> _logger;

        public UsuarioRepository(FoodDeliveryAPIContext context, ILogger<UsuarioRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Usuario> AddAsync(Usuario usuario)
        {
          var buscaUsuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == usuario.Email);
          if (buscaUsuario != null)
          {
              _logger.LogWarning("Usuário com o email {Email} já existe.", usuario.Email);
              throw new InvalidOperationException($"Usuário com o email {usuario.Email} já existe.");
          }

          _context.Usuarios.Add(usuario);
          await _context.SaveChangesAsync();
            _logger.LogInformation("Usuário com o email {Email} adicionado com sucesso.", usuario.Email);
            return usuario;
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            if(email == null)
            {
                _logger.LogWarning("Email fornecido é nulo.");
               throw new ArgumentNullException(nameof(email), "Email fornecido é nulo.");
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null)
            {
                _logger.LogInformation("Nenhum usuário encontrado com o email: {Email}", email);
                throw new KeyNotFoundException($"Nenhum usuário encontrado com o email: {email}");
            }


            _logger.LogInformation("Usuário encontrado com o email: {Email}", email);

            return usuario;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var buscaUsuario = await _context.Usuarios.FindAsync(id);
            if (buscaUsuario == null)
            {
                _logger.LogWarning("Usuário com o id {Id} não encontrado para exclusão.", id);
                throw new KeyNotFoundException($"Usuário com o id {id} não encontrado para exclusão.");
            }

            _context.Usuarios.Remove(buscaUsuario);
            _logger.LogInformation("Usuário com o id {Id} removido com sucesso.", id); return true;
        }
    }
}
