namespace EntrenamientoFuerza.Models;

// Una entrada de la rutina: un ejercicio en un orden concreto, con su forma de medirse
// (repeticiones O duracion, nunca ambas) y su descanso posterior.
public class RutinaEjercicio
{
    public int Id { get; set; }
    public int RutinaId { get; set; }
    public int EjercicioId { get; set; }
    public int Orden { get; set; }
    public int Series { get; set; }
    public int? Repeticiones { get; set; }
    public int? DuracionSegundos { get; set; }
    public int DescansoSegundos { get; set; }

    public Rutina? Rutina { get; set; }
    public Ejercicio? Ejercicio { get; set; }
}
