namespace EntrenamientoFuerza.Models;

public class Ejercicio
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? VideoUrl { get; set; }
    public int? GrupoMuscularId { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; }

    public GrupoMuscular? GrupoMuscular { get; set; }
    public List<RutinaEjercicio> RutinaEjercicios { get; set; } = [];
    public List<SesionEjercicio> SesionEjercicios { get; set; } = [];
}
