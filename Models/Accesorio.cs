namespace EntrenamientoFuerza.Models;

public class Accesorio
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? ImagenUrl { get; set; }

    public List<Ejercicio> Ejercicios { get; set; } = [];
}
