using Contratos.DTOs;

namespace examen1.Interfaces
{
    public interface IAuthInterface
    {
        void Registrar(LoginDto dto);
        LoginRespuestaDTO? Login(LoginDto dto);
    }
}