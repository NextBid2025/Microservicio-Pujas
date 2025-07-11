
namespace Puja.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<bool> UsuarioExisteAsync(string userId);
    }
}