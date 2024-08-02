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

