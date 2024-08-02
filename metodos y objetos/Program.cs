using System;
using System.IO;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;

class Program
{
    static void Main(string[] args)
    {
        CajeroAutomatico cajero = new CajeroAutomatico();
        cajero.Iniciar();
    }
}

class Cuenta
{
    public string Numero { get; private set; }
    public string Clave { get; private set; }
    public double Saldo { get; private set; }

    public Cuenta(string numero, string clave, double saldo)
    {
        Numero = numero;
        Clave = clave;
        Saldo = saldo;
    }

    public bool VerificarClave(string clave)
    {
        return Clave == clave;
    }

    public void CambiarClave(string nuevaClave)
    {
        Clave = nuevaClave;
    }

    public void RealizarDeposito(double monto)
    {
        Saldo += monto;
        RegistrarMovimiento($"Depósito: ${monto}. Nuevo saldo: ${Saldo}");
    }

    public bool RealizarRetiro(double monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            RegistrarMovimiento($"Retiro: ${monto}. Nuevo saldo: ${Saldo}");
            return true;
        }
        return false;
    }

    public void RealizarTransferencia(Cuenta cuentaDestino, double monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            cuentaDestino.Saldo += monto;
            RegistrarMovimiento($"Transferencia: ${monto} a cuenta {cuentaDestino.Numero}. Nuevo saldo: ${Saldo}");
            cuentaDestino.RegistrarMovimiento($"Transferencia recibida: ${monto} de cuenta {Numero}. Nuevo saldo: {cuentaDestino.Saldo}");
        }
    }

    public void RegistrarMovimiento(string movimiento)
    {
        string rutaArchivo = $"{Numero}_movimientos.txt";
        File.AppendAllLines(rutaArchivo, new string[] { movimiento });
    }

    public string[] ObtenerMovimientos()
    {
        string rutaArchivo = $"{Numero}_movimientos.txt";
        return File.Exists(rutaArchivo) ? File.ReadAllLines(rutaArchivo) : new string[0];
    }
}

class CajeroAutomatico
{
    Cuenta[] cuentas = new Cuenta[3];

    public CajeroAutomatico()
    {
        cuentas[0] = new Cuenta("253090", "1346", 1800000.00);
        cuentas[1] = new Cuenta("569543", "1245", 1250000.00);
        cuentas[2] = new Cuenta("794613", "8956", 2000000.00);
    }

    public void Iniciar()
    {
        string cuentaNumero, clave;
        Cuenta cuenta;

        do
        {
            Console.WriteLine("Bienvenido al Cajero Automático");
            Console.WriteLine("Por favor ingrese su número de cuenta");
            cuentaNumero = Console.ReadLine();

            Console.WriteLine("Ingrese su Contraseña");
            clave = Console.ReadLine();

            cuenta = BuscarCuenta(cuentaNumero, clave);

            if (cuenta != null)
            {
                OperarCajero(cuenta);
            }
            else
            {
                Console.WriteLine("Número de Cuenta o Contraseña incorrecta. Por favor, inténtelo de nuevo.");
            }
        } while (true);
    }

    private void OperarCajero(Cuenta cuenta)
    {
        int opcion;

        do
        {
            System.Threading.Thread.Sleep(4000);
            Console.Clear();
            MostrarMenu();
            opcion = LeerOpcion();

            switch (opcion)
            {
                case 1:
                    RealizarRetiro(cuenta);
                    break;
                case 2:
                    MostrarMovimientos(cuenta);
                    break;
                case 3:
                    RealizarDeposito(cuenta);
                    break;
                case 4:
                    CambiarClave(cuenta);
                    break;
                case 5:
                    ConsultarSaldo(cuenta);
                    break;
                case 6:
                    RealizarTransferencia(cuenta);
                    break;
                case 7:
                    Console.WriteLine("Gracias por utilizar nuestros servicios.");
                    return;
                default:
                    Console.WriteLine("Opción inválida. Por favor, seleccione una opción válida.");
                    break;
            }
        } while (true);
    }

    private Cuenta BuscarCuenta(string numero, string clave)
    {
        foreach (Cuenta cuenta in cuentas)
        {
            if (cuenta.Numero == numero && cuenta.VerificarClave(clave))
            {
                return cuenta;
            }
        }
        return null;
    }

    private void MostrarMenu()
    {
        Console.WriteLine("\nMenú de opciones:");
        Console.WriteLine("1. Retiro");
        Console.WriteLine("2. Movimientos");
        Console.WriteLine("3. Depósito");
        Console.WriteLine("4. Cambio de clave");
        Console.WriteLine("5. Consulta de saldo");
        Console.WriteLine("6. Transferencia");
        Console.WriteLine("7. Salir");
        Console.WriteLine("Seleccione una opción");
    }

    private int LeerOpcion()
    {
        int opcion;
        while (!int.TryParse(Console.ReadLine(), out opcion))
        {
            Console.WriteLine("Opción inválida. Por favor, seleccione una opción válida.");
        }
        return opcion;
    }

    private void RealizarRetiro(Cuenta cuenta)
    {
        Console.WriteLine("Ingrese el monto a retirar.");
        double monto;
        while (!double.TryParse(Console.ReadLine(), out monto) || monto < 0)
        {
            Console.WriteLine("Monto inválido. Por favor, ingrese un monto válido.");
        }

        if (cuenta.RealizarRetiro(monto))
        {
            Console.WriteLine($"Se ha retirado ${monto}. Nuevo saldo: ${cuenta.Saldo}");
        }
        else
        {
            Console.WriteLine("Saldo insuficiente.");
        }
    }

    private void MostrarMovimientos(Cuenta cuenta)
    {
        string[] movimientos = cuenta.ObtenerMovimientos();
        Console.WriteLine("Movimientos recientes:");
        foreach (string movimiento in movimientos)
        {
            Console.WriteLine(movimiento);
        }
    }

    private void RealizarDeposito(Cuenta cuenta)
    {
        Console.WriteLine("Ingrese el monto a depositar.");
        double monto;
        while (!double.TryParse(Console.ReadLine(), out monto) || monto <= 0)
        {
            Console.WriteLine("Monto inválido. Por favor, ingrese un monto válido.");
        }

        cuenta.RealizarDeposito(monto);
        Console.WriteLine($"Se ha depositado ${monto}. Nuevo saldo: ${cuenta.Saldo}");
    }

    private void CambiarClave(Cuenta cuenta)
    {
        Console.WriteLine("Ingrese su nueva clave:");
        string nuevaClave = Console.ReadLine();
        cuenta.CambiarClave(nuevaClave);
        Console.WriteLine("Clave cambiada correctamente.");
    }

    private void ConsultarSaldo(Cuenta cuenta)
    {
        Console.WriteLine($"Su saldo actual es: ${cuenta.Saldo}");
    }

    private void RealizarTransferencia(Cuenta cuenta)
    {
        Console.WriteLine("Ingrese el número de cuenta al que desea hacer la transferencia:");
        string cuentaDestinoNumero = Console.ReadLine();

        Cuenta cuentaDestino = BuscarCuentaDestino(cuentaDestinoNumero);

        if (cuentaDestino == null)
        {
            Console.WriteLine("La cuenta destino no existe.");
            return;
        }

        Console.WriteLine("Ingrese el monto a transferir:");
        double monto;
        while (!double.TryParse(Console.ReadLine(), out monto) || monto <= 0)
        {
            Console.WriteLine("Monto inválido. Por favor, ingrese un monto válido.");
        }

        cuenta.RealizarTransferencia(cuentaDestino, monto);
        Console.WriteLine($"Se ha transferido ${monto} a la cuenta {cuentaDestino.Numero}. Nuevo saldo: ${cuenta.Saldo}");
    }

    private Cuenta BuscarCuentaDestino(string numero)
    {
        foreach (Cuenta cuenta in cuentas)
        {
            if (cuenta.Numero == numero)
            {
                return cuenta;
            }
        }
        return null;
    }
}

