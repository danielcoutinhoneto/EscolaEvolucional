using System;

namespace EscolaEvolucional.Api.Models
{
    public sealed class MatriculaResultado
    {
        private MatriculaResultado(
            MatriculaResultadoTipo tipo,
            Matricula matricula)
        {
            Tipo = tipo;
            Matricula = matricula;
        }

        public MatriculaResultadoTipo Tipo { get; }
        public Matricula Matricula { get; }

        public static MatriculaResultado CriarSucesso(Matricula matricula)
        {
            if (matricula == null)
            {
                throw new ArgumentNullException(nameof(matricula));
            }

            return new MatriculaResultado(
                MatriculaResultadoTipo.Criada,
                matricula);
        }

        public static MatriculaResultado CriarFalha(MatriculaResultadoTipo tipo)
        {
            if (!Enum.IsDefined(typeof(MatriculaResultadoTipo), tipo))
            {
                throw new ArgumentOutOfRangeException(nameof(tipo));
            }

            if (tipo == MatriculaResultadoTipo.Criada)
            {
                throw new ArgumentException(
                    "O resultado de sucesso deve possuir uma matrícula.",
                    nameof(tipo));
            }

            return new MatriculaResultado(tipo, null);
        }
    }
}
