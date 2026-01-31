using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Domain.Entities.Client;

namespace Backend.Data.Repositories.ClienteRepository
{
    public interface IClienteRepository
    {
        Task CrearClienteAsync(Cliente cliente);
        Task ModificarCliente(Cliente cliente);
        Task<Cliente> GetCliente(int id);
        Task<Cliente> GetClienteConHistorial(int id);
        IQueryable<Cliente> GetClientesQueryable(bool estaActivo, string? searchTerm);
        Task<bool> ExisteCuilAsync(string cuil);
        Task<bool> ExisteClienteById(int id);
    }
}