namespace EntrenamientoFuerza.Models;

// Registro real de una serie hecha: lo que realmente se completo, no lo planificado.
public class SesionSerie
{
    public int Id { get; set; }
    public int SesionEjercicioId { get; set; }
    public int NumeroSerie { get; set; }
    public int? RepeticionesRealizadas { get; set; }
    public int? DuracionSegundosReal { get; set; }
    public decimal? PesoKg { get; set; }
    public bool Completada { get; set; }

    public SesionEjercicio? SesionEjercicio { get; set; }
}
