using Microsoft.EntityFrameworkCore;

namespace LunaTestTask.Models.Contexts;

public class TokenContext : DbContext
{
    private DbSet<TokenModel> Tokens { get; set; }
    public TokenContext(DbContextOptions<TokenContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<TokenModel>().ToTable("Token");
    }

    public async Task<TokenModel?> GetToken(string authKey)
    {
        if (authKey.StartsWith("Bearer ")) authKey = authKey.Substring("Bearer ".Length).Trim();
        return await Tokens.Where(token => token.Token == authKey).FirstOrDefaultAsync();
    }

    public async Task<Guid> GetTokenUserId(string authKey)
    {
        return (await GetToken(authKey)).UserId;
    }

    public async Task<TokenModel?> GetTokenByUserId(Guid userId)
    {
        return await Tokens.Where(Token => Token.UserId == userId).FirstOrDefaultAsync();
    }

    public async Task CreateNewToken(TokenModel tokenModel)
    {
        await Tokens.AddAsync(tokenModel);
        await SaveChangesAsync();
    }

    public async Task DeleteToken(TokenModel tokenModel)
    {
        Tokens.Remove(tokenModel);
        await SaveChangesAsync();
    }
}