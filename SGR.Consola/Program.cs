/*

El proyecto ejecutable actuará como nuestra interfaz de usuario y como Composition Root.

En el Program.cs se deberán instanciar las implementaciones de los repositorios y servicios (Infraestructura)
y pasárselas por constructor a los Casos de Uso (Aplicación).

Simular un flujo de ejecución interactuando con los Casos de Uso mediante la creación de DTOs.

Se debe probar el "Camino Feliz": crear reclamos, agregarles actuaciones que cambien su estado 
(por ejemplo Inspeccion → OrdenDeTrabajo → TrabajoRealizado), agregar una Observacion y verificar que el 
estado no cambia, eliminar la última actuación y verificar que el estado se recalcula, cambiar el estado 
manualmente y listar.

Se deben probar los "Caminos de Error": intentar crear un reclamo con asunto vacío o de más de 200 caracteres 
para ver cómo actúa el Dominio, agregar una actuación a un reclamo inexistente, y cambiar temporalmente el 
retorno del AutorizacionProvisionalService a false para corroborar que se lanza la AutorizacionException.

Utilizar bloques try/catch para capturar y mostrar mensajes amigables al usuario, diferenciando 
DominioException, AutorizacionException y errores generales.


*/

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


// camino feliz
try
catch 

// cammino malo
