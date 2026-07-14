using Domain.Enums;

namespace Domain.Entities;

public class MateriaEstadoGuardado
{
    public string NombreMateria { get; set; }
    public EstadoMateria Estado { get; set; }
}