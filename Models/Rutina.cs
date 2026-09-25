namespace EntrenamientoFuerza.Models;

public class Rutina
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int CreadoPorId { get; set; }
    public bool Activa { get; set; } = true;
    public DateTime FechaCreacion { get; set; }

    public Usuario? CreadoPor { get; set; }
    public List<RutinaEjercicio> RutinaEjercicios { get; set; } = [];
    public List<SesionDiario> Sesiones { get; set; } = [];
}
