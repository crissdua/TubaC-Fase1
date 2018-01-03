Imports SAPbobsCOM
Imports SAPbouiCOM
Imports System.Windows.Forms
Imports System.IO
Imports System.Data.SqlClient
Public Class ItemsBox

    Dim oCompany As SAPbobsCOM.Company = Login.con.oCompany
    Dim Connected As Boolean = Login.con.Connected
    Dim connectionString As String = Conexion.ObtenerConexion.ConnectionString
    Dim wo As ProductionOrders = oCompany.GetBusinessObject(BoObjectTypes.oProductionOrders)
    Public Shared SQL_Conexion As SqlConnection = New SqlConnection()
    Private Sub ItemsBox_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        Dim con As SqlConnection = New SqlConnection(connectionString)
        con.Open()
        Dim SQL_da As SqlDataAdapter = New SqlDataAdapter("SELECT T0.ItemCode as Codigo, t2.itemname,T0.WhsCode as Almacen, T1.DistNumber as 'No. Lote',t1.u_ancho as Ancho,T0.quantity as 'Peso Lote', T2.ONHAND as'Total Stock',T3.ItmsGrpNam as'Grupo de Articulo' FROM OBTQ T0 INNER JOIN OBTN T1 ON T0.MdAbsEntry = T1.AbsEntry INNER JOIN OITM T2 ON T0.ItemCode = T2.ItemCode INNER JOIN OITB T3 ON T2.ItmsGrpCod = T3.ItmsGrpCod WHERE  T0.Quantity > 0 and T0.itemcode LIKE '" + TextBox1.Text + "%' ORDER BY t1.distnumber", con)
        Dim DT_dat As System.Data.DataTable = New System.Data.DataTable()
        SQL_da.Fill(DT_dat)
        DGV.DataSource = DT_dat
        con.Close()
    End Sub

    Private Sub DGV_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DGV.CellContentClick
        Me.Hide()
    End Sub

    Dim dialog As New System.Windows.Forms.Form With {
        .FormBorderStyle = FormBorderStyle.None,
        .BackColor = Color.White
        }
    Dim texto As New Label() With {
        .Text = "Esto es un popup",
        .Left = 20,
        .Top = 20
    }
    Dim boton As New System.Windows.Forms.Button() With {
        .Text = "Cerrar",
        .Top = 100,
        .Left = 100
        }

    Private Sub Button1_Click(sender As Object, e As EventArgs)
        dialog.Controls.Add(texto)
        dialog.Controls.Add(boton)
        dialog.ShowDialog()
    End Sub

End Class