using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Entities;
using Domain.Enums;

namespace Repository
{
    public class MateriaRepository
    {
        private readonly string _rutaArchivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "progreso_materias.json");
        
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public List<Semestre> ObtenerSemestres()
        {
            List<Semestre> planBase = ObtenerPlanDeEstudiosBase();

            List<MateriaEstadoGuardado> estadosGuardados = CargarEstadosDesdeArchivo();

            if (estadosGuardados.Count > 0)
            {
                var dicEstados = estadosGuardados.ToDictionary(e => e.NombreMateria, e => e.Estado);

                foreach (var semestre in planBase)
                {
                    foreach (var materia in semestre.Materias) 
                    {
                        if (dicEstados.TryGetValue(materia.Nombre, out var estadoGuardado))
                        {
                            materia.CambiarEstado(estadoGuardado);
                        }
                    }
                }
            }

            return planBase;
        }

        public void GuardarProgreso(List<Semestre> semestres)
        {
            var estadosAGuardar = new List<MateriaEstadoGuardado>();

            foreach (var semestre in semestres)
            {
                foreach (var materia in semestre.Materias)
                {
                    estadosAGuardar.Add(new MateriaEstadoGuardado
                    {
                        NombreMateria = materia.Nombre,
                        Estado = materia.Estado 
                    });
                }
            }

            try
            {
                string jsonString = JsonSerializer.Serialize(estadosAGuardar, _jsonOptions);
                File.WriteAllText(_rutaArchivo, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar el progreso: {ex.Message}");
            }
        }

        private List<MateriaEstadoGuardado> CargarEstadosDesdeArchivo()
        {
            if (!File.Exists(_rutaArchivo))
            {
                return new List<MateriaEstadoGuardado>();
            }

            try
            {
                string jsonString = File.ReadAllText(_rutaArchivo);
                return JsonSerializer.Deserialize<List<MateriaEstadoGuardado>>(jsonString, _jsonOptions) 
                       ?? new List<MateriaEstadoGuardado>();
            }
            catch
            {
                return new List<MateriaEstadoGuardado>();
            }
        }
        public List<Semestre> ObtenerPlanDeEstudiosBase()
        {
            //Semestre 1
            var prog1 = new Materia("Programación 1", "", 0, 5);
            var calculo1 = new Materia("Cálculo en una variable", "", 0, 7);
            var algebra1 = new Materia("Algebra Lineal", "", 0, 7);
            var taller1 = new Materia("Taller de Tecnologia 1", "", 0, 1);
            var semestre1 = new Semestre(1);
            semestre1.AgregarMateria(prog1);
            semestre1.AgregarMateria(calculo1);
            semestre1.AgregarMateria(algebra1);
            semestre1.AgregarMateria(taller1);
            //Semestre 2
            var prog2 = new Materia("Programación 2", "POO", 0, 6);
            var fisica = new Materia("Fundamentos de Sistemas Ciberfisicos", "",  0, 9);
            var fundamentos = new Materia("Fundamentos de Computación", "", 0, 3);
            var discreta = new Materia("Matematica Discreta", "", 0, 8);
            
            prog2.Previas.Add(new Previa(prog1, TipoCreditoPrevia.Parcial));
            var semestre2 = new Semestre(2);
            semestre2.AgregarMateria(prog2);
            semestre2.AgregarMateria(discreta);
            semestre2.AgregarMateria(fisica);
            semestre2.AgregarMateria(fundamentos);
            
            //Semestre 3
            var logica = new Materia("Lógica para computación", "",  0, 5);
            var arqui1 = new Materia("Arquitectura de Sistemas", "",   0, 5);
            var pye = new Materia("Probabilidad y Estadisticas", "",   0, 7);
            var A1 = new Materia("Estructuras de Datos y Algoritmos 1", "",  0, 5);
            
            logica.Previas.Add(new Previa(fundamentos, TipoCreditoPrevia.Parcial));
            pye.Previas.Add(new Previa(calculo1, TipoCreditoPrevia.Total));
            pye.Previas.Add(new Previa(algebra1, TipoCreditoPrevia.Total));
            A1.Previas.Add(new Previa(prog2, TipoCreditoPrevia.Parcial));
            A1.Previas.Add(new Previa(fundamentos, TipoCreditoPrevia.Parcial));

            var semestre3 = new Semestre(3);
            semestre3.AgregarMateria(pye);
            semestre3.AgregarMateria(A1);
            semestre3.AgregarMateria(logica);
            semestre3.AgregarMateria(arqui1);
            
            
            //Semstre 4
            var semestre4  = new Semestre(4);
            
            var SO = new Materia("Sistemas Operativos", "",  6, 6);
            var A2 = new Materia("Estructuras de Datos y Algoritmos 2","",  6, 8);
            var bd1 = new Materia("Base de Datos 1", "",  6, 3);
            var FIS = new Materia("Fundamentos de Ingenieria de Software", "",  6, 5);
            var algebra2 = new Materia("Matematica electiva", "",  6, 7);
            
            SO.Previas.Add(new Previa(arqui1, TipoCreditoPrevia.Parcial));
            A2.Previas.Add(new Previa(discreta, TipoCreditoPrevia.Parcial));
            A2.Previas.Add(new Previa(A1, TipoCreditoPrevia.Parcial));
            bd1.Previas.Add(new Previa(prog2, TipoCreditoPrevia.Parcial));
            FIS.Previas.Add(new Previa(prog2, TipoCreditoPrevia.Parcial));
            algebra2.Previas.Add(new Previa(calculo1, TipoCreditoPrevia.Total));
            algebra2.Previas.Add(new Previa(algebra1, TipoCreditoPrevia.Total));

            semestre4.AgregarMateria(algebra2);
            semestre4.AgregarMateria(bd1);
            semestre4.AgregarMateria(FIS);
            semestre4.AgregarMateria(SO);
            semestre4.AgregarMateria(A2);
            
            
            //Semestre 5
                
            var semestre5  = new Semestre(5);
            var bd2 = new Materia("Bases de Datos 2", "",  6, 6);
            var da1 = new Materia("Diseño de Aplicaciones 1", "",   6, 6);
            var admin =new Materia("Administración General", "",  6, 2);
            var redes = new Materia("Redes", "",  9, 6 );
            var teoria = new Materia("Teoria de la Computacion", "",  9, 8);
            

            bd2.Previas.Add(new Previa(bd1, TipoCreditoPrevia.Parcial));
            bd2.Previas.Add(new Previa(logica, TipoCreditoPrevia.Parcial));
            da1.Previas.Add(new  Previa(A1, TipoCreditoPrevia.Total));
            da1.Previas.Add(new  Previa(bd1, TipoCreditoPrevia.Parcial));
            da1.Previas.Add(new  Previa(FIS, TipoCreditoPrevia.Parcial));
            redes.Previas.Add(new Previa(SO, TipoCreditoPrevia.Parcial));
            teoria.Previas.Add(new Previa(logica, TipoCreditoPrevia.Parcial));
            teoria.Previas.Add(new Previa(A1, TipoCreditoPrevia.Parcial));
            
            semestre5.AgregarMateria(teoria);
            semestre5.AgregarMateria(redes);
            semestre5.AgregarMateria(admin);
            semestre5.AgregarMateria(bd2);
            semestre5.AgregarMateria(da1);
            
            //Semestre 6
            var semestre6  = new Semestre(6);
            var da2 = new Materia("Diseño de Aplicaciones 2", "",  12, 9);
            var pr = new Materia("Programación de Redes","",   12, 5);
            var taller2 = new Materia("Taller de tecnologia 2", " ", 12, 3);
            var mach = new Materia("Machine Learning", "", 12, 6);
            var ISA1 = new Materia("Ingeniería de software ágil 1","",  12, 4);
            

            da2.Previas.Add(new Previa(da1, TipoCreditoPrevia.Parcial));
            da2.Previas.Add(new Previa(FIS, TipoCreditoPrevia.Parcial));
            pr.Previas.Add(new Previa(da1, TipoCreditoPrevia.Parcial));
            pr.Previas.Add(new Previa(SO, TipoCreditoPrevia.Parcial));
            taller2.Previas.Add(new Previa(A2, TipoCreditoPrevia.Total));
            taller2.Previas.Add(new Previa(taller1, TipoCreditoPrevia.Total));
            taller2.Previas.Add(new Previa(bd1, TipoCreditoPrevia.Total));
            taller2.Previas.Add(new Previa(fisica, TipoCreditoPrevia.Parcial));
            taller2.Previas.Add(new Previa(redes, TipoCreditoPrevia.Total));
            taller2.Previas.Add(new Previa(da1, TipoCreditoPrevia.Total));
            mach.Previas.Add(new Previa(A1, TipoCreditoPrevia.Total));
            mach.Previas.Add(new Previa(pye, TipoCreditoPrevia.Total));
            ISA1.Previas.Add(new Previa(admin, TipoCreditoPrevia.Parcial));
            ISA1.Previas.Add(new Previa(da1, TipoCreditoPrevia.Parcial));
            ISA1.Previas.Add(new Previa(FIS, TipoCreditoPrevia.Parcial));
            
            semestre6.AgregarMateria(da2);
            semestre6.AgregarMateria(pr);
            semestre6.AgregarMateria(taller2);
            semestre6.AgregarMateria(mach);
            semestre6.AgregarMateria(ISA1);
            
            //Semestre 7
            var semestre7  = new Semestre(7);
            
            var IA = new Materia("Inteligencia Artificial", "",16, 7);
            var ISA2 = new Materia("Ingeniería de software ágil 2", "", 16, 5);
            var arqui2 = new Materia("Arquitecura de Software", "",16, 6);
            var seguridad = new Materia("Seguridad", "", 16, 3);
            var bigdata = new Materia("Big Data", "",  16, 4);

            IA.Previas.Add(new Previa(pye, TipoCreditoPrevia.Parcial));
            IA.Previas.Add(new Previa(A2, TipoCreditoPrevia.Parcial));
            IA.Previas.Add(new Previa(logica, TipoCreditoPrevia.Parcial));
            ISA2.Previas.Add(new Previa(da1, TipoCreditoPrevia.Total));
            ISA2.Previas.Add(new Previa(da2, TipoCreditoPrevia.Parcial));
            ISA2.Previas.Add(new Previa(pr, TipoCreditoPrevia.Parcial));
            ISA2.Previas.Add(new Previa(ISA1, TipoCreditoPrevia.Parcial));
            arqui2.Previas.Add(new Previa(A2, TipoCreditoPrevia.Total));
            arqui2.Previas.Add(new Previa(bd2, TipoCreditoPrevia.Total));
            arqui2.Previas.Add(new Previa(da1, TipoCreditoPrevia.Parcial));
            arqui2.Previas.Add(new Previa(pr, TipoCreditoPrevia.Parcial));
            bigdata.Previas.Add(new Previa(bd2, TipoCreditoPrevia.Parcial));
            bigdata.Previas.Add(new Previa(da1, TipoCreditoPrevia.Total));
            bigdata.Previas.Add(new Previa(SO, TipoCreditoPrevia.Parcial));

            semestre7.AgregarMateria(IA);
            semestre7.AgregarMateria(bigdata);
            semestre7.AgregarMateria(arqui2);
            semestre7.AgregarMateria(seguridad);
            semestre7.AgregarMateria(ISA2);
            
            
            //Semestre 8
            var semestre8  = new Semestre(8);
            
            var arqui3 = new Materia("Arquitectura de Software en la practica","", 20, 8);
            var ti = new Materia("Trabajo Integrador", "",20, 2);
            var ingProd = new Materia("Ingenieria de Productos", "" , 20, 5);
            var nuevaTec = new Materia("Materia de Nuevas tecnologias", "", 20, 4);
            var lenguajes = new Materia("Materia de Lenguaje de Programación", "",   20, 7);

            arqui3.Previas.Add(new Previa(mach, TipoCreditoPrevia.Total));
            arqui3.Previas.Add(new Previa(pr, TipoCreditoPrevia.Parcial));
            arqui3.Previas.Add(new Previa(arqui2, TipoCreditoPrevia.Parcial));
            arqui3.Previas.Add(new Previa(ISA2, TipoCreditoPrevia.Parcial));

            ti.Previas.Add(new Previa(bd2, TipoCreditoPrevia.Total));
            ti.Previas.Add(new Previa(da2, TipoCreditoPrevia.Total));
            ti.Previas.Add(new Previa(pr, TipoCreditoPrevia.Total));
            ti.Previas.Add(new Previa(teoria, TipoCreditoPrevia.Total));
            ti.Previas.Add(new Previa(ISA1, TipoCreditoPrevia.Total));

            lenguajes.Previas.Add(new Previa(A1, TipoCreditoPrevia.Parcial));
            lenguajes.Previas.Add(new Previa(logica, TipoCreditoPrevia.Parcial));

            ingProd.Previas.Add(new Previa(da2, TipoCreditoPrevia.Parcial));
            ingProd.Previas.Add(new Previa(pr, TipoCreditoPrevia.Parcial));
            ingProd.Previas.Add(new Previa(ISA1, TipoCreditoPrevia.Parcial));

            nuevaTec.Previas.Add(new Previa(mach, TipoCreditoPrevia.Parcial));
            nuevaTec.Previas.Add(new Previa(da2, TipoCreditoPrevia.Parcial));

            
            semestre8.AgregarMateria(arqui3);
            semestre8.AgregarMateria(ti);
            semestre8.AgregarMateria(ingProd);
            semestre8.AgregarMateria(nuevaTec);
            semestre8.AgregarMateria(lenguajes);
            
            
            //Semestre 9
            var semestre9  = new Semestre(9);
            var comunicacion = new Materia("Materia de Comunicacion","", 16, 2);
            var electiva1 = new Materia("Electiva 1", "", 16, 1);
            
            comunicacion.Previas.Add(new Previa(ISA1, TipoCreditoPrevia.Total));
            semestre9.AgregarMateria(comunicacion);
            semestre9.AgregarMateria(electiva1);
            
            //Semestre 10
            var semestre10 = new Semestre(10);
            
            var electiva2 = new Materia("Electiva 2","", 16, 1);
            var electiva3 = new Materia("Electiva 3", "", 16, 1);
            
            semestre10.AgregarMateria(electiva2);
            semestre10.AgregarMateria(electiva3);

            return new List<Semestre>
            {
                semestre1,
                semestre2, 
                semestre3,
                semestre4,
                semestre5,
                semestre6,
                semestre7,
                semestre8,
                semestre9,
                semestre10
            };
        }
    }
}