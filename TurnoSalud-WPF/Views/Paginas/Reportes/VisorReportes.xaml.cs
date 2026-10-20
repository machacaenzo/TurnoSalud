using ClosedXML.Excel;
using Microsoft.Win32;
using System;
using System.Collections;
using System.ComponentModel; // Necesario para TypeDescriptor
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Views.Notificaciones;

namespace Views.Paginas.Reportes
{
    public partial class VisorReportes : Window
    {
        private readonly IEnumerable _datosReporte;
        private readonly string _tituloReporte;

        public VisorReportes(string titulo, IEnumerable datos)
        {
            InitializeComponent();
            _tituloReporte = titulo.ToUpper();
            _datosReporte = datos;

            lblTitulo.Text = _tituloReporte;
            dgReporte.ItemsSource = _datosReporte;
        }
        private void dgReporte_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            
            if (e.PropertyType == typeof(TimeSpan))
            {
                e.Cancel = true; 
            }

            string[] columnasAOcultar = { "IdTurno", "IdPaciente", "IdMedico", "IdFranja", "IdEstado" };
            if (Array.Exists(columnasAOcultar, col => col.Equals(e.PropertyName)))
            {
                e.Cancel = true;
            }

            
            if (e.PropertyType == typeof(DateTime) || e.PropertyType == typeof(DateTime?))
            {
                if (e.Column is DataGridTextColumn column)
                {
                    if (e.PropertyName.Contains("FechaRegistro"))
                    {
                        column.Binding.StringFormat = "dd/MM/yyyy HH:mm";
                    }
                    else 
                    {
                        column.Binding.StringFormat = "dd/MM/yyyy";
                    }
                }
            }

            switch (e.PropertyName)
            {
                case "HoraInicioStr":
                    e.Column.Header = "HORA INICIO";
                    break;
                case "HoraFinStr":
                    e.Column.Header = "HORA FIN";
                    break;
                case "NombrePaciente":
                    e.Column.Header = "PACIENTE";
                    break;
                case "NombreMedico":
                    e.Column.Header = "MÉDICO";
                    break;
                case "FechaTurno":
                    e.Column.Header = "FECHA TURNO";
                    break;
            }
        }


        private void btnExportarExcel_Click(object sender, RoutedEventArgs e)
        {
            if (_datosReporte == null) return;

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Archivo de Excel (*.xlsx)|*.xlsx",
                FileName = $"{_tituloReporte.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    DataTable dt = ConvertToDataTable(_datosReporte);

                    using (var workbook = new XLWorkbook())
                    {
                        string nombreHoja = _tituloReporte;
                        if (nombreHoja.Length > 31)
                        {
                            nombreHoja = nombreHoja.Substring(0, 31);
                        }

                        var worksheet = workbook.Worksheets.Add(dt, nombreHoja);


                        if (worksheet.LastRowUsed() != null && worksheet.LastRowUsed().RowNumber() > 1)
                        {
                            var firstDataRow = worksheet.Row(2);

                            foreach (var cell in firstDataRow.Cells())
                            {
                                if (cell.DataType == XLDataType.DateTime)
                                {
                                    var column = cell.WorksheetColumn();

                                    
                                    if (column.FirstCell().GetString().Contains("FechaRegistro"))
                                    {
                                        column.Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
                                    }
                                    else 
                                    {
                                        column.Style.DateFormat.Format = "dd/MM/yyyy";
                                    }
                                }
                            }
                        }

                        worksheet.Columns().AdjustToContents();

                        workbook.SaveAs(saveFileDialog.FileName);
                    }
                    new Notificacion("Reporte exportado a Excel correctamente.", Notificacion.TipoNotificacion.Exito) { Owner = this }.ShowDialog();
                }
                catch (Exception ex)
                {
                    new Notificacion("Error al exportar a Excel: " + ex.Message, Notificacion.TipoNotificacion.Error) { Owner = this }.ShowDialog();
                }
            }
        }

        
        private DataTable ConvertToDataTable(IEnumerable data)
        {
            DataTable dt = new DataTable();
            PropertyDescriptorCollection properties = null;

            foreach (var item in data)
            {
                if (properties == null)
                {
                    properties = TypeDescriptor.GetProperties(item.GetType());
                    foreach (PropertyDescriptor prop in properties)
                    {
                        dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    }
                }

                DataRow row = dt.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                dt.Rows.Add(row);
            }
            return dt;
        }


        private void btnClose_Click(object sender, RoutedEventArgs e) { this.Close(); }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); }
    }
}