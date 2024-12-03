using System.Text;
using Microsoft.EntityFrameworkCore;

namespace LunaTestTask.Models.Contexts;

public class UserContext : DbContext
{
    private DbSet<UserModel> Users { get; set; }
    public UserContext(DbContextOptions<UserContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserModel>().ToTable("User");
    }

    public async Task<string> ValidUniqueUser(UserModel userModel)
    {
        var errors = new StringBuilder();
        if (await Users.AnyAsync(i => i.Username == userModel.Username)) errors.Append("Username is already used\n");
        if (await Users.AnyAsync(i => i.Email == userModel.Email)) errors.Append("Email is already used");

        return errors.ToString();
    }

    public async Task<UserModel> GetUser(UserModel userModel)
    {
        return await Users.Where(user => user.Username == userModel.Username || user.Email == userModel.Email)
            .FirstOrDefaultAsync();;
    }

    public async Task AddUser(UserModel userModel)
    {
        userModel.CreatedAt = DateTime.UtcNow;
        userModel.UpdatedAt = DateTime.UtcNow;
        await Users.AddAsync(userModel);
        await SaveChangesAsync();
    }
}
