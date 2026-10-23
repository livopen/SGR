namespace SGR.Aplicacion.Actuaciones;
using System;
using System.Linq;
using SGR.Aplicacion.Autorizacion;
using SGR.Aplicacion.Comun;
using SGR.Aplicacion.Reclamos;
using SGR.Dominio.Actuaciones;

public class ModificarActuacionUseCase{
    private IActuacionRepository _actuacionRepo;
    private IReclamoRepository _reclamoRepo; 
    private ActualizacionEstadoReclamoService _estadoService;
    private IAutorizacionService _autorizacion;
    public ModificarActuacionUseCase(IActuacionRepository actuacionRepository, IReclamoRepository reclamoRepository, ActualizacionEstadoReclamoService estadoService,
        IAutorizacionService autorizacionService)
    {
        _actuacionRepo = actuacionRepository;
        _reclamoRepo = reclamoRepository;
        _estadoService = estadoService;
        _autorizacion = autorizacionService;

    }
    public ModificarActuacionResponse Ejecutar(ModificarActuacionRequest request)
    {
        //oarseo id a guid
        Guid idActuacion = Guid.Parse(request.idActuacion);
        Guid idUsuario = Guid.Parse(request.idUsuario);

        if (!_autorizacion.PoseeElPermiso(idUsuario, Permiso.ActuacionModificacion.ToString()))
            throw new AutorizacionException("El usuario no tiene permisos.");

        var actuacion = _actuacionRepo.ObtenerTodos().FirstOrDefault(t => t.Id == idActuacion)
            ?? throw new EntidadNoEncontradaException($"La actuacion con ID {idActuacion} no existe.");


        if (!Enum.TryParse<TipoActuacion>(request.tipo, true, out var nuevoTipoValido))
            throw new EntidadNoEncontradaException($"El tipo de actuacion '{request.tipo}' no es válido.");

        var detalleValido = new DetalleActuacion(request.detalle);

        actuacion.Modificar(nuevoTipoValido,detalleValido,idUsuario);

        var reclamo = _reclamoRepo.ObtenerPorId(actuacion.Id)
            ?? throw new EntidadNoEncontradaException($"No existe el reclamo asociado al ID proporcionado");

        _actuacionRepo.Modificar(actuacion);
        
        _estadoService.Ejecutar(reclamo,idUsuario);
        
        return new ModificarActuacionResponse(actuacion.Id.ToString(),actuacion.Detalle.Descripcion,actuacion.Tipo.ToString());
    }



}

public record ModificarActuacionRequest(string idActuacion, string detalle, string tipo, string idUsuario);
public record ModificarActuacionResponse(string idActuacion, string detalleNuevo, string nuevoTipo);