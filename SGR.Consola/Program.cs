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
    var responseReclamo = agregarReclamoUseCase.Ejecutar(new AgregarReclamoRequest(););
    var reclamoId = responseReclamo.Id;//no la mato porque se usa mas adelante
    Console.WriteLine("poner algo");

    // 2. agrego las actuaciones
    agregarActuacionUseCase.Ejecutar(new AgregarActuacionRequest(););
    Console.WriteLine("poner algo");
    agregarActuacionUseCase.Ejecutar(new AgregarActuacionRequest(););
    Console.WriteLine("poner algo");
    agregarActuacionUseCase.Ejecutar(new AgregarActuacionRequest(););
    Console.WriteLine("poner algo");


    // 3. Agrego una Observacion y verifico que el estado no cambia
    var resAct4 = agregarActuacionUseCase.Ejecutar( new AgregarActuacionRequest(););
    Console.WriteLine("poner algo");

    // 4. Elimino la ultima actuacion
    eliminarActuacionUseCase.Ejecutar(new EliminarActuacionRequest(););
    Console.WriteLine("poner algo");

    // 5. cambio el estado
    cambiarEstadoReclamoUseCase.Ejecutar(new CambiarEstadoReclamoRequest(););
    Console.WriteLine("poner algo");

    // 6.imprimo el listado 
    var listadoResponse = listarTodosReclamosUseCase.Ejecutar(new ListarTodosReclamosResponse());
    Console.WriteLine("poner algo");
    foreach (var dto in listadoResponse.Reclamos)
    {
        Console.WriteLine("poner algo");
    }
}
catch (DominioException ex)
{
    Console.WriteLine("mensaje para dominio");
}
catch (EntidadNoEncontradaException ex){
    Console.WriteLine("mensaje para la entidad");
}
catch (AutorizacionException ex)
{
    Console.WriteLine("mensaje para la autorizacion");
}
catch(AplicacionExeption){
    Console.WriteLine("mensaje para la aplicacion");
}
catch (Exception ex)
{
    Console.WriteLine("mensaje para Excepciones de .net");
}

// cammino malo dominio

try {
    var responseReclamo = agregarReclamoUseCase.Ejecutar(new AgregarReclamoRequest());
}
catch (DominioException ex)
{
    Console.WriteLine("mensaje para dominio");
}
catch (EntidadNoEncontradaException ex){
    Console.WriteLine("mensaje para la entidad");
}
catch (AutorizacionException ex)
{
    Console.WriteLine("mensaje para la autorizacion");
}
catch(AplicacionExeption){
    Console.WriteLine("mensaje para la aplicacion");
}
catch (Exception ex)
{
    Console.WriteLine("mensaje para Excepciones de .net");
}


// cammino malo entidad
try {
    // codigo 
}
catch (DominioException ex)
{
    Console.WriteLine("mensaje para dominio");
}
catch (EntidadNoEncontradaException ex){
    Console.WriteLine("mensaje para la entidad");
}
catch (AutorizacionException ex)
{
    Console.WriteLine("mensaje para la autorizacion");
}
catch(AplicacionExeption){
    Console.WriteLine("mensaje para la aplicacion");
}
catch (Exception ex)
{
    Console.WriteLine("mensaje para Excepciones de .net");
}


// cammino malo autorizacion
try {
    // codigo 
}
catch (DominioException ex)
{
    Console.WriteLine("mensaje para dominio");
}
catch (EntidadNoEncontradaException ex){
    Console.WriteLine("mensaje para la entidad");
}
catch (AutorizacionException ex)
{
    Console.WriteLine("mensaje para la autorizacion");
}
catch(AplicacionExeption){
    Console.WriteLine("mensaje para la aplicacion");
}
catch (Exception ex)
{
    Console.WriteLine("mensaje para Excepciones de .net");
}

// cammino malo aplicacion
try {
    // codigo 
}
catch (DominioException ex)
{
    Console.WriteLine("mensaje para dominio");
}
catch (EntidadNoEncontradaException ex){
    Console.WriteLine("mensaje para la entidad");
}
catch (AutorizacionException ex)
{
    Console.WriteLine("mensaje para la autorizacion");
}
catch(AplicacionExeption){
    Console.WriteLine("mensaje para la aplicacion");
}
catch (Exception ex)
{
    Console.WriteLine("mensaje para Excepciones de .net");
}