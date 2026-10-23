using SGR.Dominio.Comun;
using SGR.Aplicacion.Comun;
using SGR.Aplicacion.Autorizacion;
using SGR.Infraestructura.Actuaciones;
using SGR.Infraestructura.Reclamos;
using SGR.Infraestructura.Autorizacion;
using SGR.Aplicacion.Reclamos;
using SGR.Aplicacion.Actuaciones;

Console.WriteLine("==================================================");
Console.WriteLine(" SISTEMA DE GESTIÓN DE RECLAMOS (SGR) - PRUEBAS");
Console.WriteLine("==================================================\n");

// ==========================================
// 1. COMPOSITION ROOT (Configuración DI)
// ==========================================
// Instanciamos los repositorios y servicios de Infraestructura
var actuacionRepository = new ActuacionTxtRepository();
var reclamoRepository = new ReclamoTxtRepository();
var autorizacionService = new AutorizacionProvisionalService(); 
var actualizacionEstadoService = new ActualizacionEstadoReclamoService(reclamoRepository, actuacionRepository);

// Instanciamos los Casos de Uso (Aplicación) pasándoles las dependencias
var agregarReclamoUseCase = new AgregarReclamoUseCase(reclamoRepository, autorizacionService);
var agregarActuacionUseCase = new AgregarActuacionUseCase(actuacionRepository, reclamoRepository, autorizacionService, actualizacionEstadoService);
var eliminarActuacionUseCase = new EliminarActuacionUseCase(actuacionRepository, reclamoRepository, autorizacionService, actualizacionEstadoService);
var listarTodosReclamosUseCase = new ListarTodosReclamosUseCase(reclamoRepository);
// Nota: Se asume que estos Casos de Uso tienen los constructores definidos o se ajustarán en el código final.
var cambiarEstadoReclamoUseCase = new CambiarEstadoReclamoUseCase(reclamoRepository, autorizacionService); 
var obtenerReclamoPorIdUseCase = new ObtenerReclamoPorIdUseCase(reclamoRepository, actuacionRepository);

// Variables globales para la simulación
string idUsuarioSimulado = Guid.NewGuid().ToString();
string reclamoIdGenerado = string.Empty;
string ultimaActuacionId = string.Empty;

// ==========================================
// 2. CAMINO FELIZ (Flujo de éxito)
// ==========================================
Console.WriteLine(">>> INICIANDO PRUEBAS: CAMINO FELIZ <<<\n");

try
{
    // A. Crear un Reclamo
    Console.WriteLine("A. Creando un nuevo reclamo...");
    var requestReclamo = new AgregarReclamoRequest("Bache peligroso en la avenida", idUsuarioSimulado);
    var responseReclamo = agregarReclamoUseCase.Ejecutar(requestReclamo);
    reclamoIdGenerado = responseReclamo.IdReclamo;
    Console.WriteLine($"[ÉXITO] Reclamo creado con ID: {reclamoIdGenerado}\n");

    // B. Agregar actuaciones que cambian el estado (Inspeccion -> OrdenDeTrabajo -> TrabajoRealizado)
    Console.WriteLine("B. Agregando actuaciones que mutan el estado...");
    
    var reqInspeccion = new AgregarActuacionRequest(reclamoIdGenerado, "Inspeccion", "Se verificó el bache", idUsuarioSimulado);
    agregarActuacionUseCase.Ejecutar(reqInspeccion);
    Console.WriteLine($"[ÉXITO] Actuación 'Inspección' agregada. Estado esperado: EnInspeccion.");

    var reqOrden = new AgregarActuacionRequest(reclamoIdGenerado, "OrdenDeTrabajo", "Se solicitó asfalto", idUsuarioSimulado);
    agregarActuacionUseCase.Ejecutar(reqOrden);
    Console.WriteLine($"[ÉXITO] Actuación 'OrdenDeTrabajo' agregada. Estado esperado: EnEjecucion.");

    var reqTrabajo = new AgregarActuacionRequest(reclamoIdGenerado, "TrabajoRealizado", "Bache tapado", idUsuarioSimulado);
    var resTrabajo = agregarActuacionUseCase.Ejecutar(reqTrabajo);
    ultimaActuacionId = resTrabajo.IdReclamo; // El response devuelve el Id de la Actuacion generada
    Console.WriteLine($"[ÉXITO] Actuación 'TrabajoRealizado' agregada. Estado esperado: Resuelto.\n");

    // C. Agregar una Observacion (no debería cambiar el estado)
    Console.WriteLine("C. Agregando una Observación...");
    var reqObservacion = new AgregarActuacionRequest(reclamoIdGenerado, "Observacion", "Los vecinos agradecen", idUsuarioSimulado);
    var resObservacion = agregarActuacionUseCase.Ejecutar(reqObservacion);
    Console.WriteLine($"[ÉXITO] Observación agregada con ID: {resObservacion.IdReclamo}. El estado debe seguir siendo Resuelto.\n");

    // D. Eliminar la última actuación y verificar recalculo
    Console.WriteLine("D. Eliminando la observación para verificar recálculo de estado...");
    var reqEliminar = new EliminarActuacionRequest(resObservacion.IdReclamo, idUsuarioSimulado);
    eliminarActuacionUseCase.Ejecutar(reqEliminar);
    Console.WriteLine($"[ÉXITO] Observación eliminada. El estado debe mantenerse consistente.\n");

    // E. Cambiar el estado manualmente
    Console.WriteLine("E. Cambiando el estado manualmente a 'Cerrado'...");
    var reqCambioEstado = new CambiarEstadoReclamoRequest(idUsuarioSimulado, reclamoIdGenerado, "Cerrado");
    cambiarEstadoReclamoUseCase.Ejecutar(reqCambioEstado);
    Console.WriteLine($"[ÉXITO] Estado cambiado correctamente a 'Cerrado'.\n");

    // F. Listar todos los reclamos
    Console.WriteLine("F. Listando los reclamos registrados...");
    var listaReclamos = listarTodosReclamosUseCase.Ejecutar(new ListarTodosReclamosRequest());
    foreach (var reclamo in listaReclamos.lista)
    {
        Console.WriteLine($" -> ID: {reclamo.Id} | Asunto: {reclamo.Asunto} | Estado: {reclamo.Estado}");
    }
    Console.WriteLine();
}
catch (DominioException ex) { Console.WriteLine($"[ERROR DE DOMINIO] {ex.Message}"); }
catch (EntidadNoEncontradaException ex) { Console.WriteLine($"[ERROR DE ENTIDAD] {ex.Message}"); }
catch (AutorizacionException ex) { Console.WriteLine($"[ERROR DE PERMISOS] {ex.Message}"); }
catch (AplicacionException ex) { Console.WriteLine($"[ERROR DE APLICACIÓN] {ex.Message}"); }
catch (Exception ex) { Console.WriteLine($"[ERROR GENERAL] {ex.Message}"); }

// ==========================================
// 3. CAMINOS DE ERROR (Casos alternativos)
// ==========================================
Console.WriteLine(">>> INICIANDO PRUEBAS: CAMINOS DE ERROR <<<\n");

// Error 1: Intentar crear reclamo con asunto vacío
try
{
    Console.WriteLine("Prueba Error 1: Crear reclamo con asunto vacío...");
    var requestReclamoMalo = new AgregarReclamoRequest("", idUsuarioSimulado);
    agregarReclamoUseCase.Ejecutar(requestReclamoMalo);
}
catch (DominioException ex) { Console.WriteLine($"[ERROR ESPERADO - DOMINIO] {ex.Message}\n"); }
catch (Exception ex) { Console.WriteLine($"[FALLO INESPERADO] {ex.Message}\n"); }

// Error 2: Agregar actuación a un reclamo inexistente
try
{
    Console.WriteLine("Prueba Error 2: Agregar actuación a un reclamo inexistente...");
    string idFalso = Guid.NewGuid().ToString();
    var requestActuacionHuerfana = new AgregarActuacionRequest(idFalso, "Observacion", "Detalle x", idUsuarioSimulado);
    agregarActuacionUseCase.Ejecutar(requestActuacionHuerfana);
}
catch (EntidadNoEncontradaException ex) { Console.WriteLine($"[ERROR ESPERADO - ENTIDAD] {ex.Message}\n"); }
catch (Exception ex) { Console.WriteLine($"[FALLO INESPERADO] {ex.Message}\n"); }

// Error 3: Simular rechazo de permisos (Autorización)
try
{
    Console.WriteLine("Prueba Error 3: Intentar acción sin permisos...");
    
    // Creamos un servicio mockeado "al vuelo" que siempre retorna false
    IAutorizacionService servicioDenegado = new AutorizacionProvisionalDenegada();
    var agregarReclamoSinPermisos = new AgregarReclamoUseCase(reclamoRepository, servicioDenegado);
    
    var requestSinPermiso = new AgregarReclamoRequest("Asunto válido", idUsuarioSimulado);
    agregarReclamoSinPermisos.Ejecutar(requestSinPermiso);
}
catch (AutorizacionException ex) { Console.WriteLine($"[ERROR ESPERADO - PERMISOS] {ex.Message}\n"); }
catch (Exception ex) { Console.WriteLine($"[FALLO INESPERADO] {ex.Message}\n"); }

Console.WriteLine("==================================================");
Console.WriteLine(" PRUEBAS FINALIZADAS");
Console.WriteLine("==================================================");

// Clase auxiliar para la Prueba de Error 3 (Servicio que siempre deniega)
public class AutorizacionProvisionalDenegada : IAutorizacionService
{
    public bool PoseeElPermiso(Guid idUsuario, string permiso) => false;
}