using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

await using var ctx = new BlogContext();
await ctx.Database.EnsureDeletedAsync();
await ctx.Database.EnsureCreatedAsync();

var ids = new List<int>();

var configuration = new MapperConfiguration(cfg =>
{
    cfg.AddExpressionMapping();

    cfg.CreateMap<Blog, BlogDto>();
});

var mapper = new Mapper(configuration);
Expression<Func<BlogDto, bool>> dtoExpression = x => ids.Contains(x.Id);
var expression = mapper.Map<Expression<Func<Blog, bool>>>(dtoExpression);

_ = ctx.Blogs.Where(expression).ToList();

public class BlogContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseSqlServer(@"Server=localhost;Database=Blogs;User=SA;Password=password;Connect Timeout=60;ConnectRetryCount=0;Encrypt=false")
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging();
}

public class Blog
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class BlogDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
}