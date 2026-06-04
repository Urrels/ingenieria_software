using System.Collections.Generic;

namespace SeguridadYServicios
{
    /// <summary>
    /// Algoritmo genérico de dígitos verificadores.
    ///
    /// DVH (horizontal, por fila):
    ///   dvh = Σ_i Σ_j  Unicode(atributo[i][j])  ×  (i+1)  ×  (j+1)
    ///   donde i = posición del atributo (0-based) y j = posición del carácter (0-based).
    ///   La posición del atributo y la del carácter participan explícitamente en el
    ///   producto, de modo que intercambiar atributos o caracteres produce un DVH distinto.
    ///
    /// DVV (vertical, por columna):
    ///   dvv = Σ_k Σ_j  Unicode(filas[k][colIdx][j])  ×  (k+1)  ×  (j+1)
    ///   donde k = posición de la fila en el conjunto ordenado (0-based).
    ///   Detecta inserciones, eliminaciones e intercambios de filas.
    /// </summary>
    public static class CalculadorDVH
    {
        public static int Calcular(string[] atributos)
        {
            int dvh = 0;
            for (int i = 0; i < atributos.Length; i++)
            {
                string val = atributos[i] ?? string.Empty;
                for (int j = 0; j < val.Length; j++)
                    dvh += val[j] * (i + 1) * (j + 1);
            }
            return dvh;
        }

        public static int CalcularVertical(List<string[]> filas, int colIdx)
        {
            int dvv = 0;
            for (int k = 0; k < filas.Count; k++)
            {
                string val = (filas[k] != null && colIdx < filas[k].Length)
                    ? filas[k][colIdx] ?? string.Empty
                    : string.Empty;
                for (int j = 0; j < val.Length; j++)
                    dvv += val[j] * (k + 1) * (j + 1);
            }
            return dvv;
        }
    }
}
