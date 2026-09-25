namespace EntrenamientoFuerza.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Rol { get; set; } = "Usuario";
    public bool Activo { get; set; } = true;
    public DateTime FechaAlta { get; set; }

    public List<Rutina> Rutinas { get; set; } = [];
    public List<SesionDiario> Sesiones { get; set; } = [];
}
