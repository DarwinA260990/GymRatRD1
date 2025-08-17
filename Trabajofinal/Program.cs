using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

//Entidades
namespace Trabajofinal1.Models.Entities
{
    public class Miembro
    {
        [Key] public int MiembroId { get; set; }
        [Required, MaxLength(150)] public string Nombre { get; set; }
        [MaxLength(150)] public string Apellido { get; set; }
        public string Cedula { get; set; }
        [MaxLength(50)] public string Telefono { get; set; }
        [MaxLength(100)] public string Correo { get; set; }

        //Relación con Membresía
        public int? MembresiaId { get; set; }
        public Membresia Membresia { get; set; }
    }

    public class Membresia
    {
        [Key] public int MembresiaId { get; set; }
        [Required, MaxLength(150)] public string Tipo { get; set; }
        [MaxLength(100)] public string Duracion { get; set; }
        public string Costo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    public class Entrenador
    {
        [Key] public int EntrenadorId { get; set; }
        [Required, MaxLength(150)] public string Nombre { get; set; }
        [MaxLength(100)] public string Apellido { get; set; }
        public string Cedula { get; set; }
        [MaxLength(50)] public string Especialidad { get; set; }
        public decimal Salario { get; set; }
    }

    public class Rutina
    {
        [Key] public int RutinaId { get; set; }
        [Required, MaxLength(150)] public string Nombre { get; set; }
        [MaxLength(100)] public string Descripcion { get; set; }
        [MaxLength(50)] public string Duracion { get; set; }
        [MaxLength(50)] public string NivelDificultad { get; set; }
        [MaxLength(50)] public string TiempoEntrenamiento { get; set; }
    }

    public class Pago
    {
        [Key] public int PagoId { get; set; }
        public string Monto { get; set; }           
        public DateTime Fecha { get; set; }
        [MaxLength(50)] public string MetodoPago { get; set; }

        //Relación con Miembro
        public int? MiembroId { get; set; }
        public Miembro Miembro { get; set; }
    }

    public class Asistencia
    {
        [Key] public int AsistenciaId { get; set; }
        public DateTime Fecha { get; set; }
        [MaxLength(50)] public string HoraEntrada { get; set; }
        [MaxLength(50)] public string HoraSalida { get; set; }

        //Relación con Miembro 
        public int? MiembroId { get; set; }
        public Miembro Miembro { get; set; }
    }
}

//Context
namespace Trabajofinal1.Data
{
    using Trabajofinal1.Models.Entities;

    public class GymDbContext : DbContext
    {
        public DbSet<Miembro> Miembros { get; set; }
        public DbSet<Membresia> Membresias { get; set; }
        public DbSet<Entrenador> Entrenadores { get; set; }
        public DbSet<Rutina> Rutinas { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }

        //Conexion base de datos
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=.\\SQLEXPRESS;Database=GymRatRD;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Miembro -> Membresia, En este caso si borramos la Membresia, los Miembros quedan sin membresía.
            modelBuilder.Entity<Miembro>()
                .HasOne(m => m.Membresia)
                .WithMany()
                .HasForeignKey(m => m.MembresiaId)
                .OnDelete(DeleteBehavior.SetNull);

            // Pago -> Miembro, en este caso si borramos Miembro, los pagos quedan con FK NULL.
            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Miembro)
                .WithMany()
                .HasForeignKey(p => p.MiembroId)
                .OnDelete(DeleteBehavior.SetNull);

            // Asistencia -> Miembro, En este caso si borramos Miembro, las asistencias quedan con FK NULL.
            modelBuilder.Entity<Asistencia>()
                .HasOne(a => a.Miembro)
                .WithMany()
                .HasForeignKey(a => a.MiembroId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

//Programa principal
namespace Trabajofinal1
{
    using Trabajofinal1.Data;
    using Trabajofinal1.Models.Entities;

    public class Program
    {
        public static void Main(string[] args)
        {
            using (var db = new GymDbContext())
            {
                // Para prototipo. Si usas migraciones, cambia a db.Database.Migrate();
                db.Database.EnsureCreated();
            }

            Console.WriteLine("======================================");
            Console.WriteLine("              GymRatRD                ");
            Console.WriteLine("======================================");

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nSeleccione la opcion a la que quiere acceder caballero:");
                Console.WriteLine("1. Miembros");
                Console.WriteLine("2. Membresías");
                Console.WriteLine("3. Entrenadores");
                Console.WriteLine("4. Rutinas");
                Console.WriteLine("5. Pagos");
                Console.WriteLine("6. Asistencias");
                Console.WriteLine("7. Salir");
                Console.Write("Opción: ");
                var option = Console.ReadLine();

                switch (option)
                {
                    case "1": MenuMembers(); break;
                    case "2": MenuMemberships(); break;
                    case "3": MenuTrainers(); break;
                    case "4": MenuRoutines(); break;
                    case "5": MenuPayments(); break;
                    case "6": MenuAttendances(); break;
                    case "7": exit = true; break;
                    default: Console.WriteLine("Su opcion no esta bien caballero."); break;
                }
            }
        }

        //Utilidades para leer los datos
        static int ReadInt(string prompt, bool optional = false)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                var s = Console.ReadLine();
                if (optional && string.IsNullOrWhiteSpace(s)) return 0;
                if (int.TryParse(s, out var v)) return v;
                Console.WriteLine("Su valor es inválido caballero, intente nuevamente.");
            }
        }

        static decimal ReadDecimal(string prompt, bool optional = false)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                var s = Console.ReadLine();
                if (optional && string.IsNullOrWhiteSpace(s)) return 0m;
                if (decimal.TryParse(s, out var v)) return v;
                Console.WriteLine("Su valor es inválido caballero, intente nuevamente.");
            }
        }

        static DateTime ReadDate(string prompt, bool optional = false)
        {
            while (true)
            {
                Console.Write($"{prompt} (Año-Mes-Dia): ");
                var s = Console.ReadLine();
                if (optional && string.IsNullOrWhiteSpace(s)) return DateTime.MinValue;
                if (DateTime.TryParse(s, out var v)) return v;
                Console.WriteLine("Su fecha es inválida caballero, intente nuevamente.");
            }
        }

        static string ReadText(string prompt, bool optional = false)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                var s = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(s) || optional) return s ?? "";
                Console.WriteLine("El texto es requerido caballero, intente nuevamente.");
            }
        }

        //Menus de opciones
        static void MenuMembers()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n--- Miembros ---");
                Console.WriteLine("1. Agregar");
                Console.WriteLine("2. Eliminar");
                Console.WriteLine("3. Mostrar miembros");
                Console.WriteLine("4. Volver");
                Console.Write("Opción: ");
                var op = Console.ReadLine();

                switch (op)
                {
                    case "1": AddMember(); break;
                    case "2": DeleteMember(); break;
                    case "3": ListMembers(); break;
                    case "4": back = true; break;
                    default: Console.WriteLine("Opción no válida caballero."); break;
                }
            }
        }

        static void MenuMemberships()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n--- Membresias ---");
                Console.WriteLine("1. Agregar");
                Console.WriteLine("2. Eliminar");
                Console.WriteLine("3. Mostrar membreias");
                Console.WriteLine("4. Volver");
                Console.Write("Opción: ");
                var op = Console.ReadLine();

                switch (op)
                {
                    case "1": AddMembership(); break;
                    case "2": DeleteMembership(); break;
                    case "3": ListMemberships(); break;
                    case "4": back = true; break;
                    default: Console.WriteLine("Opción no válida caballero."); break;
                }
            }
        }

        static void MenuTrainers()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n--- Entrenadores ---");
                Console.WriteLine("1. Agregar");
                Console.WriteLine("2. Eliminar");
                Console.WriteLine("3. Mostrar entrenadores");
                Console.WriteLine("4. Volver");
                Console.Write("Opción: ");
                var op = Console.ReadLine();

                switch (op)
                {
                    case "1": AddTrainer(); break;
                    case "2": DeleteTrainer(); break;
                    case "3": ListTrainers(); break;
                    case "4": back = true; break;
                    default: Console.WriteLine("Opción no válida caballero."); break;
                }
            }
        }

        static void MenuRoutines()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n--- Rutinas ---");
                Console.WriteLine("1. Agregar");
                Console.WriteLine("2. Eliminar");
                Console.WriteLine("3. Mostrar Rutinas");
                Console.WriteLine("4. Volver");
                Console.Write("Opción: ");
                var op = Console.ReadLine();

                switch (op)
                {
                    case "1": AddRoutine(); break;
                    case "2": DeleteRoutine(); break;
                    case "3": ListRoutines(); break;
                    case "4": back = true; break;
                    default: Console.WriteLine("Opción no válida caballero."); break;
                }
            }
        }

        static void MenuPayments()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n--- Pagos ---");
                Console.WriteLine("1. Registrar");
                Console.WriteLine("2. Eliminar");
                Console.WriteLine("3. Mostrar pagos");
                Console.WriteLine("4. Volver");
                Console.Write("Opción: ");
                var op = Console.ReadLine();

                switch (op)
                {
                    case "1": AddPayment(); break;
                    case "2": DeletePayment(); break;
                    case "3": ListPayments(); break;
                    case "4": back = true; break;
                    default: Console.WriteLine("Opción no válida caballero."); break;
                }
            }
        }

        static void MenuAttendances()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n--- Asistencias ---");
                Console.WriteLine("1. Registrar");
                Console.WriteLine("2. Eliminar");
                Console.WriteLine("3. Mostrar asistencias");
                Console.WriteLine("4. Volver");
                Console.Write("Opción: ");
                var op = Console.ReadLine();

                switch (op)
                {
                    case "1": AddAttendance(); break;
                    case "2": DeleteAttendance(); break;
                    case "3": ListAttendances(); break;
                    case "4": back = true; break;
                    default: Console.WriteLine("Opción no válida caballero."); break;
                }
            }
        }

        //CRUD: Miembros
        static void AddMember()
        {
            using var db = new GymDbContext();

            int memInput = ReadInt("ID Membresía", true);
            int? membershipId = memInput == 0 ? (int?)null : memInput;

            // FIX: Hay que validar que la membresía exista antes de guardar.
            if (membershipId.HasValue && !db.Membresias.Any(m => m.MembresiaId == membershipId.Value))
            {
                Console.WriteLine("La membresía indicada no existe. Cree la membresía o ponga un 0 caballero.");
                return;
            }

            var entity = new Miembro
            {
                Nombre = ReadText("Nombre"),
                Apellido = ReadText("Apellido", true),
                Cedula = ReadText("Cédula"),
                Telefono = ReadText("Teléfono", true),
                Correo = ReadText("Correo", true),
                MembresiaId = membershipId
            };

            db.Miembros.Add(entity);
            db.SaveChanges();
            Console.WriteLine("Miembro agregado.");
        }

        static void DeleteMember()
        {
            using var db = new GymDbContext();
            int id = ReadInt("ID del miembro a eliminar");
            var entity = db.Miembros.Find(id);
            if (entity == null) { Console.WriteLine("No encontrado."); return; }

            //FIX: eliminar dependientes para no chocar con la FK si el esquema no tiene SET NULL
            var pagos = db.Pagos.Where(p => p.MiembroId == id).ToList();
            var asist = db.Asistencias.Where(a => a.MiembroId == id).ToList();
            db.Pagos.RemoveRange(pagos);
            db.Asistencias.RemoveRange(asist);

            db.Miembros.Remove(entity);
            db.SaveChanges();
            Console.WriteLine("Miembro eliminado.");
        }

        static void ListMembers()
        {
            using var db = new GymDbContext();
            var list = db.Miembros.Include(x => x.Membresia).ToList();
            Console.WriteLine("\nMiembros:");
            foreach (var x in list)
                Console.WriteLine($"{x.MiembroId} - {x.Nombre} {x.Apellido} | Cédula: {x.Cedula} | Membresía: {x.Membresia?.Tipo ?? "N/D"}");
        }

        //CRUD: Membresias
        static void AddMembership()
        {
            using var db = new GymDbContext();
            var entity = new Membresia
            {
                Tipo = ReadText("Tipo"),
                Duracion = ReadText("Duración", true),
                Costo = ReadText("Costo"),
                FechaInicio = ReadDate("Fecha de inicio"),
                FechaFin = ReadDate("Fecha de fin")
            };
            db.Membresias.Add(entity);
            db.SaveChanges();
            Console.WriteLine("Membresía agregada.");
        }

        static void DeleteMembership()
        {
            using var db = new GymDbContext();
            int id = ReadInt("ID de la membresía a eliminar");
            var entity = db.Membresias.Find(id);
            if (entity == null) { Console.WriteLine("No encontrada."); return; }

            //Si el esquema no tiene SET NULL, evitaremos borrar si hay miembros vinculados.
            bool hasMembers = db.Miembros.Any(m => m.MembresiaId == id);
            if (hasMembers)
            {
                Console.WriteLine("No se puede eliminar: hay miembros asociados. Primero reasigne o quite la membresía de esos miembros.");
                return;
            }

            db.Membresias.Remove(entity);
            db.SaveChanges();
            Console.WriteLine("Membresía eliminada.");
        }

        static void ListMemberships()
        {
            using var db = new GymDbContext();
            var list = db.Membresias.ToList();
            Console.WriteLine("\nMembresías:");
            foreach (var x in list)
                Console.WriteLine($"{x.MembresiaId} - {x.Tipo} | {x.Duracion} | ${x.Costo} | {x.FechaInicio:Año-Mes-Dia} → {x.FechaFin:Año-Mes-Dia}");
        }

        //CRUD: Entrenadores 
        static void AddTrainer()
        {
            using var db = new GymDbContext();
            var e = new Entrenador
            {
                Nombre = ReadText("Nombre"),
                Apellido = ReadText("Apellido", true),
                Cedula = ReadText("Cédula"),
                Especialidad = ReadText("Especialidad", true),
                Salario = ReadDecimal("Salario")
            };
            db.Entrenadores.Add(e);
            db.SaveChanges();
            Console.WriteLine("Entrenador agregado.");
        }

        static void DeleteTrainer()
        {
            using var db = new GymDbContext();
            int id = ReadInt("ID del entrenador a eliminar");
            var entity = db.Entrenadores.Find(id);
            if (entity == null) { Console.WriteLine("No encontrado."); return; }
            db.Entrenadores.Remove(entity);
            db.SaveChanges();
            Console.WriteLine("Entrenador eliminado.");
        }

        static void ListTrainers()
        {
            using var db = new GymDbContext();
            var list = db.Entrenadores.ToList();
            Console.WriteLine("\nEntrenadores:");
            foreach (var x in list)
                Console.WriteLine($"{x.EntrenadorId} - {x.Nombre} {x.Apellido} | {x.Especialidad} | ${x.Salario}");
        }

        //CRUD: Rutinas
        static void AddRoutine()
        {
            using var db = new GymDbContext();
            var e = new Rutina
            {
                Nombre = ReadText("Nombre"),
                Descripcion = ReadText("Descripción", true),
                Duracion = ReadText("Duración", true),
                NivelDificultad = ReadText("Nivel de dificultad", true),
                TiempoEntrenamiento = ReadText("Tiempo de entrenamiento", true)
            };
            db.Rutinas.Add(e);
            db.SaveChanges();
            Console.WriteLine("Rutina agregada.");
        }

        static void DeleteRoutine()
        {
            using var db = new GymDbContext();
            int id = ReadInt("ID de la rutina a eliminar");
            var entity = db.Rutinas.Find(id);
            if (entity == null) { Console.WriteLine("No encontrada."); return; }
            db.Rutinas.Remove(entity);
            db.SaveChanges();
            Console.WriteLine("Rutina eliminada.");
        }

        static void ListRoutines()
        {
            using var db = new GymDbContext();
            var list = db.Rutinas.ToList();
            Console.WriteLine("\nRutinas:");
            foreach (var x in list)
                Console.WriteLine($"{x.RutinaId} - {x.Nombre} | {x.Duracion} | {x.NivelDificultad} | {x.TiempoEntrenamiento}");
        }

        //CRUD: Pagos
        static void AddPayment()
        {
            using var db = new GymDbContext();
            int memberIdInput = ReadInt("ID del miembro", true);
            int? memberId = memberIdInput == 0 ? (int?)null : memberIdInput;

            // Validar existencia de Miembro si se proporciona
            if (memberId.HasValue && !db.Miembros.Any(m => m.MiembroId == memberId.Value))
            {
                Console.WriteLine("El miembro indicado no existe. Use un ID válido o deje 0.");
                return;
            }

            var e = new Pago
            {
                MiembroId = memberId,
                Monto = ReadText("Monto"),
                Fecha = ReadDate("Fecha del pago"),
                MetodoPago = ReadText("Método de pago", true)
            };

            db.Pagos.Add(e);
            db.SaveChanges();
            Console.WriteLine("Pago registrado.");
        }

        static void DeletePayment()
        {
            using var db = new GymDbContext();
            int id = ReadInt("ID del pago a eliminar");
            var entity = db.Pagos.Find(id);
            if (entity == null) { Console.WriteLine("No encontrado."); return; }
            db.Pagos.Remove(entity);
            db.SaveChanges();
            Console.WriteLine("Pago eliminado.");
        }

        static void ListPayments()
        {
            using var db = new GymDbContext();
            var list = db.Pagos.Include(p => p.Miembro).ToList();
            Console.WriteLine("\nPagos:");
            foreach (var x in list)
            {
                string miembroName = x.Miembro != null ? $"{x.Miembro.Nombre} {x.Miembro.Apellido}" : "N/D";
                Console.WriteLine($"{x.PagoId} - ${x.Monto} | {x.Fecha:Año-Mes-Dia} | Miembro: {miembroName} | {x.MetodoPago}");
            }
        }

        //CRUD: Asistencias
        static void AddAttendance()
        {
            using var db = new GymDbContext();
            int memberIdInput = ReadInt("ID del miembro", true);
            int? memberId = memberIdInput == 0 ? (int?)null : memberIdInput;

            // Validar existencia de Miembro si se proporciona
            if (memberId.HasValue && !db.Miembros.Any(m => m.MiembroId == memberId.Value))
            {
                Console.WriteLine("El miembro indicado no existe. Use un ID válido o deje 0.");
                return;
            }

            var e = new Asistencia
            {
                MiembroId = memberId,
                Fecha = ReadDate("Fecha de asistencia"),
                HoraEntrada = ReadText("Hora de entrada (H:M)", true),
                HoraSalida = ReadText("Hora de salida (H:M)", true)
            };
            db.Asistencias.Add(e);
            db.SaveChanges();
            Console.WriteLine("Asistencia registrada.");
        }

        static void DeleteAttendance()
        {
            using var db = new GymDbContext();
            int id = ReadInt("ID de la asistencia a eliminar");
            var entity = db.Asistencias.Find(id);
            if (entity == null) { Console.WriteLine("No encontrada."); return; }
            db.Asistencias.Remove(entity);
            db.SaveChanges();
            Console.WriteLine("Asistencia eliminada.");
        }

        static void ListAttendances()
        {
            using var db = new GymDbContext();
            var list = db.Asistencias.Include(a => a.Miembro).ToList();
            Console.WriteLine("\nAsistencias:");
            foreach (var x in list)
            {
                string miembroName = x.Miembro != null ? $"{x.Miembro.Nombre} {x.Miembro.Apellido}" : "N/D";
                Console.WriteLine($"{x.AsistenciaId} - {x.Fecha:Año-Mes-Dia} | {x.HoraEntrada}-{x.HoraSalida} | Miembro: {miembroName}");
            }
        }
    }
}


