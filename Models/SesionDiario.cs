namespace EntrenamientoFuerza.Models;

// Una sesion de entrenamiento real, hecha un dia concreto por un usuario.
public class SesionDiario
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int? RutinaId { get; set; }
    public DateOnly Fecha { get; set; }
    public string? Notas { get; set; }
    public DateTime FechaCreacion { get; set; }

    public Usuario? Usuario { get; set; }
    public Rutina? Rutina { get; set; }
    public List<SesionEjercicio> SesionEjercicios { get; set; } = [];
}
