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

