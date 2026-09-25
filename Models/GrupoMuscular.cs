namespace EntrenamientoFuerza.Models;

public class GrupoMuscular
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public List<Ejercicio> Ejercicios { get; set; } = [];
}
