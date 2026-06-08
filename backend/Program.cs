using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Libera o frontend acessar a API
builder.Services.AddCors(options =>
{
    options.AddPolicy("Aberto", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();
app.UseCors("Aberto");

var conn = builder.Configuration.GetConnectionString("DefaultConnection")!;

// Atalho
SqlConnection Db() => new SqlConnection(conn);

// AUTENTICAÇÃO

// POST /api/login
app.MapPost("/api/login", async (LoginRequest req) =>
{
    await using var db = Db();
    await db.OpenAsync();

    var cmd = new SqlCommand(
        "SELECT Id FROM Usuarios WHERE Usuario = @u AND Senha = @s", db);
    cmd.Parameters.AddWithValue("@u", req.Usuario);
    cmd.Parameters.AddWithValue("@s", req.Senha);

    var id = await cmd.ExecuteScalarAsync();
    if (id == null)
        return Results.Unauthorized();

    return Results.Ok(new { sucesso = true, mensagem = "Login realizado!" });
});

// PRODUTOS

// GET /api/produtos
app.MapGet("/api/produtos", async () =>
{
    await using var db = Db();
    await db.OpenAsync();

    var cmd = new SqlCommand("SELECT * FROM Produtos ORDER BY Codigo", db);
    var lista = new List<object>();

    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        lista.Add(new
        {
            id           = reader.GetInt32(0),
            codigo       = reader.GetInt32(1),
            descricao    = reader.GetString(2),
            codigoBarras = reader.IsDBNull(3) ? "" : reader.GetString(3),
            valorVenda   = reader.GetDecimal(4),
            pesoBruto    = reader.GetDecimal(5),
            pesoLiquido  = reader.GetDecimal(6)
        });
    }

    return Results.Ok(lista);
});

// GET /api/produtos/{id}
app.MapGet("/api/produtos/{id}", async (int id) =>
{
    await using var db = Db();
    await db.OpenAsync();

    var cmd = new SqlCommand("SELECT * FROM Produtos WHERE Id = @id", db);
    cmd.Parameters.AddWithValue("@id", id);

    await using var reader = await cmd.ExecuteReaderAsync();
    if (!await reader.ReadAsync())
        return Results.NotFound();

    return Results.Ok(new
    {
        id           = reader.GetInt32(0),
        codigo       = reader.GetInt32(1),
        descricao    = reader.GetString(2),
        codigoBarras = reader.IsDBNull(3) ? "" : reader.GetString(3),
        valorVenda   = reader.GetDecimal(4),
        pesoBruto    = reader.GetDecimal(5),
        pesoLiquido  = reader.GetDecimal(6)
    });
});

// POST /api/produtos
app.MapPost("/api/produtos", async (ProdutoRequest req) =>
{
    await using var db = Db();
    await db.OpenAsync();

    var cmd = new SqlCommand(@"
        INSERT INTO Produtos (Codigo, Descricao, CodigoBarras, ValorVenda, PesoBruto, PesoLiquido)
        VALUES (@codigo, @descricao, @codbarras, @valor, @bruto, @liquido)", db);

    cmd.Parameters.AddWithValue("@codigo",    req.Codigo);
    cmd.Parameters.AddWithValue("@descricao", req.Descricao);
    cmd.Parameters.AddWithValue("@codbarras", (object?)req.CodigoBarras ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@valor",     req.ValorVenda);
    cmd.Parameters.AddWithValue("@bruto",     req.PesoBruto);
    cmd.Parameters.AddWithValue("@liquido",   req.PesoLiquido);

    await cmd.ExecuteNonQueryAsync();
    return Results.Ok(new { sucesso = true, mensagem = "Produto cadastrado!" });
});

// PUT /api/produtos/{id}
app.MapPut("/api/produtos/{id}", async (int id, ProdutoRequest req) =>
{
    await using var db = Db();
    await db.OpenAsync();

    var cmd = new SqlCommand(@"
        UPDATE Produtos SET
            Codigo       = @codigo,
            Descricao    = @descricao,
            CodigoBarras = @codbarras,
            ValorVenda   = @valor,
            PesoBruto    = @bruto,
            PesoLiquido  = @liquido
        WHERE Id = @id", db);

    cmd.Parameters.AddWithValue("@id",        id);
    cmd.Parameters.AddWithValue("@codigo",    req.Codigo);
    cmd.Parameters.AddWithValue("@descricao", req.Descricao);
    cmd.Parameters.AddWithValue("@codbarras", (object?)req.CodigoBarras ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@valor",     req.ValorVenda);
    cmd.Parameters.AddWithValue("@bruto",     req.PesoBruto);
    cmd.Parameters.AddWithValue("@liquido",   req.PesoLiquido);

    await cmd.ExecuteNonQueryAsync();
    return Results.Ok(new { sucesso = true, mensagem = "Produto atualizado!" });
});

// DELETE /api/produtos/{id}
app.MapDelete("/api/produtos/{id}", async (int id) =>
{
    await using var db = Db();
    await db.OpenAsync();

    var cmd = new SqlCommand("DELETE FROM Produtos WHERE Id = @id", db);
    cmd.Parameters.AddWithValue("@id", id);

    await cmd.ExecuteNonQueryAsync();
    return Results.Ok(new { sucesso = true, mensagem = "Produto excluído!" });
});

// CLIENTES

// GET /api/clientes
app.MapGet("/api/clientes", async () =>
{
    await using var db = Db();
    await db.OpenAsync();

    var cmd = new SqlCommand("SELECT * FROM Clientes ORDER BY Codigo", db);
    var lista = new List<object>();

    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        lista.Add(new
        {
            id        = reader.GetInt32(0),
            codigo    = reader.GetInt32(1),
            nome      = reader.GetString(2),
            fantasia  = reader.IsDBNull(3) ? "" : reader.GetString(3),
            documento = reader.GetString(4),
            endereco  = reader.IsDBNull(5) ? "" : reader.GetString(5)
        });
    }

    return Results.Ok(lista);
});

// GET /api/clientes/{id}
app.MapGet("/api/clientes/{id}", async (int id) =>
{
    await using var db = Db();
    await db.OpenAsync();

    var cmd = new SqlCommand("SELECT * FROM Clientes WHERE Id = @id", db);
    cmd.Parameters.AddWithValue("@id", id);

    await using var reader = await cmd.ExecuteReaderAsync();
    if (!await reader.ReadAsync())
        return Results.NotFound();

    return Results.Ok(new
    {
        id        = reader.GetInt32(0),
        codigo    = reader.GetInt32(1),
        nome      = reader.GetString(2),
        fantasia  = reader.IsDBNull(3) ? "" : reader.GetString(3),
        documento = reader.GetString(4),
        endereco  = reader.IsDBNull(5) ? "" : reader.GetString(5)
    });
});

// POST /api/clientes
app.MapPost("/api/clientes", async (ClienteRequest req) =>
{
    await using var db = Db();
    await db.OpenAsync();

    var cmd = new SqlCommand(@"
        INSERT INTO Clientes (Codigo, Nome, Fantasia, Documento, Endereco)
        VALUES (@codigo, @nome, @fantasia, @documento, @endereco)", db);

    cmd.Parameters.AddWithValue("@codigo",    req.Codigo);
    cmd.Parameters.AddWithValue("@nome",      req.Nome);
    cmd.Parameters.AddWithValue("@fantasia",  (object?)req.Fantasia ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@documento", req.Documento);
    cmd.Parameters.AddWithValue("@endereco",  (object?)req.Endereco ?? DBNull.Value);

    await cmd.ExecuteNonQueryAsync();
    return Results.Ok(new { sucesso = true, mensagem = "Cliente cadastrado!" });
});

// PUT /api/clientes/{id}
app.MapPut("/api/clientes/{id}", async (int id, ClienteRequest req) =>
{
    await using var db = Db();
    await db.OpenAsync();

    var cmd = new SqlCommand(@"
        UPDATE Clientes SET
            Codigo    = @codigo,
            Nome      = @nome,
            Fantasia  = @fantasia,
            Documento = @documento,
            Endereco  = @endereco
        WHERE Id = @id", db);

    cmd.Parameters.AddWithValue("@id",        id);
    cmd.Parameters.AddWithValue("@codigo",    req.Codigo);
    cmd.Parameters.AddWithValue("@nome",      req.Nome);
    cmd.Parameters.AddWithValue("@fantasia",  (object?)req.Fantasia ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@documento", req.Documento);
    cmd.Parameters.AddWithValue("@endereco",  (object?)req.Endereco ?? DBNull.Value);

    await cmd.ExecuteNonQueryAsync();
    return Results.Ok(new { sucesso = true, mensagem = "Cliente atualizado!" });
});

// DELETE /api/clientes/{id}
app.MapDelete("/api/clientes/{id}", async (int id) =>
{
    await using var db = Db();
    await db.OpenAsync();

    var cmd = new SqlCommand("DELETE FROM Clientes WHERE Id = @id", db);
    cmd.Parameters.AddWithValue("@id", id);

    await cmd.ExecuteNonQueryAsync();
    return Results.Ok(new { sucesso = true, mensagem = "Cliente excluído!" });
});

app.Run();

// MODELOS

record LoginRequest(string Usuario, string Senha);

record ProdutoRequest(
    int     Codigo,
    string  Descricao,
    string? CodigoBarras,
    decimal ValorVenda,
    decimal PesoBruto,
    decimal PesoLiquido
);

record ClienteRequest(
    int     Codigo,
    string  Nome,
    string? Fantasia,
    string  Documento,
    string? Endereco
);
