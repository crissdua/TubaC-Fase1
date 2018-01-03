Public Class Batchs
    Public batchsnum As Double
    Dim cantidadR As Double
    Dim oCompany As SAPbobsCOM.Company = Login.con.oCompany
    Dim Connected As Boolean = Login.con.Connected
    Dim connectionString As String = Conexion.ObtenerConexion.ConnectionString
    Dim oreceipt As SAPbobsCOM.Documents = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry)
    Dim objectCode As Integer
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If cantidadR = Convert.ToDouble(Label6.Text) Then
            reciboprod()
            batchs()
        Else
            MessageBox.Show("Verifique que la cantidad requerida concuerde con el total consumido")
        End If
    End Sub
    Friend Sub load(itemcode As String, cantidad As Double, objectcodes As Integer)
        Label4.Text = itemcode
        Label2.Text = cantidad
        cantidadR = cantidad
        objectCode = objectcodes
    End Sub
    Private Sub Batchs_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
    Public Function reciboprod()
        oreceipt.Lines.BaseEntry = objectCode
        oreceipt.Lines.BaseType = 202
        oreceipt.Lines.Quantity = cantidadR
        oreceipt.Lines.AccountCode = "_SYS00000000039"
        oreceipt.Lines.TransactionType = SAPbobsCOM.BoTransactionTypeEnum.botrntReject

    End Function

    Public Function batchs()
        Dim oReturn As Integer = -1
        Dim line As Integer
        Dim i As Integer = 0
        While i < DGV.Rows.Count - 1
            oreceipt.Lines.BatchNumbers.SetCurrentLine(i)
            oreceipt.Lines.BatchNumbers.BatchNumber = "000-00" & DGV.Rows(i).Cells(0).Value.ToString
            oreceipt.Lines.BatchNumbers.Quantity = Convert.ToDouble(DGV.Rows(i).Cells(1).Value)
            oreceipt.Lines.BatchNumbers.AddmisionDate = "2017-12-29"
            oreceipt.Lines.BatchNumbers.Add()
            i = i + 1
        End While
        oreceipt.Lines.Add()
        oReturn = oreceipt.Add
        If oReturn <> 0 Then
            MessageBox.Show(oCompany.GetLastErrorDescription)
        Else
            MessageBox.Show("Recibo creado correctamente")
            Me.Hide()
            Dim frm As New FrmPrincipal
            frm.limpiacampos()
        End If
    End Function



    Private Sub DGV_KeyDown(sender As Object, e As KeyEventArgs) Handles DGV.KeyDown

    End Sub

    Private Sub DGV_KeyUp(sender As Object, e As KeyEventArgs) Handles DGV.KeyUp
        Dim suma As Double
        Dim queda As Double
        For Each row As DataGridViewRow In DGV.Rows
            suma += Val(row.Cells(1).Value)
        Next
        queda = Convert.ToDouble(Label2.Text) - suma
        Label6.Text = queda
        Label6.Refresh()

    End Sub
End Class