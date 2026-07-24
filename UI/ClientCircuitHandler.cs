using Microsoft.AspNetCore.Components.Server.Circuits;

namespace UI;

public class ClientCircuitHandler : CircuitHandler
{
    private readonly IHostApplicationLifetime _lifetime;

    private int _usuariosConectados = 0;

    private readonly string _archivoLog =
        Path.Combine(
            AppContext.BaseDirectory,
            "materiasapp.log"
        );


    public ClientCircuitHandler(IHostApplicationLifetime lifetime)
    {
        _lifetime = lifetime;
    }


    private void Log(string mensaje)
    {
        File.AppendAllText(
            _archivoLog,
            $"{DateTime.Now:HH:mm:ss} - {mensaje}{Environment.NewLine}"
        );
    }


    public override Task OnCircuitOpenedAsync(
        Circuit circuit,
        CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref _usuariosConectados);

        Log($"CONECTADO. Usuarios: {_usuariosConectados}");

        return Task.CompletedTask;
    }



    public override Task OnCircuitClosedAsync(
        Circuit circuit,
        CancellationToken cancellationToken)
    {
        Interlocked.Decrement(ref _usuariosConectados);

        Log($"DESCONECTADO. Usuarios: {_usuariosConectados}");


        if (_usuariosConectados <= 0)
        {
            Task.Run(async () =>
            {
                await Task.Delay(3000);


                if (_usuariosConectados <= 0)
                {
                    Log("CERRANDO APLICACIÓN");

                    Environment.Exit(0);
                }

            });
        }


        return Task.CompletedTask;
    }
}