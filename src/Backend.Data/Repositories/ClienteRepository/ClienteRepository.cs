using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Domain.Entities.Client;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories.ClienteRepository
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ApplicationDbContext _context;

        public ClienteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CrearClienteAsync(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task ModificarCliente(Cliente cliente)
        {
            _context.Update(cliente);
            await _context.SaveChangesAsync();
        }
        public Task<bool> ExisteClienteById(int id)
        {
            return _context.Clientes.AnyAsync(c => c.Id == id);
        }

        public Task<bool> ExisteCuilAsync(string cuil)
        {
            return _context.Clientes.AnyAsync(c => c.Cuil == cuil);
        }

        public Task<Cliente> GetCliente(int id)
        {
            return _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
        }

        public Task<Cliente> GetClienteConHistorial(int id)
        {
            return _context.Clientes
                    .AsNoTracking()
                    .Include(c => c.HistorialesScoring)
                        .ThenInclude(h => h.Ajustes)
                    .Include(c => c.HistorialesScoring)
                        .ThenInclude(h => h.Alertas)
                    .FirstOrDefaultAsync(c => c.Id == id);
        }

        public IQueryable<Cliente> GetClientesQueryable(bool estaActivo, string? searchTerm)
        {
            var query = _context.Clientes
                        .AsNoTracking()
                        .Include(c => c.HistorialesScoring)
                        .Where(c => c.EstaActivo == estaActivo)
                        .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {

                var queryLower = searchTerm.Trim().ToLower();
                query = query.Where(c =>
                    c.Nombre.ToLower().Contains(queryLower) ||
                    c.Apellido.ToLower().Contains(queryLower) ||
                    c.Cuil.Contains(searchTerm.Trim()) ||
                    (c.Nombre.ToLower() + " " + c.Apellido.ToLower()).Contains(queryLower) ||
                    (c.Apellido.ToLower() + " " + c.Nombre.ToLower()).Contains(queryLower)
                );
            }

            return query.OrderBy(c => c.Apellido)
                        .ThenBy(c => c.Nombre);

        }
    }
}