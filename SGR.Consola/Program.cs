using SGR.Dominio;
using SGR.Aplicacion;
using SGR.Infraestructura;

IActuacionRepository actuacionRepository= new ActuacionTxtRepository();
IReclamoRepository reclamoRepository= new ReclamoTxtRepository();
IAutorizacionService autorizacionProvisionalService = new AutorizacionProvisionalService ();

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
    var requestReclamo = new AgregarReclamoRequest();
    var responseReclamo = agregarReclamoUseCase.Ejecutar(requestReclamo);
    var reclamoId = responseReclamo.Id;
    Console.WriteLine("poner algo");

    // 2. agrego las actuaciones
    var reqAct1 = new AgregarActuacionRequest();
    var reqAct2 = new AgregarActuacionRequest();
    var reqAct3 = new AgregarActuacionRequest();
    agregarActuacionUseCase.Ejecutar(reqAct1);
    Console.WriteLine("poner algo");
    agregarActuacionUseCase.Ejecutar(reqAct2);
    Console.WriteLine("poner algo");
    agregarActuacionUseCase.Ejecutar(reqAct3);
    Console.WriteLine("poner algo");


    // 3. Agrego una Observacion y verifico que el estado no cambia
    var reqAct4 = new AgregarActuacionRequest();
    var resAct4 = agregarActuacionUseCase.Ejecutar(reqAct4);
    Console.WriteLine("poner algo");

    // 4. Elimino la ultima actuacion
    var reqEliminarAct = new EliminarActuacionRequest();
    eliminarActuacionUseCase.Ejecutar(reqEliminarAct);
    Console.WriteLine("poner algo");

    // 5. cambio el estado
    var reqCambioEstado = new CambiarEstadoReclamoRequest();
    cambiarEstadoReclamoUseCase.Ejecutar(reqCambioEstado);
    Console.WriteLine("poner algo");

    // 6.imprimir el listado 
    var listadoResponse = listarTodosReclamosUseCase.Ejecutar(new ListarTodosReclamosResponse());
    Console.WriteLine("poner algo");
    foreach (var dto in listadoResponse.Reclamos)
    {
        Console.WriteLine("poner algo");
    }
}
catch (DominioException ex)
{
    Console.WriteLine("poner algo");
}
catch (AutorizacionException ex)
{
    Console.WriteLine("poner algo");
}
catch (Exception ex)
{
    Console.WriteLine("poner algo");
}

// cammino malo
