using SGR.Dominio;
using SGR.Aplicacion;
using SGR.Infraestructura;

IActuacionRepository actuacionRepository= new ActuacionTxtRepository();
IReclamoRepository reclamoRepository= new ReclamoTxtRepository();
IAutorizacionService autorizacionProvisionalService = new AutorizacionProvisionalService ();

// probablemente todos tengan una interfaz de comportamiento
// si ese es el caso debo instanciar con un array que guarde interfaz

var actualizacionEstadoReclamoService = new ActualizacionEstadoReclamoService (); // no tiene codigo

var agregarActuacionUseCase = new AgregarActuacionUseCase(); // no tiene codigo
var eliminarActuacionUseCase = new EliminarActuacionUseCase(); // no tiene codigo
var obtenerActuacionPorIdUseCase = new ObtenerActuacionPorIdUseCase(); // no tiene codigo
var obtenerActuacionesPorIdReclamoUseCase = new ObtenerActuacionesPorIdReclamoUseCase(); // no tiene codigo

// el use case que falta
var modificarActuacionUseCase = new ModificarActuacionUseCase(); // no existe 


// este tiene el constructor definido
var agregarReclamoUseCase = new AgregarReclamoUseCase(reclamoRepository, autorizacionProvisionalService );

// estos no tiene constructor definido
var cambiarEstadoReclamoUseCase = new CambiarEstadoReclamoUseCase(reclamoRepository, autorizacionProvisionalService);
var eliminarReclamoUseCase = new EliminarReclamoUseCase(reclamoRepository, autorizacionProvisionalService, actuacionRepository);
var listarTodosReclamosUseCase = new ListarTodosReclamosUseCase(reclamoRepository); 
var modificarAsuntoReclamo = new ModificarAsuntoReclamo(reclamoRepository, autorizacionProvisionalService);
var obtenerReclamoPorIdUseCase = new ObtenerReclamoPorIdUseCase(reclamoRepository,actuacionRepository); 


// solo van a ser esqueletos, se vva a modificar posteriormente 

// camino feliz
try
{
    // 1. cereo el reclamo y me lo guardo
    var requestReclamo = new AgregarReclamoRequest("arbol caido","34A", new DateTime(2026,08,25));
    var responseReclamo = agregarReclamoUseCase.Ejecutar(requestReclamo);
    var reclamoId = responseReclamo.IdReclamo;
    Console.WriteLine($"se guardo correctamente el reclamo, el id del reclamo es {reclamoId}");

    // 2. agrego las actuaciones
    var requestAgActuacion1 = new AgregarActuacionRequest(/*datos*/)
    agregarActuacionUseCase.Ejecutar(requestActuacion1);
    Console.WriteLine($"se agrego correctamente la actuacion........");

    var requestAgActuacion2 = new AgregarActuacionRequest(/*datos*/);
    agregarActuacionUseCase.Ejecutar(requestActuacion2);
    Console.WriteLine($"se agrego correctamente la actuacion.........");

    var requestAgActuacion3 = new AgregarActuacionRequest(/*datos*/);
    agregarActuacionUseCase.Ejecutar(requestActuacion3);
    Console.WriteLine($"se agrego correctamente la actuacion.......");


    // 3. Agrego una Observacion y verifico que el estado no cambia
      // cambiar nombre  
      // puede estar mal 
    var requestAgActuacion =  new AgregarActuacionRequest(/*datos*/);
    var responseActuacion = agregarActuacionUseCase.Ejecutar(requestActuacion);
    Console.WriteLine($"se agrego correctamente la observacion........."); // en este mepa que no era asi el print

    // 4. Elimino la ultima actuacion
    eliminarActuacionUseCase.Ejecutar(ObtenerActuacionPorIdUseCase.Ejecutar(/*request de algo.id*/));
    Console.WriteLine($"se elimino correctamente la ultima actuacion.........");

    // 5. cambio el estado
    cambiarEstadoReclamoUseCase.Ejecutar(new CambiarEstadoExpedienteRequest(requestReclamo.IdUsuario,reclamoId, "en proceso"));
    Console.WriteLine("se pudo cambiar correctamente el estado del reclamo");

    // 6.imprimo el listado 
    var listadoResponse = listarTodosReclamosUseCase.Ejecutar(new ListarTodosReclamosRequest());
    Console.WriteLine(" los reclamos que se hicieron son:");
    foreach (var dto in listadoResponse.Reclamos)
    {
        Console.WriteLine("poner algo"); // no se que poner 
    }
}
catch (DominioException ex)
{
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}
catch (EntidadNoEncontradaException ex){
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}
catch (AutorizacionException ex)
{
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}
catch(AplicacionExeption){
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}
catch (Exception ex)
{
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}

// cammino malo reclamo con asunto vacío

try {
    var requestReclamoMalo = new AgregarReclamoRequest("","34A", new DateTime(2026,08,25));
    var responseReclamoMalo = agregarReclamoUseCase.Ejecutar(requestReclamoMalo);
}
catch (DominioException ex)
{
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}
catch (EntidadNoEncontradaException ex){
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}
catch (AutorizacionException ex)
{
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}
catch(AplicacionExeption){
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}
catch (Exception ex)
{
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}

/*

Se debe probar el "Camino Feliz": crear reclamos, agregarles actuaciones que cambien su estado (por ejemplo 
Inspeccion → OrdenDeTrabajo → TrabajoRealizado), agregar una Observacion y verificar que el estado no cambia,
eliminar la última actuación y verificar que el estado se recalcula, cambiar el estado manualmente y listar.

Se deben probar los "Caminos de Error": intentar crear un reclamo con asunto vacío o de más de 200 caracteres
para ver cómo actúa el Dominio, agregar una actuación a un reclamo inexistente, y cambiar temporalmente el 
retorno del AutorizacionProvisionalService a false para corroborar que se lanza la AutorizacionException.

Utilizar bloques try/catch para capturar y mostrar mensajes amigables al usuario, diferenciando 
DominioException, AutorizacionException y errores generales.


*/

// cammino malo agregar una actuación a un reclamo inexistente
try {                                           
    var requestActuacionHuerfana = new AgregarActuacionRequest(
        //ReclamoId 
        //Tipo: TipoActuacion Observacion
        //Detalle
        //IdUsuario
    );

    agregarActuacionUseCase.Ejecutar(requestActuacionHuerfana); 
}
catch (DominioException ex)
{
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}
catch (EntidadNoEncontradaException ex){
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}
catch (AutorizacionException ex)
{
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}
catch(AplicacionExeption){
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}
catch (Exception ex)
{
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}


// cammino malo retorno del AutorizacionProvisionalService false
try {

    IAutorizacionService servicioDenegado = new AutorizacionProvisionalService(permitirTodo: false);
    var agregarReclamoSinPermisosUseCase = new AgregarReclamoUseCase(reclamoRepository, servicioDenegado);
}
catch (DominioException ex)
{
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}
catch (EntidadNoEncontradaException ex){
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}
catch (AutorizacionException ex)
{
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}
catch(AplicacionExeption){
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}
catch (Exception ex)
{
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}

// cammino malo aplicacion, no se si hay que hacer este try-catch
try {
    // codigo
}
catch (DominioException ex)
{
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}
catch (EntidadNoEncontradaException ex){
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}
catch (AutorizacionException ex)
{
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}
catch(AplicacionExeption){
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}
catch (Exception ex)
{
    Console.WriteLine($"lamentamos las molestias, se detecto un error en el sistema: {ex.message}\n");
}