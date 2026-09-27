namespace EntrenamientoFuerza.Models;

public class Accesorio
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public List<Ejercicio> Ejercicios { get; set; } = [];
}
