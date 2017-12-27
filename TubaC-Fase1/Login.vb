Imports SAPbobsCOM
Imports SAPbouiCOM
Imports System.Windows.Forms
Imports System.IO
Imports System.Xml
Imports TubacBarCodeProduction.Conexion
Public Class Login
    Public con As New Conexion
    Private Const CP_NOCLOSE_BUTTON As Integer = &H200
    Protected Overloads Overrides ReadOnly Property CreateParams() As CreateParams
        Get
            Dim myCp As CreateParams = MyBase.CreateParams
            myCp.ClassStyle = myCp.ClassStyle Or CP_NOCLOSE_BUTTON
            Return myCp
        End Get
    End Property

    Private Sub Login_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        con.MakeConnectionSAP(TextBox1.Text, TextBox2.Text)
        'MessageBox.Show(con.Connected.ToString)
        Me.Hide()
        Dim frm As New FrmPrincipal
        frm.Show()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Hide()
        Dim frm As New cParametros
        frm.Show()
    End Sub
End Class