namespace _4VGymAPIEF.Data
    
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Identity.Client;
    using Models;
    //es la clase de Entity Framework que conecta tu código C# con la base de datos.
    //Permite consultar, añadir, modificar y eliminar datos mediante objetos.


    public class GymDbContext : DbContext
    {
        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
        {
        }
        //Un DbSet<T> es una propiedad del DbContext que permite consultar y gestionar las entidades de un tipo,
        //normalmente asociadas a una tabla de la base de datos.
        public DbSet<Activity> Activities { get; set; }

        public DbSet<Instructor> Instructors { get; set; }

        public DbSet<ActivityType> ActivityTypes { get; set; }








    }
}
