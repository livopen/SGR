using SGR.Dominio;
using SGR.Aplicacion;
using SGR.Infraestructura;

// 1. Repositorios y Servicios de Infraestructura
IActuacionRepository actuacionRepository = new ActuacionTxtRepository();
IReclamoRepository reclamoRepository = new ReclamoTxtRepository();
IAutorizacionService autorizacionProvisionalService = new AutorizacionProvisionalService();

// 2. Servicio de Aplicación para recálculo de estado de reclamos
var actualizacionEstadoReclamoService = new ActualizacionEstadoReclamoService(reclamoRepository, actuacionRepository);

// 3. Casos de Uso de Reclamos
var agregarReclamoUseCase = new AgregarReclamoUseCase(reclamoRepository, autorizacionProvisionalService);
var cambiarEstadoReclamoUseCase = new CambiarEstadoReclamoUseCase(reclamoRepository, autorizacionProvisionalService);
var eliminarReclamoUseCase = new EliminarReclamoUseCase(reclamoRepository, autorizacionProvisionalService, actuacionRepository);
var listarTodosReclamosUseCase = new ListarTodosReclamosUseCase(reclamoRepository); 
var modificarAsuntoReclamoUseCase = new ModificarAsuntoReclamoUseCase(reclamoRepository, autorizacionProvisionalService);
var obtenerReclamoPorIdUseCase = new ObtenerReclamoPorIdUseCase(reclamoRepository, actuacionRepository); 

// 4. Casos de Uso de Actuaciones
var agregarActuacionUseCase = new AgregarActuacionUseCase(actuacionRepository, reclamoRepository, autorizacionProvisionalService, actualizacionEstadoReclamoService);
var eliminarActuacionUseCase = new EliminarActuacionUseCase(actuacionRepository, reclamoRepository, autorizacionProvisionalService, actualizacionEstadoReclamoService);
var obtenerActuacionPorIdUseCase = new ObtenerActuacionPorIdUseCase(actuacionRepository);
var obtenerActuacionesPorIdReclamoUseCase = new ObtenerActuacionesPorIdReclamoUseCase(actuacionRepository);

// esta no fue creada todavia 
var modificarActuacionUseCase = new ModificarActuacionUseCase(actuacionRepository, reclamoRepository, autorizacionProvisionalService, actualizacionEstadoReclamoService);

// solo van a ser esqueletos, se vva a modificar posteriormente 

// camino feliz
try
{
    // 1. cereo el reclamo y me lo guardo
    var responseReclamo = agregarReclamoUseCase.Ejecutar(new AgregarReclamoRequest("arbol caido","34A", new DateTime(2026,08,25)));
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
      // seguro esta mal, no se bien con que cu deberia ser 
    var responseActuacion = agregarActuacionUseCase.Ejecutar(new AgregarActuacionRequest(/*datos*/));
    var responseReclamoEntero = ObtenerReclamoPorIdUseCase.Ejecutar(reclamoId);
    Console.WriteLine($"se agrego correctamente la observacion y su estado es {requesReclamoEntero.Estado}"); // en este mepa que no era asi el print

    // 4. Elimino la ultima actuacion
    var ultAct = ObtenerActuacionesPorIdReclamoUseCase.Ejecutar(reclamoId); // me da lista
    var idActuacion = altAct[altAct.cont-1].IdActuacion;
    eliminarActuacionUseCase.Ejecutar(idActuacion);// sera asi?
    Console.WriteLine($"se elimino correctamente la ultima actuacion con id: {idActuacion}");

    // 5. cambio el estado
    // tengo dudas con el parametro del estado
    cambiarEstadoReclamoUseCase.Ejecutar(new CambiarEstadoExpedienteRequest(requestReclamo.IdUsuario,reclamoId, "EnEjecucion")); 
    Console.WriteLine($"se pudo cambiar correctamente el estado del reclamo a: {requesReclamoEntero.Estado}");

    // 6.imprimo el listado 
    var listadoResponse = listarTodosReclamosUseCase.Ejecutar(new ListarTodosReclamosRequest());
    Console.WriteLine(" los reclamos que se hicieron son:");
    foreach (var dto in listadoResponse.Reclamos)
    {
        Console.WriteLine($"el reclamo: {dto}" ); // dudo si es asi, la idea es llamar el tostring 
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

// cammino malo agregar una actuación a un reclamo inexistente
try {                                           
    var requestActuacionHuerfana = new AgregarActuacionRequest("3ab",Estado.Recibido,"luminaria rota","724a");

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