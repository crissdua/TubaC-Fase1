Imports SAPbobsCOM
Imports SAPbouiCOM
Imports System.Windows.Forms
Imports System.IO
Imports System.Data.SqlClient
Imports TubaC_Fase1.Batchs
Public Class FrmPrincipal
    Dim objectCode As String
    Dim itemcode As String
    Dim oCompany As SAPbobsCOM.Company = Login.con.oCompany
    Dim Connected As Boolean = Login.con.Connected
    Dim connectionString As String = Conexion.ObtenerConexion.ConnectionString
    Private Const CP_NOCLOSE_BUTTON As Integer = &H200
    Dim wo As ProductionOrders = oCompany.GetBusinessObject(BoObjectTypes.oProductionOrders)
    Dim cantidadR As Double

    Public Shared SQL_Conexion As SqlConnection = New SqlConnection()

    Protected Overloads Overrides ReadOnly Property CreateParams() As CreateParams
        Get
            Dim myCp As CreateParams = MyBase.CreateParams
            myCp.ClassStyle = myCp.ClassStyle Or CP_NOCLOSE_BUTTON
            Return myCp
        End Get
    End Property

    Public Function cargaitems()
        Dim con As SqlConnection = New SqlConnection(connectionString)
        con.Open()
        Dim SQL_da As SqlDataAdapter = New SqlDataAdapter("SELECT T0.ItemCode as Codigo, t2.itemname,T0.WhsCode as Almacen, T1.DistNumber as 'No. Lote',t1.u_ancho as Ancho,T0.quantity as 'Peso Lote', T2.ONHAND as'Total Stock',T3.ItmsGrpNam as'Grupo de Articulo' FROM OBTQ T0 INNER JOIN OBTN T1 ON T0.MdAbsEntry = T1.AbsEntry INNER JOIN OITM T2 ON T0.ItemCode = T2.ItemCode INNER JOIN OITB T3 ON T2.ItmsGrpCod = T3.ItmsGrpCod WHERE  T0.Quantity > 0 ORDER BY t1.distnumber", con)
        Dim DT_dat As System.Data.DataTable = New System.Data.DataTable()
        SQL_da.Fill(DT_dat)
        DGV.DataSource = DT_dat
        con.Close()
    End Function

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        Dim con As SqlConnection = New SqlConnection(connectionString)
        con.Open()
        Dim SQL_da As SqlDataAdapter = New SqlDataAdapter("SELECT T0.ItemCode as Codigo, t2.itemname,T0.WhsCode as Almacen, T1.DistNumber as 'No. Lote',t1.u_ancho as Ancho,T0.quantity as 'Peso Lote', T2.ONHAND as'Total Stock',T3.ItmsGrpNam as'Grupo de Articulo' FROM OBTQ T0 INNER JOIN OBTN T1 ON T0.MdAbsEntry = T1.AbsEntry INNER JOIN OITM T2 ON T0.ItemCode = T2.ItemCode INNER JOIN OITB T3 ON T2.ItmsGrpCod = T3.ItmsGrpCod WHERE  T0.Quantity > 0 and T0.itemcode LIKE '" + TextBox1.Text + "%' ORDER BY t1.distnumber", con)
        Dim DT_dat As System.Data.DataTable = New System.Data.DataTable()
        SQL_da.Fill(DT_dat)
        DGV2.DataSource = DT_dat
        con.Close()
    End Sub
    Public Sub SetMyCustomFormat()
        ' Set the Format type and the CustomFormat string.
        DateTimePicker1.Format = DateTimePickerFormat.Custom
        DateTimePicker1.CustomFormat = "yyyy/MM/dd"
    End Sub 'SetMyCustomFormat
    Private Sub FrmPrincipal_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cargaitems()
        SetMyCustomFormat()
    End Sub

    Private Sub DGV_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DGV.CellContentClick
        txtArticulo.Clear()
        labelProducto.Text = DGV(0, DGV.CurrentCell.RowIndex).Value.ToString()
    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged
        Dim con As SqlConnection = New SqlConnection(connectionString)
        con.Open()
        Dim SQL_da As SqlDataAdapter = New SqlDataAdapter("SELECT T0.ItemCode as Codigo, t2.itemname,T0.WhsCode as Almacen, T1.DistNumber as 'No. Lote',t1.u_ancho as Ancho,T0.quantity as 'Peso Lote', T2.ONHAND as'Total Stock',T3.ItmsGrpNam as'Grupo de Articulo' FROM OBTQ T0 INNER JOIN OBTN T1 ON T0.MdAbsEntry = T1.AbsEntry INNER JOIN OITM T2 ON T0.ItemCode = T2.ItemCode INNER JOIN OITB T3 ON T2.ItmsGrpCod = T3.ItmsGrpCod WHERE  T0.Quantity > 0 and T0.itemcode LIKE '" + TextBox2.Text + "%' ORDER BY t1.distnumber", con)
        Dim DT_dat As System.Data.DataTable = New System.Data.DataTable()
        SQL_da.Fill(DT_dat)
        DGV.DataSource = DT_dat
        con.Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim frm As New ItemsBox
        frm.Show()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If txtAncho.Text = String.Empty Or txtCantidad.Text = String.Empty Or txtDescM.Text = String.Empty Then
            MessageBox.Show("Verifique ancho, cantidad de tiras o descripcion")
        Else
            DGV3.Rows.Add(txtArticulo.Text, TxtDesc.Text, txtAncho.Text, txtCantidad.Text, txtDescM.Text)
            txtCantR.Enabled = False
            limpiacampos()
        End If


    End Sub

    Private Sub DGV2_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DGV2.CellContentClick
        txtArticulo.Clear()
        txtArticulo.Text = DGV2(0, DGV2.CurrentCell.RowIndex).Value.ToString()
        TxtDesc.Text = DGV2(1, DGV2.CurrentCell.RowIndex).Value.ToString()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Dim result As Integer = MessageBox.Show("Desea Ingresar la Orden?", "Atencion", MessageBoxButtons.YesNoCancel)
            If result = DialogResult.Cancel Then
                MessageBox.Show("Cancelado")
            ElseIf result = DialogResult.No Then
                MessageBox.Show("No se realizara la orden")
            ElseIf result = DialogResult.Yes Then
                Try
                    oCompany.StartTransaction()

                    If DGV3.RowCount > 0 Or labelProducto.Text = String.Empty Then

                        If Encabezadoorder() Then
                            If linesorder() Then
                                If enviaorden() = 0 Then

                                Else
                                    MessageBox.Show("Error en el envio de orden : final")
                                End If
                            Else
                                MessageBox.Show("Error en el envio de orden : lineas")
                            End If
                        Else
                            MessageBox.Show("Error en el envio de orden : encabezado")
                        End If

                        If actualizaorden() = 0 Then

                        Else
                            MessageBox.Show("Error al actualizar la orden")
                        End If

                        Dim frm As New Batchs
                        frm.load(itemcode, cantidadR, objectCode)
                        frm.ShowDialog()
                        oCompany.EndTransaction(BoWfTransOpt.wf_Commit)

                        limpiacampos()
                    Else
                        MessageBox.Show("Debe ingresar almenos una linea para la orden")
                    End If

                Catch ex As Exception
                    If oCompany.InTransaction Then
                        oCompany.EndTransaction(BoWfTransOpt.wf_RollBack)
                        MsgBox("Error: " + ex.Message.ToString)
                        limpiacampos()
                    End If
                End Try

            End If
        Catch ex As Exception
            MsgBox("Error: " + ex.Message.ToString)

        End Try

    End Sub

    Public Function limpiacampos()
        TextBox1.Clear()
        TextBox2.Clear()
        txtArticulo.Clear()
        txtAncho.Clear()
        txtCantidad.Clear()
        TxtDesc.Clear()
        txtDescM.Clear()
    End Function
    Public Function Encabezadoorder()
        Dim oReturn As Integer = -1
        Dim oError As Integer = 0
        Dim errMsg As String = ""
        Dim regresa As Integer = 0
        'Try

        If Connected = True Then
            wo.ItemNo = labelProducto.Text
            wo.DueDate = DateTimePicker1.Text
            wo.ProductionOrderType = BoProductionOrderTypeEnum.bopotSpecial
            wo.ProductionOrderStatus = SAPbobsCOM.BoProductionOrderStatusEnum.boposPlanned
            cantidadR = Convert.ToDouble(txtCantR.Text)
            wo.PlannedQuantity = cantidadR
            txtCantR.Enabled = False
            Return regresa = 1
            End If
        'Catch ex As Exception
        '    MsgBox("Error: " + ex.Message.ToString)
        'End Try
    End Function
    Public Function linesorder()
        Dim i As Integer = 0
        Dim regresa As Integer = 0
        While i < DGV3.Rows.Count
            wo.Lines.ItemNo = DGV3.Rows(i).Cells(0).Value.ToString
            wo.Lines.BaseQuantity = Convert.ToDouble(DGV3.Rows(i).Cells(3).Value)
            wo.Lines.ProductionOrderIssueType = BoIssueMethod.im_Manual
            wo.Lines.Warehouse = "BMP2"
            wo.Lines.DistributionRule = "SL4"
            wo.Lines.WipAccount = "_SYS00000000253"
            wo.Lines.Add()
            i = i + 1
        End While
        Return regresa = 1
    End Function

    'Public Function batc()
    '    Dim oReturn As Integer = -1
    '    oreceipt.Lines.Add()
    '    oReturn = oreceipt.Add
    '    If oReturn <> 0 Then
    '        MessageBox.Show(oCompany.GetLastErrorDescription)
    '    End If
    'End Function

    Public Function enviaorden()
        Try
            Dim contlinea As Integer = 0
            Dim oReturn As Integer = -1
            oReturn = wo.Add()
            If oReturn <> 0 Then
                MessageBox.Show(oCompany.GetLastErrorDescription)
            End If
            'MessageBox.Show(wo.GetNewObjectCode().ToString)
            objectCode = oCompany.GetNewObjectKey()

        Catch ex As Exception
            MsgBox("Error: " + ex.Message.ToString)
        End Try
    End Function
    Public Function actualizaorden()
        Try
            Dim oReturn As Integer = -1
            wo.GetByKey(objectCode)
            wo.ProductionOrderStatus = SAPbobsCOM.BoProductionOrderStatusEnum.boposReleased
            oReturn = wo.Update()
            If oReturn <> 0 Then
                MessageBox.Show(oCompany.GetLastErrorDescription)
            End If
        Catch ex As Exception
            MsgBox("Error: " + ex.Message.ToString)
        End Try
    End Function

    'Public Function reciboprod()
    '    oreceipt.Lines.BaseEntry = objectCode
    '    oreceipt.Lines.BaseType = 202
    '    oreceipt.Lines.Quantity = 2
    '    oreceipt.Lines.AccountCode = "_SYS00000000039"
    '    oreceipt.Lines.TransactionType = SAPbobsCOM.BoTransactionTypeEnum.botrntReject

    'End Function

    Private Sub txtArticulo_TextChanged(sender As Object, e As EventArgs) Handles txtArticulo.TextChanged

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        limpiacampos()
        wo = Nothing
        DGV.DataSource = Nothing
        DGV2.DataSource = Nothing
        DGV3.Rows.Clear()
    End Sub
End Class