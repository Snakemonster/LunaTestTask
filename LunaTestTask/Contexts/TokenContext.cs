using Microsoft.EntityFrameworkCore;

namespace LunaTestTask.Models.Contexts;

public class TokenContext : DbContext
{
    public DbSet<TokenModel> Tokens { get; set; }
    public TokenContext(DbContextOptions<TokenContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<TokenModel>().ToTable("Token");
    }

    public async Task<TokenModel?> GetToken(string authKey)
    {
        return await Tokens.Where(token => token.Token == authKey.Substring("Bearer ".Length).Trim()).FirstOrDefaultAsync();
    }

    public async Task<Guid> GetUserId(string authKey)
    {
        return (await Tokens.Where(token => token.Token == authKey.Substring("Bearer ".Length).Trim()).FirstOrDefaultAsync()).UserId;
    }
}