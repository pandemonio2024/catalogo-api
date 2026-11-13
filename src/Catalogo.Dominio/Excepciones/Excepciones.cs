namespace Catalogo.Dominio.Excepciones;

/// <summary>Se rompió una regla de negocio (se traduce a HTTP 400).</summary>
public class ReglaDeNegocioException(string mensaje) : Exception(mensaje);

/// <summary>El recurso pedido no existe (se traduce a HTTP 404).</summary>
public class NoEncontradoException(string entidad, int id)
    : Exception($"{entidad} con id {id} no existe.");
