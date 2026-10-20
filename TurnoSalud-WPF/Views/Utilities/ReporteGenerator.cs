using Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Views.Utilities
{
    public static class ReporteGenerator
    {
        public static void GenerarComprobanteTurnoPdf(Turno turno, string rutaArchivo)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .AlignCenter()
                        .Column(col =>
                        {
                            col.Item().Text("Comprobante de Turno Médico").SemiBold().FontSize(20).FontColor(Colors.Grey.Darken2);
                            col.Item().Text("TurnoSalud System").FontSize(9).FontColor(Colors.Grey.Medium);
                        });

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(col =>
                        {
                            col.Spacing(15);
                            col.Item().Grid(grid =>
                            {
                                grid.VerticalSpacing(5);
                                grid.Columns(2);
                                grid.Item().Text(span => span.Span("PACIENTE:").SemiBold().FontColor(Colors.Grey.Darken1));
                                grid.Item().Text(turno.NombrePaciente);
                                grid.Item().Text(span => span.Span("MÉDICO:").SemiBold().FontColor(Colors.Grey.Darken1));
                                grid.Item().Text(turno.NombreMedico);
                                grid.Item().Text(span => span.Span("ESPECIALIDAD:").SemiBold().FontColor(Colors.Grey.Darken1));
                                grid.Item().Text(turno.Especialidad);
                            });
                            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                            col.Item().Grid(grid =>
                            {
                                grid.VerticalSpacing(10);
                                grid.Columns(2);

                                grid.Item().Text(span => span.Span("FECHA:").SemiBold().FontColor(Colors.Grey.Darken1));
                                grid.Item().Text(span => span.Span($"{turno.FechaTurno:dd/MM/yyyy}").SemiBold());

                                grid.Item().Text(span => span.Span("HORARIO:").SemiBold().FontColor(Colors.Grey.Darken1));
                                grid.Item().Text(span => span.Span($"{turno.HoraInicioStr} - {turno.HoraFinStr} ({turno.Bloque})").SemiBold());
                            });
                        });

                    // --- BLOQUE CORREGIDO ---
                    page.Footer()
                     .AlignCenter()
                     .DefaultTextStyle(x => x.FontColor(Colors.Grey.Medium))
                     .Text(text =>
                     {
                         text.Span("Generado el ");
                         text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                     });

                });
            })
            .GeneratePdf(rutaArchivo);
        }
    }
}

