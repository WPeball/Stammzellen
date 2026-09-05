using Microsoft.EntityFrameworkCore;
using Stammzellen.Data; // Wichtig, damit das Projekt deinen Data-Ordner findet

var builder = WebApplication.CreateBuilder(args);

// NEU: SQLite Datenbank-Kontext hinzufügen
builder.Services.AddDbContext<Stammzellen.Data.DataDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection")));

// Add services to the container.
builder.Services.AddRazorPages();

// --- SESSIONS AKTIVIEREN & ALS ESSENTIELL MARKIEREN ---
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Nach 30 Minuten Inaktivität ausloggen
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // WICHTIG: Erlaubt Sessions auch ohne Cookie-Banner!
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles(); // oder app.MapStaticAssets(); je nach .NET Version
app.UseRouting();
// HIER MUSS APP.USESESSION STEHEN:
app.UseSession();
app.UseAuthorization();




//app.UseHttpsRedirection();
//app.UseRouting();
//app.UseSession();
//app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

// ==========================================================================
// AUTOMATISCHE DATENBANK-INITIALISIERUNG (SEED DATA)
// ==========================================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<Stammzellen.Data.DataDbContext>();

        // Stellt sicher, dass die Datenbank existiert
        context.Database.EnsureCreated();

        // Wenn noch keine Blogposts existieren, legen wir Beispiel-Daten an
        if (!context.BlogPosts.Any())
        {
            context.BlogPosts.AddRange(
                new Stammzellen.Models.BlogPost
                {
                    Titel = "Durchbruch in der iPSC-Forschung: Reprogrammierung optimiert",
                    Inhalt = "Unserem Laborteam ist es gelungen, die Effizienz der Reprogrammierung von adulten somatischen Zellen in induzierte pluripotente Stammzellen (iPS-Zellen) um 15% zu steigern. Durch den gezielten Einsatz eines neuen molekularen Cocktails konnte die Zellsterblichkeit während des Prozesses drastisch gesenkt werden. Die Vitalitätsprüfungen im Dashboard zeigen vielversprechende Langzeitergebnisse.",
                    ErstelltAm = DateTime.Now.AddDays(-5)
                },
                new Stammzellen.Models.BlogPost
                {
                    Titel = "Kryokonservierung: Optimale Lagerungsbedingungen für MSCs",
                    Inhalt = "Die langfristige Vitalität von mesenchymalen Stammzellen (MSCs) hängt entscheidend von der Abkühlrate im Kryo-Tank ab. In einer neuen Testreihe untersuchen wir die Auswirkungen verschiedener Schutzmedien auf die Zellmembran. Erste Daten wurden bereits im Labor-Dashboard dokumentiert. Wir freuen uns auf eine wissenschaftliche Diskussion in den Kommentaren.",
                    ErstelltAm = DateTime.Now.AddDays(-2)
                }
            );

            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Fehler bei der Datenbank-Initialisierung (Seed Data).");
    }
}

// Das steht schon ganz unten in deiner Program.cs
app.Run();

