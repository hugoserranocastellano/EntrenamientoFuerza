namespace EntrenamientoFuerza.Models;

// Un ejercicio dentro de una sesion real, con el detalle de lo hecho serie a serie.
public class SesionEjercicio
{
    public int Id { get; set; }
    public int SesionDiarioId { get; set; }
    public int EjercicioId { get; set; }
    public int Orden { get; set; }
    public string? Notas { get; set; }

    public SesionDiario? SesionDiario { get; set; }
    public Ejercicio? Ejercicio { get; set; }
    public List<SesionSerie> Series { get; set; } = [];
}
